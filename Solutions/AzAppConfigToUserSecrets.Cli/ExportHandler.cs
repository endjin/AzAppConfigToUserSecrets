// <copyright file="ExportHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.CommandLine.Invocation;
using AzAppConfigToUserSecrets.Cli.Infrastructure;
using Azure;
using Azure.Data.AppConfiguration;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Spectre.Console;

namespace AzAppConfigToUserSecrets.Cli;

/// <summary>
/// Exports settings data from the Azure App Configuration Service to .NET User Settings.
/// </summary>
internal static class ExportHandler
{
    /// <summary>
    /// Command Handler invoked by the Export command.
    /// </summary>
    /// <param name="tenantId">Azure AD Tenant Id.</param>
    /// <param name="userSecretsId">User Secret Id to write settings to.</param>
    /// <param name="endpoint">Azure App Configuration Service URL endpoint.</param>
    /// <param name="console">Console to write output to.</param>
    /// <param name="context">System.CommandLine InvocationContext for Command Line Args.</param>
    /// <returns>Whether the command executed successfully.</returns>
    public static async Task<int> ExecuteAsync(
        string tenantId,
        string userSecretsId,
        string endpoint,
        ICompositeConsole console,
        InvocationContext? context = null)
    {
        await console.Status().StartAsync("Thinking...", async ctx =>
        {
            var credentials = new InteractiveBrowserCredential(new InteractiveBrowserCredentialOptions { TenantId = tenantId });
            var client = new ConfigurationClient(new Uri(endpoint), credentials);
            var selector = new SettingSelector { Fields = SettingFields.All };
            Pageable<ConfigurationSetting> settings = client.GetConfigurationSettings(selector);

            var userSecrets = new UserSecrets(userSecretsId);
            IDictionary<string, string> secrets = userSecrets.Load();
            ctx.Status($"Authentication against Azure Tenant {tenantId}");

            foreach (ConfigurationSetting setting in settings)
            {
                ctx.Status($"Retrieving Data from App Configuration {endpoint}");

                if (setting is SecretReferenceConfigurationSetting secretReference)
                {
                    var identifier = new KeyVaultSecretIdentifier(secretReference.SecretId);
                    var secretClient = new SecretClient(identifier.VaultUri, credentials);

                    Response<KeyVaultSecret>? secret = await secretClient.GetSecretAsync(identifier.Name, identifier.Version);
                    userSecrets.Set(setting.Key, secret.Value.Value);

                    continue;
                }

                userSecrets.Set(setting.Key, setting.Value);
            }

            ctx.Status($"Saving to User Secrets: {userSecretsId}");

            await userSecrets.SaveAsync().ConfigureAwait(false);
        }).ConfigureAwait(false);

        console.WriteLine("All Done!");

        return 0;
    }
}