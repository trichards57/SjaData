// <copyright file="Program.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SJAData.Loader;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Reflection;

var version = Assembly.GetExecutingAssembly().GetName().Version;
var panel = new Panel($"[bold]SJA Data Loader[/]\nVersion:{version:3}") { Expand = true };
AnsiConsole.Write(panel);

var app = new CommandApp();

app.Configure(c =>
{
    c.AddCommand<Loader>("load");
});

var res = await app.RunAsync(args);

return res;
