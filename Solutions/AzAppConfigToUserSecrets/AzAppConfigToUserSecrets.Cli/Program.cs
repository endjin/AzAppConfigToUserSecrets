using System.CommandLine.Parsing;
using AzAppConfigToUserSecrets.Cli.Infrastructure;

ICompositeConsole console = new CompositeConsole();

return await new CommandLineParser(console).Create().InvokeAsync(args, console).ConfigureAwait(false);