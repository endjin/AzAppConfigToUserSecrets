// <copyright file="ExportHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Azure;
using Azure.Data.AppConfiguration;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Spectre.Console;

namespace AzAppConfigToUserSecrets.Cli.Handlers;

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
    /// <returns>Whether the command executed successfully.</returns>
    public static async Task<int> ExecuteAsync(
        string tenantId,
        string userSecretsId,
        string endpoint)
    {
        await AnsiConsole.Status().StartAsync("Thinking...", async ctx =>
        {
            InteractiveBrowserCredential credentials = new(new InteractiveBrowserCredentialOptions { TenantId = tenantId });
            ConfigurationClient client = new(new Uri(endpoint), credentials);
            SettingSelector selector = new() { Fields = SettingFields.All };
            Pageable<ConfigurationSetting> settings = client.GetConfigurationSettings(selector);

            UserSecrets userSecrets = new(userSecretsId);
            IDictionary<string, string?> secrets = userSecrets.Load();
            ctx.Status($"Authentication against Azure Tenant {tenantId}");

            foreach (ConfigurationSetting setting in settings)
            {
                ctx.Status($"Retrieving Data from App Configuration {endpoint}");

                if (setting is SecretReferenceConfigurationSetting secretReference)
                {
                    KeyVaultSecretIdentifier identifier = new(secretReference.SecretId);
                    SecretClient secretClient = new(identifier.VaultUri, credentials);

                    Response<KeyVaultSecret>? secret = await secretClient.GetSecretAsync(identifier.Name, identifier.Version);
                    userSecrets.Set(setting.Key, secret.Value.Value);

                    continue;
                }

                userSecrets.Set(setting.Key, setting.Value);
            }

            ctx.Status($"Saving to User Secrets: {userSecretsId}");

            await userSecrets.SaveAsync().ConfigureAwait(false);
        }).ConfigureAwait(false);

        AnsiConsole.WriteLine("All Done!");

        return 0;
    }
}