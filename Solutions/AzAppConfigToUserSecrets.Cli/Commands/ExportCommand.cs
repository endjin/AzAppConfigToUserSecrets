// <copyright file="ExportCommand.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using AzAppConfigToUserSecrets.Cli.Commands.Settings;
using AzAppConfigToUserSecrets.Cli.Handlers;
using Spectre.Console;
using Spectre.Console.Cli;

namespace AzAppConfigToUserSecrets.Cli.Commands;

/// <summary>
/// Command to export settings from Azure App Configuration to .NET User Secrets.
/// </summary>
public class ExportCommand : AsyncCommand<ExportSettings>
{
    /// <summary>
    /// Executes the export command.
    /// </summary>
    /// <param name="context">The command context.</param>
    /// <param name="settings">The command settings.</param>
    /// <returns>The exit code.</returns>
    public override async Task<int> ExecuteAsync(CommandContext context, ExportSettings settings)
    {
        try
        {
            return await ExportHandler.ExecuteAsync(
                settings.TenantId,
                settings.UserSecretsId,
                settings.Endpoint).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            return 1;
        }
    }
}