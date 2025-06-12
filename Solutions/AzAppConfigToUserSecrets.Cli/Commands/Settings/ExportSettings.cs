// <copyright file="ExportSettings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.ComponentModel;
using Spectre.Console.Cli;

namespace AzAppConfigToUserSecrets.Cli.Commands.Settings;

/// <summary>
/// Settings for the export command.
/// </summary>
public class ExportSettings : CommandSettings
{
    /// <summary>
    /// Gets or sets the Azure AD Tenant ID.
    /// </summary>
    [CommandOption("--tenant-id")]
    [Description("Id of AAD Tenant that contains your App Configuration Service")]
    public required string TenantId { get; set; }

    /// <summary>
    /// Gets or sets the User Secrets ID.
    /// </summary>
    [CommandOption("--user-secrets-id")]
    [Description("User Secrets Id for your application")]
    public required string UserSecretsId { get; set; }

    /// <summary>
    /// Gets or sets the Azure App Configuration endpoint.
    /// </summary>
    [CommandOption("--endpoint")]
    [Description("Azure Application Configuration Service endpoint (URI)")]
    public required string Endpoint { get; set; }
}