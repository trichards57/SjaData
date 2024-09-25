// <copyright file="Program.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Compliance.Classification;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using SjaData.Server.Controllers.Binders;
using SjaData.Server.Controllers.Filters;
using SjaData.Server.Data;
using SjaData.Server.DataTypes;
using SjaData.Server.Logging;
using SjaData.Server.Model;
using SjaData.Server.Model.Converters;
using SjaData.Server.Services;
using SjaData.Server.Services.Interfaces;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Runtime.CompilerServices;
using System.Security.Claims;

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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        var configSection = builder.Configuration.GetRequiredSection("AzureAd");

        o.MetadataAddress = $"https://login.microsoftonline.com/{configSection.GetValue<string>("TenantId")}/.well-known/openid-configuration";
        o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidAudience = configSection.GetValue<string>("ClientId"),
            ValidateSignatureLast = true,
        };
        o.Events = new JwtBearerEvents
        {
            OnTokenValidated = async c =>
            {
                var db = c.HttpContext.RequestServices.GetRequiredService<DataContext>();

                if (c.Principal == null || c.Principal.Identity == null)
                {
                    return;
                }

                var principal = c.Principal;

                var id = principal.GetNameIdentifierId();

                if (string.IsNullOrEmpty(id))
                {
                    return;
                }

                var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user is null)
                {
                    user = new User
                    {
                        Id = id,
                        Name = c.Principal?.FindFirstValue("name") ?? "User",
                        Role = Role.None,
                    };
                    db.Users.Add(user);
                    try
                    {
                        await db.SaveChangesAsync();
                    }
                    catch (DbUpdateException)
                    {
                        // There was probably a concurrent request and that one got there first.
                        user = await db.Users.FirstOrDefaultAsync(u => u.Id == id) ?? throw new InvalidOperationException();
                    }
                }

                // Add the role to the claims
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Role, user.Role.ToString()),
                    new(ClaimTypes.Name, user.Name),
                };

                var identity = new ClaimsIdentity(claims, principal.Identity.AuthenticationType);
                principal.AddIdentity(identity);
            },
        };
    });

builder.Services.AddAuthorizationCore(o =>
{
    o.AddPolicy("Admin", p => p.RequireClaim(ClaimTypes.Role, Role.Admin.ToString()));
    o.AddPolicy("Lead", p => p.RequireClaim(ClaimTypes.Role, Role.Admin.ToString(), Role.Lead.ToString()));
    o.AddPolicy("User", p => p.RequireClaim(ClaimTypes.Role, Role.Admin.ToString(), Role.Lead.ToString(), Role.None.ToString()));
});

builder.Services.AddTransient<IHoursService, HoursService>();
builder.Services.AddTransient<IPersonService, PersonService>();
builder.Services.AddTransient<IUserService, UserService>();

builder.Services.AddRedaction(c =>
{
    c.SetRedactor<StarRedactor>(new DataClassificationSet(DataClassifications.PatientData));
});

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(o =>
{
    o.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "SjaData.Server.xml"));
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
    o.OperationFilter<SwaggerDefaultValues>();
    o.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
    o.OperationFilter<SecurityRequirementsOperationFilter>();

    o.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
    });
});

builder.Services.AddApiVersioning().AddMvc().AddApiExplorer();

builder.WebHost.ConfigureKestrel(o =>
{
    o.Limits.MaxRequestBodySize = 100_000_000;
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI(o =>
{
    foreach (var desc in app.DescribeApiVersions().Select(s => s.GroupName))
    {
        o.SwaggerEndpoint($"/swagger/{desc}/swagger.json", desc);
    }
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

await app.RunAsync();
