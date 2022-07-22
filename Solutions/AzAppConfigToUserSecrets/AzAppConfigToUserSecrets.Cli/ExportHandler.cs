// <copyright file="ExportHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.CommandLine.Invocation;
using System.Text.Json;
using AzAppConfigToUserSecrets.Cli.Infrastructure;
using Azure;
using Azure.Data.AppConfiguration;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Spectre.Console;

namespace AzAppConfigToUserSecrets.Cli;

public static class ExportHandler
{
    public static Task<int> ExecuteAsync(
      string tenantId,
      string userSecretsId,
      string endpoint,
      ICompositeConsole console,
      InvocationContext context = null)
    {
        ConfigurationClient? configurationClient;

        var userSecretsStore = new UserSecretsStore(userSecretsId);

        var secrets = userSecretsStore.Load();

        var credentials = new DefaultAzureCredential(new DefaultAzureCredentialOptions
        {
            InteractiveBrowserTenantId = tenantId,
            SharedTokenCacheTenantId = tenantId,
            VisualStudioCodeTenantId = tenantId,
            VisualStudioTenantId = tenantId,
        });

        configurationClient = new ConfigurationClient(new Uri(endpoint), credentials);

        var selector = new SettingSelector { Fields = SettingFields.All };
        Pageable<ConfigurationSetting> settings = configurationClient.GetConfigurationSettings(selector);

        foreach (ConfigurationSetting setting in settings)
        {
            if (setting.ContentType == "application/vnd.microsoft.appconfig.keyvaultref+json;charset=utf-8")
            {
                Uri secretUri = new Uri(JsonSerializer.Deserialize<JsonElement>(setting.Value).GetProperty("uri").GetString());
                Uri vaultUri = new Uri(secretUri.GetLeftPart(UriPartial.Authority));
                string secretName = ""; // TODO: pull out secret name from URI. What if there is an explicit version in there?

                SecretClient secretClient = new SecretClient(vaultUri, credentials);
                var secret = await secretClient.GetSecretAsync(secretName);
                // TODO set value
            }

            console.WriteLine($"{setting.Key} / {setting.Value}");
            userSecretsStore.Set(setting.Key, setting.Value);
        }

        userSecretsStore.Save();
        return Task.FromResult(0);
    }
}