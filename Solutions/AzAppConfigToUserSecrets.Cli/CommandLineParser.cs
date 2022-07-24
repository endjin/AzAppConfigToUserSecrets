using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using AzAppConfigToUserSecrets.Cli;
using AzAppConfigToUserSecrets.Cli.Infrastructure;

public class CommandLineParser
{
  private readonly ICompositeConsole console;

  public CommandLineParser(ICompositeConsole console)
  {
    this.console = console;
  }

  public delegate Task Export(string tenantId, string userSecretsId, string endpoint, ICompositeConsole console, InvocationContext? invocationContext = null);

  public Parser Create(Export? export = null)
  {
    // if environmentInit hasn't been provided (for testing) then assign the Command Handler
    export ??= ExportHandler.ExecuteAsync;

    RootCommand rootCommand = Root();
    rootCommand.AddCommand(Export());

    var commandBuilder = new CommandLineBuilder(rootCommand);

    return commandBuilder.UseDefaults().Build();

    static RootCommand Root()
    {
      return new RootCommand
      {
        Name = "actus",
        Description = "Export Azure App Configuration to .NET User Secrets",
      };
    }

    Command Export()
    {
      var cmd = new Command("export", "");

      var tenantIdArg = new Option<string>("--tenant-id")
      {
        Description = "Id of AAD Tenant that contains your App Configuration Service",
        Arity = ArgumentArity.ExactlyOne,
        IsRequired = true,
      };

      var userSecretsIdArg = new Option<string>("--user-secrets-id")
      {
        Description = "User Secrets Id for your application",
        Arity = ArgumentArity.ExactlyOne,
        IsRequired = true,
      };

      var endpointArg = new Option<string>("--endpoint")
      {
        Description = "Azure Application Configuration Service endpoint (URI)",
        Arity = ArgumentArity.ExactlyOne,
        IsRequired = true,
      };

      cmd.AddOption(tenantIdArg);
      cmd.AddOption(userSecretsIdArg);
      cmd.AddOption(endpointArg);

      cmd.SetHandler(async (context) =>
      {
        string? tenantId = context.ParseResult.GetValueForOption(tenantIdArg);
        string? userSecretsId = context.ParseResult.GetValueForOption(userSecretsIdArg);
        string? endpoint = context.ParseResult.GetValueForOption(endpointArg);

        await export(tenantId!, userSecretsId!, endpoint!, (ICompositeConsole)context.Console, context).ConfigureAwait(false);
      });

      return cmd;
    }
  }
}