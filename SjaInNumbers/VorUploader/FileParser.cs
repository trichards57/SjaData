// -----------------------------------------------------------------------
// <copyright file="FileParser.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Syncfusion.XlsIO;

namespace VorUploader;

/// <summary>
/// Helper class to parse a VOR data file.
/// </summary>
internal static class FileParser
{
    /// <summary>
    /// Parses the given file.
    /// </summary>
    /// <param name="file">The file to parse.</param>
    /// <param name="fileDate">The date of the file.</param>
    /// <returns>The parsed vor incidents.</returns>
    public static IEnumerable<VorIncident> ParseFile(string file, DateOnly fileDate)
    {
        using var excelEngine = new ExcelEngine();
        using var fileItem = File.OpenRead(file);

        var excelApp = excelEngine.Excel;

        excelApp.Workbooks.Open(fileItem);

        var sheet = excelApp.Worksheets[0];

        var columns = new Dictionary<string, int>();

        var headerRow = sheet.Rows[0];

        foreach (var c in headerRow.Cells)
        {
            if (c.Text == null)
            {
                Console.WriteLine("Warning : New format VOR file.");
                yield break;
            }

            columns[c.Text.Replace(" ", "", StringComparison.OrdinalIgnoreCase)] = c.Column;
        }

        foreach (var cols in sheet.Rows.Skip(1).Select(r => r.Columns))
        {
            var reg = cols[columns["VehicleReg"] - 1].Text.Trim().ToUpperInvariant();
            var fleetNum = cols[columns["FleetNumber"] - 1].Text?.Trim() ?? "";
            var bodyType = cols[columns["BodyType"] - 1].Text?.Trim() ?? "";
            var make = cols[columns["Make"] - 1].Text?.Trim() ?? "";
            var model = cols[columns["Model"] - 1].Text?.Trim() ?? "";
            var comments = cols[columns["Comments"] - 1].Text?.Trim() ?? "";
            var startDate = DateOnly.FromDateTime(cols[columns["StartDate"] - 1].DateTime);
            var description = cols[columns["Description"] - 1].Text?.Trim() ?? "";
            var estimatedReturnCol = columns.GetValueOrDefault("EstimatedRepairDate");
            if (estimatedReturnCol == default)
            {
                estimatedReturnCol = columns.GetValueOrDefault("ExpectedFinishDate");
            }

            if (string.IsNullOrWhiteSpace(reg) || string.IsNullOrWhiteSpace(fleetNum))
            {
                continue;
            }

            DateOnly? estimatedReturn = null;

            if (estimatedReturnCol != default)
            {
                var estimatedReturnDateTime = cols[estimatedReturnCol - 1].DateTime;
                estimatedReturn = estimatedReturnDateTime == DateTime.MinValue ? null : DateOnly.FromDateTime(estimatedReturnDateTime);
            }

            var incident = new VorIncident
            {
                CallSign = fleetNum,
                Registration = reg,
                BodyType = bodyType,
                Make = make,
                Model = model,
                Comments = comments,
                Description = description,
                EstimatedRepairDate = estimatedReturn,
                StartDate = startDate,
                UpdateDate = fileDate,
            };

            yield return incident;
        }

        excelApp.Workbooks.Close();
        fileItem.Close();
    }
}
