﻿// <copyright file="UserSecrets.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Text.Json.Nodes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace AzAppConfigToUserSecrets.Cli;

/// <summary>
/// Performs operations against the .NET User Secrets Store.
/// </summary>
internal class UserSecrets
{
    private readonly string userSecretsId;
    private IDictionary<string, string?> secrets = new Dictionary<string, string?>();

    /// <summary>
    /// Create a new instance of the <see cref="UserSecrets"/>.
    /// </summary>
    /// <param name="userSecretsId">Id of the User Secret Store to interact with.</param>
    public UserSecrets(string userSecretsId)
    {
        this.userSecretsId = userSecretsId;
    }

    /// <summary>
    /// Retrieve the value for the given key.
    /// </summary>
    /// <param name="key">Key of the value to retrieve.</param>
    /// <returns>Value stored against the provided key.</returns>
    public string this[string key] => this.secrets[key] ?? string.Empty;

    /// <summary>
    /// Store the provided value against the provided key.
    /// </summary>
    /// <param name="key">Key to store the value.</param>
    /// <param name="value">Value to store.</param>
    public void Set(string key, string? value) => this.secrets[key] = value;

    /// <summary>
    /// Save the User Secrets Store for the currently set values.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous save operation.</returns>
    public async Task SaveAsync()
    {
        string secretsFilePath = PathHelper.GetSecretsPathFromSecretsId(this.userSecretsId);

        // Ensure directory exists
        string? directoryPath = Path.GetDirectoryName(secretsFilePath);
        if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        JsonObject jsonObject = new();

        foreach (KeyValuePair<string, string?> keyValuePair in this.secrets.AsEnumerable())
        {
            jsonObject[keyValuePair.Key] = keyValuePair.Value;
        }

        await File.WriteAllTextAsync(secretsFilePath, jsonObject.ToString()).ConfigureAwait(false);
    }

    /// <summary>
    /// Load the User Secret Store.
    /// </summary>
    /// <returns>Configuration Settings saved in the User Secrets Store.</returns>
    public IDictionary<string, string?> Load()
    {
        string secretsFilePath = PathHelper.GetSecretsPathFromSecretsId(this.userSecretsId);

        if (File.Exists(secretsFilePath))
        {
            this.secrets = new ConfigurationBuilder()
                .AddJsonFile(secretsFilePath, true)
                .Build()
                .AsEnumerable()
                .Where(i => i.Value != null)
                .ToDictionary(
                i => i.Key,
                i => i.Value,
                StringComparer.OrdinalIgnoreCase);
        }
        else
        {
            this.secrets = new Dictionary<string, string?>();
        }

        return this.secrets;
    }
}