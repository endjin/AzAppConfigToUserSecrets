// <copyright file="ICompositeConsole.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Spectre.Console;

namespace AzAppConfigToUserSecrets.Cli.Infrastructure;

/// <summary>
/// Interface for console operations using Spectre.Console.
/// </summary>
public interface ICompositeConsole : IAnsiConsole
{
}