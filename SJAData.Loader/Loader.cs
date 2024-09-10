// <copyright file="Loader.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using CsvHelper;
using Microsoft.Extensions.Options;
using SJAData.Loader.Configuration;
using SJAData.Loader.Model;
using SjaData.Model;
using SjaData.Model.Hours;
using SjaData.Server.Model;
using Spectre.Console;
using System.Globalization;
using System.Net.Http.Json;
using Region = SjaData.Model.Region;

namespace SJAData.Loader;

internal class Loader(IOptions<Settings> settings)
{
    private readonly Settings settings = settings.Value;

    internal async Task<int> ExecuteAsync(string inputFileName)
    {
        var hours = new List<Hours>();

        using (var reader = new StreamReader(inputFileName))
        using (var csv = new CsvReader(reader, CultureInfo.CurrentUICulture))
        {
            csv.Context.RegisterClassMap<HoursMap>();
            hours.AddRange(csv.GetRecords<Hours>());
        }

        var hourCount = hours.Count;

        await AnsiConsole.Progress()
            .Columns(
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new RemainingTimeColumn(),
                new SpinnerColumn()
            )
            .StartAsync(async context =>
            {
                var uploadTask = context.AddTask("Uploading Hours", true, hourCount);

                foreach (var hour in hours.Where(h => !string.IsNullOrWhiteSpace(h.Name)))
                {
                    var newEntry = new NewHoursEntry
                    {
                        Date = hour.ShiftDate,
                        Hours = hour.ShiftLength,
                        PersonId = int.Parse(hour.IdNumber),
                        Name = hour.Name,
                        Trust = CalculateTrust(hour),
                        Region = CalculateRegion(hour),
                    };

                    if (newEntry.Trust == Trust.Undefined && newEntry.Region == Region.Undefined)
                    {
                        uploadTask.Increment(1);
                        continue;
                    }

                    HttpClient client = new();

                    await client.PostAsJsonAsync(settings.HoursApiPath, newEntry, JsonContext.Default.NewHoursEntry);

                    uploadTask.Increment(1);
                }
            });

        return 0;
    }

    private static Trust CalculateTrust(Hours hour) => hour.CrewType switch
    {
        "NHS E EEAST" => Trust.EastOfEnglandAmbulanceService,
        "NHS E EMAS" => Trust.EastMidlandsAmbulanceService,
        "NHS E IOW" => Trust.IsleOfWightAmbulanceService,
        "NHS E LAS" => Trust.LondonAmbulanceService,
        "NHS E NEAS" => Trust.NorthEastAmbulanceService,
        "NHS E NWAS" => Trust.NorthWestAmbulanceService,
        "NWAS 365" => Trust.NorthWestAmbulanceService,
        "NHS E SCAS" => Trust.SouthCentralAmbulanceService,
        "NHS E SECAmb" => Trust.SouthEastCoastAmbulanceService,
        "NHS E SWAST" => Trust.SouthWesternAmbulanceService,
        "NHS E YAS" => Trust.YorkshireAmbulanceService,
        "YAS" => Trust.YorkshireAmbulanceService,
        _ => Trust.Undefined,
    };

    private static Region CalculateRegion(Hours hours)
    {
        return Region.Undefined;
    }
}
