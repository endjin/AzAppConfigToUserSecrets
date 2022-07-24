// <copyright file="ICompositeConsole.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.CommandLine;
using Spectre.Console;

namespace AzAppConfigToUserSecrets.Cli.Infrastructure;

/// <summary>
/// Interface which enabled interop between System.CommandLine and Spectre.Console.
/// </summary>
public interface ICompositeConsole : IConsole, IAnsiConsole
{
}