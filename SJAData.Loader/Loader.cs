// <copyright file="Loader.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using CsvHelper;
using SJAData.Loader.Configuration;
using SJAData.Loader.Model;
using SjaData.Model;
using SjaData.Model.Hours;
using SjaData.Server.Model;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Globalization;
using System.Net.Http.Json;

using Region = SjaData.Model.Region;

namespace SJAData.Loader;

internal class Loader : AsyncCommand<Settings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        await AnsiConsole.Progress()
            .Columns(
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new RemainingTimeColumn(),
                new SpinnerColumn())
            .StartAsync(async context =>
            {
                var peopleTask = context.AddTask("Loading People", true);
                peopleTask.IsIndeterminate = true;

                var hoursTask = context.AddTask("Loading Hours", false);

                var uploadTask = context.AddTask("Uploading Hours", false);

                var hours = new List<Hours>();
                var people = new Dictionary<int, Person>();

                using (var reader = new StreamReader(settings.PersonFile))
                using (var csv = new CsvReader(reader, CultureInfo.CurrentUICulture))
                {
                    csv.Context.RegisterClassMap<PersonMap>();

                    await foreach (var record in csv.GetRecordsAsync<Person>())
                    {
                        if (!people.TryAdd(record.MyDataNumber, record)
                            && (RolePriority(record.JobRoleTitle) > RolePriority(people[record.MyDataNumber].JobRoleTitle)
                                || (record.IsVolunteer && !people[record.MyDataNumber].IsVolunteer)))
                        {
                            people[record.MyDataNumber] = record;
                        }

                        peopleTask.Increment(1);
                    }
                }

                peopleTask.StopTask();

                hoursTask.IsIndeterminate = true;
                hoursTask.StartTask();
                using (var reader = new StreamReader(settings.InputFile))
                using (var csv = new CsvReader(reader, CultureInfo.CurrentUICulture))
                {
                    csv.Context.RegisterClassMap<HoursMap>();

                    await foreach (var h in csv.GetRecordsAsync<Hours>())
                    {
                        hours.Add(h);
                        hoursTask.Increment(1);
                    }
                }

                hoursTask.StopTask();

                uploadTask.MaxValue = hours.Count;
                uploadTask.StartTask();

                foreach (var hour in hours.Where(h => !string.IsNullOrWhiteSpace(h.Name)))
                {
                    if (!people.TryGetValue(int.Parse(hour.IdNumber), out var person))
                    {
                        uploadTask.Increment(1);
                        continue;
                    }

                    var newEntry = new NewHoursEntry
                    {
                        Date = hour.ShiftDate,
                        Hours = hour.ShiftLength,
                        PersonId = int.Parse(hour.IdNumber),
                        Name = hour.Name,
                        Trust = CalculateTrust(hour),
                        Region = CalculateRegion(person, hour),
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

                uploadTask.StopTask();
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

    private static int RolePriority(string role) => role.ToLowerInvariant() switch
    {
        "emergency ambulance crew" => 2,
        "associate ambulance practitioner" => 3,
        "emergency medical technician" => 3,
        "local ambulance lead" => 0,
        "emergency transport attendant" => 1,
        "district ambulance lead" => 0,
        "ambulance care assistant" => 1,
        "ihcd technician" => 3,
        "director of urgent and emergency care" => 0,
        "ambulance locality manager (castle donington)" => 0,
        "regional clinical lead - ambulance operations support" => 0,
        "operations manager" => 0,
        "district clinical officer - student paramedics and ambulance operations liason" => 0,
        "regional clinical education lead(nw-ne-em)" => 0,
        "clinical educator" => 0,
        "ambulance crew – paramedic" => 4,
        "clinical educator lead" => 0,
        "patient transport attendant" => 1,
        "event emergency ambulance crew" => 1,
        "clinical educator (non-hcp)" => 0,
        "ihcd practitioner" => 3,
        "regional ambulance lead (south east)" => 0,
        "regional ambulance lead (east of england)" => 0,
        "regional ambulance lead (south west)" => 0,
        "ambulance and clinical training officer" => 0,
        "ambulance hub lead" => 0,
        "regional ambulance and clinical training delivery lead" => 0,
        "district ambulance lead - without portfolio" => 0,
        _ => throw new InvalidOperationException(),
    };

    private static Region CalculateRegion(Person person, Hours hours)
    {
        if (hours.CrewType.Equals("Event Cover Amb", StringComparison.InvariantCultureIgnoreCase))
        {
            return person.DepartmentRegion.ToLowerInvariant() switch
            {
                "london region" => Region.London,
                "events: london" => Region.London,
                "east of england region" => Region.EastOfEngland,
                "north east region" => Region.NorthEast,
                "south east region" => Region.SouthEast,
                "west midlands region" => Region.WestMidlands,
                "east midlands region" => Region.EastMidlands,
                "south west region" => Region.SouthWest,
                "north west region" => Region.NorthWest,
                _ => Region.Undefined,
            };
        }

        return Region.Undefined;
    }
}
