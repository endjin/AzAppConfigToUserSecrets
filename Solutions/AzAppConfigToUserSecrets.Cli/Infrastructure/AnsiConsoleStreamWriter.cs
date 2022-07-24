// <copyright file="AnsiConsoleStreamWriter.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.CommandLine.IO;
using Spectre.Console;

namespace AzAppConfigToUserSecrets.Cli.Infrastructure;

/// <summary>
/// Write to <see cref="IAnsiConsole"/>.
/// </summary>
internal sealed class AnsiConsoleStreamWriter : IStandardStreamWriter
{
    private readonly IAnsiConsole console;

    /// <summary>
    /// Create a new AnsiConsoleStreamWriter.
    /// </summary>
    /// <param name="console">Represents a console.</param>
    public AnsiConsoleStreamWriter(IAnsiConsole console)
    {
        this.console = console;
    }

    /// <inheritdoc />
    public void Write(string? value)
    {
        this.console.Write(value!);
    }
}