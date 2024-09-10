// <copyright file="Program.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SJAData.Loader;

var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureServices(services => { services.AddTransient<Loader>(); })
    .Build();

if (args.Length == 0)
{
    Console.WriteLine("Please specify an input file name.");
    return -1;
}

if (!File.Exists(args[0]))
{
    Console.WriteLine("The specified file does not exist.");
    return -1;
}

var loader = host.Services.GetRequiredService<Loader>();
return await loader.ExecuteAsync(args[0]);
