// <copyright file="CompositeConsole.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Spectre.Console;
using Spectre.Console.Rendering;

namespace AzAppConfigToUserSecrets.Cli.Infrastructure;

/// <summary>
/// Console wrapper for Spectre.Console operations.
/// </summary>
internal sealed class CompositeConsole : ICompositeConsole
{
    private readonly IAnsiConsole console;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompositeConsole"/> class.
    /// </summary>
    /// <param name="console">The underlying console instance.</param>
    public CompositeConsole(IAnsiConsole? console = null)
    {
        this.console = console ?? AnsiConsole.Console;
    }

    /// <inheritdoc />
    public Profile Profile => this.console.Profile;

    /// <inheritdoc />
    public IAnsiConsoleCursor Cursor => this.console.Cursor;

    /// <inheritdoc />
    public IAnsiConsoleInput Input => this.console.Input;

    /// <inheritdoc />
    public IExclusivityMode ExclusivityMode => this.console.ExclusivityMode;

    /// <inheritdoc />
    public RenderPipeline Pipeline => this.console.Pipeline;

    /// <inheritdoc />
    public void Clear(bool home) => this.console.Clear(home);

    /// <inheritdoc />
    public void Write(IRenderable renderable) => this.console.Write(renderable);
}