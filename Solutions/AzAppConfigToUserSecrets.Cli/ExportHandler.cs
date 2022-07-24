// <copyright file="ExportHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.CommandLine.Invocation;
using AzAppConfigToUserSecrets.Cli.Infrastructure;
using Azure;
using Azure.Data.AppConfiguration;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

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
    var credentials = new InteractiveBrowserCredential(new InteractiveBrowserCredentialOptions { TenantId = tenantId });
    var client = new ConfigurationClient(new Uri(endpoint), credentials);

    var selector = new SettingSelector { Fields = SettingFields.All };
    Pageable<ConfigurationSetting> settings = client.GetConfigurationSettings(selector);

    var userSecretsStore = new UserSecretsStore(userSecretsId);
    var secrets = userSecretsStore.Load();

    foreach (ConfigurationSetting setting in settings)
    {
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

    userSecretsStore.Save();

    return 0;
  }
}