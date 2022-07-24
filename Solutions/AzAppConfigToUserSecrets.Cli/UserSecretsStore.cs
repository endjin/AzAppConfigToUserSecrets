// <copyright file="UserSecretsStore.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Text.Json.Nodes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace AzAppConfigToUserSecrets.Cli;

internal class UserSecretsStore
{
    private readonly string userSecretsId;
    private IDictionary<string, string> secrets = new Dictionary<string, string>();

    public UserSecretsStore(string userSecretsId)
    {
        this.userSecretsId = userSecretsId;
    }

    public string this[string key] => this.secrets[key];

    public void Set(string key, string value) => this.secrets[key] = value;

    public void Save()
    {
        string? secretsFilePath = PathHelper.GetSecretsPathFromSecretsId(this.userSecretsId);
        JsonObject jsonObject = new();

        foreach (KeyValuePair<string, string> keyValuePair in this.secrets.AsEnumerable())
        {
          jsonObject[keyValuePair.Key] = keyValuePair.Value;
        }

        File.WriteAllText(secretsFilePath, jsonObject.ToString());
    }

    public IDictionary<string, string> Load()
    {
        string? secretsFilePath = PathHelper.GetSecretsPathFromSecretsId(this.userSecretsId);

        this.secrets = new ConfigurationBuilder()
          .AddJsonFile(secretsFilePath, true)
          .Build()
          .AsEnumerable()
          .Where((Func<KeyValuePair<string, string>, bool>)(i => i.Value != null))
          .ToDictionary(
            i => i.Key,
            i => i.Value,
            StringComparer.OrdinalIgnoreCase);

        return this.secrets;
    }
}