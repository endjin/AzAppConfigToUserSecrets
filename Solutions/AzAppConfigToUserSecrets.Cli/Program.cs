// <copyright file="Program.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using AzAppConfigToUserSecrets.Cli.Commands;
using Spectre.Console.Cli;

CommandApp app = new();

app.Configure(config =>
{
    config.SetApplicationName("actus");
    config.SetApplicationVersion("1.0.0");
    
    config.AddCommand<ExportCommand>("export")
        .WithDescription("Exports settings from Azure App Configuration to .NET User Secrets.");
});

return await app.RunAsync(args);