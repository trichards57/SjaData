// <copyright file="Program.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Compliance.Classification;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using SjaData.Model;
using SjaData.Model.Converters;
using SjaData.Model.DataTypes;
using SjaData.Server.Controllers.Binders;
using SjaData.Server.Controllers.Filters;
using SjaData.Server.Data;
using SjaData.Server.Logging;
using SjaData.Server.Model;
using SjaData.Server.Services;
using SjaData.Server.Services.Interfaces;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("SjaData.Server.Tests")]

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonOptions>(j =>
{
    j.SerializerOptions.TypeInfoResolver = JsonContext.Default;
});

builder.Services.AddControllers(o =>
{
    o.ModelBinderProviders.Add(new CustomBinderProvider());
});

builder.Services.AddDbContext<DataContext>(o =>
{
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<DataContext>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration);

builder.Services.AddTransient<IHoursService, HoursService>();
builder.Services.AddTransient<IPatientService, PatientService>();

builder.Services.AddRedaction(c =>
{
    c.SetRedactor<StarRedactor>(new DataClassificationSet(DataClassifications.PatientData));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(o =>
{
    o.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "SjaData.Server.xml"));
    o.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "SjaData.Model.xml"));
    o.MapType<DateType>(() => DateTypeBinder.Schema);
    o.MapType<Region>(() => RegionBinder.Schema);
    o.MapType<Trust>(() => TrustBinder.Schema);
    o.MapType<TimeSpan>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "date-span",
        Example = new OpenApiString("1.10:09:08"),
    });
    o.MapType<AreaDictionary<int>>(() => new OpenApiSchema
    {
        Type = "object",
        Properties = TrustConverter.GetNames().Concat(RegionConverter.GetNames()).ToDictionary(s => s, s => new OpenApiSchema
        {
            Nullable = true,
            Type = "integer",
            Format = "int32",
        }),
        AdditionalPropertiesAllowed = false,
    });
    o.MapType<AreaDictionary<TimeSpan>>(() => new OpenApiSchema
    {
        Type = "object",
        Properties = TrustConverter.GetNames().Concat(RegionConverter.GetNames()).ToDictionary(s => s, s => new OpenApiSchema
        {
            Nullable = true,
            Type = "string",
            Format = "date-span",
            Example = new OpenApiString("1.10:09:08"),
        }),
        AdditionalPropertiesAllowed = false,
    });
    o.AddSchemaFilterInstance(new RegionOrTrustSchemaFilter());
    o.AddSchemaFilterInstance(new GreaterThanSchemaFilter());
    o.OperationFilter<SwaggerDefaultValues>();
    o.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
    o.OperationFilter<SecurityRequirementsOperationFilter>();

    o.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri("https://login.microsoftonline.com/15d371bd-0830-4361-8629-598fc9162fdd/oauth2/v2.0/authorize"),
                TokenUrl = new Uri("https://login.microsoftonline.com/15d371bd-0830-4361-8629-598fc9162fdd/oauth2/v2.0/token"),
                RefreshUrl = new Uri("https://login.microsoftonline.com/15d371bd-0830-4361-8629-598fc9162fdd/oauth2/v2.0/token"),
            },
        },
    });
});

builder.Services.AddApiVersioning().AddMvc().AddApiExplorer();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI(o =>
{
    foreach (var desc in app.DescribeApiVersions())
    {
        o.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json", desc.GroupName);
    }
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

await app.RunAsync();
