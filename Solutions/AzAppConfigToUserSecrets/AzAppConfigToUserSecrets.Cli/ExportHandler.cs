// <copyright file="ExportHandler.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.CommandLine.Invocation;
using AzAppConfigToUserSecrets.Cli.Infrastructure;
using Azure;
using Azure.Data.AppConfiguration;
using Azure.Identity;
using Spectre.Console;

namespace AzAppConfigToUserSecrets.Cli;

public static class ExportHandler
{
  public static Task<int> ExecuteAsync(
    string tenantId, 
    string userSecretsId,
    string endpoint,
    string connectionString, 
    bool useCredentials,
    ICompositeConsole console,
    InvocationContext context = null)
  {
    ConfigurationClient? client;

    var userSecretsStore = new UserSecretsStore(userSecretsId);

    var secrets = userSecretsStore.Load();

    if (useCredentials)
    {
      var credentials = new DefaultAzureCredential(new DefaultAzureCredentialOptions
      {
        InteractiveBrowserTenantId = tenantId,
        SharedTokenCacheTenantId = tenantId,
        VisualStudioCodeTenantId = tenantId,
        VisualStudioTenantId = tenantId,
      });

      client = new ConfigurationClient(new Uri(endpoint), credentials);
    }
    else
    {
      client = new ConfigurationClient(connectionString);
    }

    var selector = new SettingSelector { Fields = SettingFields.All };
    Pageable<ConfigurationSetting> settings = client.GetConfigurationSettings(selector);

    foreach (ConfigurationSetting setting in settings)
    {
      console.WriteLine($"{setting.Key} / {setting.Value}");
      userSecretsStore.Set(setting.Key, setting.Value);
    }

    userSecretsStore.Save();
    return Task.FromResult(0);
  }
}