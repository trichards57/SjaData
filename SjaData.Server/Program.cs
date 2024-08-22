// <copyright file="Program.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Compliance.Classification;
using Microsoft.Extensions.Compliance.Redaction;
using Microsoft.Identity.Web;
using SjaData.Model.DataTypes;
using SjaData.Server.Api;
using SjaData.Server.Data;
using SjaData.Server.Data.Compiled;
using SjaData.Server.Logging;
using SjaData.Server.Model;
using SjaData.Server.Services;
using SjaData.Server.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonOptions>(j =>
{
    j.SerializerOptions.TypeInfoResolver = JsonContext.Default;
});

builder.Services.AddDbContext<DataContext>(o =>
{
    o.UseModel(DataContextModel.Instance)
     .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
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

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

app.MapPatientApi();
app.MapHoursApi();

app.MapFallbackToFile("/index.html");

await app.RunAsync();
