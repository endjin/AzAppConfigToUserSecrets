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

public static class ExportHandler
{
  public static async Task<int> ExecuteAsync(
    string tenantId,
    string userSecretsId,
    string endpoint,
    ICompositeConsole console,
    InvocationContext? context = null)
  {
    await console.Status().StartAsync("Thinking...", async ctx =>
    {
      var credentials = new InteractiveBrowserCredential(new InteractiveBrowserCredentialOptions {TenantId = tenantId});
      var client = new ConfigurationClient(new Uri(endpoint), credentials);
      
      var selector = new SettingSelector {Fields = SettingFields.All};
      Pageable<ConfigurationSetting> settings = client.GetConfigurationSettings(selector);

      var userSecretsStore = new UserSecretsStore(userSecretsId);
      var secrets = userSecretsStore.Load();
      ctx.Status($"Authentication against Azure Tenant {tenantId}");
      
      foreach (ConfigurationSetting setting in settings)
      {
        ctx.Status($"Retrieving Data from App Configuration {endpoint}");

        if (setting is SecretReferenceConfigurationSetting secretReference)
        {
          var identifier = new KeyVaultSecretIdentifier(secretReference.SecretId);
          var secretClient = new SecretClient(identifier.VaultUri, credentials);
          Response<KeyVaultSecret>? secret = await secretClient.GetSecretAsync(identifier.Name, identifier.Version);
          userSecretsStore.Set(setting.Key, secret.Value.Value);
          continue;
        }

        userSecretsStore.Set(setting.Key, setting.Value);
      }

      ctx.Status($"Saving to User Secrets: {userSecretsId}");

      userSecretsStore.Save();
    }).ConfigureAwait(false);
      
    console.WriteLine("All Done!");

    return 0;
  }
}