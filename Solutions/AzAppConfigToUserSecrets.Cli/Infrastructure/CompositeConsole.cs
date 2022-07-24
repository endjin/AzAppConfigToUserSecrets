// <copyright file="CompositeConsole.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.CommandLine.IO;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace AzAppConfigToUserSecrets.Cli.Infrastructure;

/// <summary>
/// Provides interoperability between System.CommandLine and Spectre.Console.
/// </summary>
internal sealed class CompositeConsole : ICompositeConsole
{
    private readonly AnsiConsoleStreamWriter standardOut;
    private readonly IStandardStreamWriter standardError;

    /// <summary>
    /// Create a new instance of CompositeConsole.
    /// </summary>
    public CompositeConsole()
    {
        this.standardOut = new AnsiConsoleStreamWriter(AnsiConsole.Console);
        this.standardError = StandardStreamWriter.Create(Console.Error);
    }

    /// <inheritdoc />
    bool IStandardOut.IsOutputRedirected => Console.IsOutputRedirected;

    /// <inheritdoc />
    bool IStandardError.IsErrorRedirected => Console.IsErrorRedirected;

    /// <inheritdoc />
    bool IStandardIn.IsInputRedirected => Console.IsInputRedirected;

    /// <inheritdoc />
    IStandardStreamWriter IStandardOut.Out => this.standardOut;

    /// <inheritdoc />
    IStandardStreamWriter IStandardError.Error => this.standardError;

    /// <inheritdoc />
    public Profile Profile => AnsiConsole.Console.Profile;

    /// <inheritdoc />
    public IAnsiConsoleCursor Cursor => AnsiConsole.Console.Cursor;

    /// <inheritdoc />
    public IAnsiConsoleInput Input => AnsiConsole.Console.Input;

    /// <inheritdoc />
    public IExclusivityMode ExclusivityMode => AnsiConsole.Console.ExclusivityMode;

    /// <inheritdoc />
    public RenderPipeline Pipeline => AnsiConsole.Console.Pipeline;

    /// <inheritdoc />
    public void Clear(bool home)
    {
        AnsiConsole.Console.Clear(home);
    }

    /// <inheritdoc />
    public void Write(IRenderable renderable)
    {
        AnsiConsole.Console.Write(renderable);
    }
}