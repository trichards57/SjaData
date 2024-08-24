// <copyright file="Program.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Compliance.Classification;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using SjaData.Model;
using SjaData.Model.Converters;
using SjaData.Model.DataTypes;
using SjaData.Server.Api;
using SjaData.Server.Data;
using SjaData.Server.Logging;
using SjaData.Server.Model;
using SjaData.Server.Services;
using SjaData.Server.Services.Interfaces;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("SjaData.Server.Tests")]

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonOptions>(j =>
{
    j.SerializerOptions.TypeInfoResolver = JsonContext.Default;
});

builder.Services.AddDbContext<DataContext>(o =>
{
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<DataContext>();

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration);

builder.Services.AddTransient<IHoursService, HoursService>();
builder.Services.AddTransient<IPatientService, PatientService>();

builder.Services.AddRedaction(c =>
{
    c.SetRedactor<StarRedactor>(new DataClassificationSet(DataClassifications.PatientData));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "SjaData.Server.xml"));
    o.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "SjaData.Model.xml"));
    o.MapType<Region>(() => RegionConverter.Schema);
    o.MapType<Trust>(() => TrustConverter.Schema);
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
    });
    o.AddSchemaFilterInstance(new RegionOrTrustSchemaFilter());
    o.AddSchemaFilterInstance(new GreaterThanSchemaFilter());
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPatientApi();
app.MapHoursApi();

app.MapFallbackToFile("/index.html");

await app.RunAsync();
