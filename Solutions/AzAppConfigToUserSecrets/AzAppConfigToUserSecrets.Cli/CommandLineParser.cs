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

  public delegate Task Export(string tenantId, string userSecretsId, string endpoint, string connectionString, bool useCredentials, ICompositeConsole console, InvocationContext invocationContext = null);

  public Parser Create(Export export = null)
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
        Description = "Export Azure App Configuration to User Secrets",
      };
    }

    Command Export()
    {
      var cmd = new Command("export", "");

      var tenantIdArg = new Option<string>("tenant-id")
      {
        Description = "",
        Arity = ArgumentArity.ExactlyOne,
      };

      var userSecretsIdArg = new Option<string>("user-secrets-id")
      {
        Description = "",
        Arity = ArgumentArity.ExactlyOne,
      };

      var endpointArg = new Option<string>("endpoint")
      {
        Description = "",
        Arity = ArgumentArity.ExactlyOne,
      };

      var connectionStringArg = new Option<string>("connection-string")
      {
        Description = "",
        Arity = ArgumentArity.ZeroOrOne,
      };

      var useCredentialsArg = new Option<bool>("use-credentials")
      {
        Description = "",
        Arity = ArgumentArity.ZeroOrOne,
      };

      cmd.Add(tenantIdArg);
      cmd.Add(userSecretsIdArg);
      cmd.Add(endpointArg);
      cmd.Add(connectionStringArg);
      cmd.Add(useCredentialsArg);

      cmd.SetHandler(async (context) =>
      {
        string tenantId = context.ParseResult.GetValueForOption(tenantIdArg);
        string userSecretsId = context.ParseResult.GetValueForOption(userSecretsIdArg);
        string endpoint = context.ParseResult.GetValueForOption(endpointArg);
        string connectionString = context.ParseResult.GetValueForOption(connectionStringArg);
        bool useCredentials = context.ParseResult.GetValueForOption(useCredentialsArg);

        await export(tenantId, userSecretsId, endpoint, connectionString, useCredentials, (ICompositeConsole)context.Console, context).ConfigureAwait(false);
      });

      return cmd;
    }
  }
}