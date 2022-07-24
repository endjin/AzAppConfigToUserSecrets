// <copyright file="CommandLineParser.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using AzAppConfigToUserSecrets.Cli;
using AzAppConfigToUserSecrets.Cli.Infrastructure;

/// <summary>
/// Parses the Command Line Args.
/// </summary>
internal class CommandLineParser
{
    private readonly ICompositeConsole console;

    /// <summary>
    /// Create a new instance of <see cref="CommandLineParser"/>.
    /// </summary>
    /// <param name="console">CompositeConsole to write output to.</param>
    public CommandLineParser(ICompositeConsole console)
    {
        this.console = console;
    }

    /// <summary>
    /// Represents the method to invoke when the export command is requested.
    /// </summary>
    /// <param name="tenantId">Azure AD Tenant Id.</param>
    /// <param name="userSecretsId">User Secret Id to write settings to.</param>
    /// <param name="endpoint">Azure App Configuration Service URL endpoint.</param>
    /// <param name="console">Console to write output to.</param>
    /// <param name="invocationContext">System.CommandLine InvocationContext for Command Line Args.</param>
    /// <returns>Whenther the command executed correctly.</returns>
    public delegate Task<int> Export(string tenantId, string userSecretsId, string endpoint, ICompositeConsole console, InvocationContext? invocationContext = null);

    /// <summary>
    /// Creates configured Command Line Parser.
    /// </summary>
    /// <param name="export">Method to invoke when the export command is requested.</param>
    /// <returns>New Command Line Parser.</returns>
    public Parser Create(Export? export = null)
    {
        // if environmentInit hasn't been provided (for testing) then assign the Command Handler
        export ??= ExportHandler.ExecuteAsync;

        RootCommand rootCommand = Root();
        rootCommand.AddCommand(Export());

        var commandBuilder = new CommandLineBuilder(rootCommand);

        return commandBuilder.UseDefaults().Build();

        static RootCommand Root()
        {
            return new RootCommand
            {
                Name = "actus",
                Description = "Export Azure App Configuration to .NET User Secrets",
            };
        }

        Command Export()
        {
            var cmd = new Command("export", "Exports settings from Azure App Configuration to .NET User Secrets.");

            var tenantIdArg = new Option<string>("--tenant-id")
            {
                Description = "Id of AAD Tenant that contains your App Configuration Service",
                Arity = ArgumentArity.ExactlyOne,
                IsRequired = true,
            };

            var userSecretsIdArg = new Option<string>("--user-secrets-id")
            {
                Description = "User Secrets Id for your application",
                Arity = ArgumentArity.ExactlyOne,
                IsRequired = true,
            };

            var endpointArg = new Option<string>("--endpoint")
            {
                Description = "Azure Application Configuration Service endpoint (URI)",
                Arity = ArgumentArity.ExactlyOne,
                IsRequired = true,
            };

            cmd.AddOption(tenantIdArg);
            cmd.AddOption(userSecretsIdArg);
            cmd.AddOption(endpointArg);

            cmd.SetHandler(async (context) =>
            {
                string? tenantId = context.ParseResult.GetValueForOption(tenantIdArg);
                string? userSecretsId = context.ParseResult.GetValueForOption(userSecretsIdArg);
                string? endpoint = context.ParseResult.GetValueForOption(endpointArg);

                await export(tenantId!, userSecretsId!, endpoint!, (ICompositeConsole)context.Console, context)
                    .ConfigureAwait(false);
            });

            return cmd;
        }
    }
}