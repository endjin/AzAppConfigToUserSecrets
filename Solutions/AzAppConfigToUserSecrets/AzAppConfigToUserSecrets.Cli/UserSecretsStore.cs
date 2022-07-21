namespace AzAppConfigToUserSecrets.Cli;

using System.Text.Json.Nodes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;

public class UserSecretsStore
{
  private IDictionary<string, string> secrets = new Dictionary<string, string>();
  private readonly string userSecretsId;

  public UserSecretsStore(string userSecretsId)
  {
    this.userSecretsId = userSecretsId;
  }

  public void Set(string key, string value) => this.secrets[key] = value;

  public string this[string key] => this.secrets[key];

  public void Save()
  {
    var secretsFilePath = PathHelper.GetSecretsPathFromSecretsId(this.userSecretsId);
    JsonObject jsonObject = new JsonObject();

    foreach (KeyValuePair<string, string> keyValuePair in secrets.AsEnumerable())
    {
      jsonObject[keyValuePair.Key] = keyValuePair.Value;
    }

    File.WriteAllText(secretsFilePath, jsonObject.ToString());
  }

  public IDictionary<string, string> Load()
  {
    var secretsFilePath = PathHelper.GetSecretsPathFromSecretsId(this.userSecretsId);

    this.secrets = new ConfigurationBuilder()
      .AddJsonFile(secretsFilePath, true)
      .Build()
      .AsEnumerable()
      .Where((Func<KeyValuePair<string, string>, bool>) (i => i.Value != null))
      .ToDictionary(
        i => i.Key,
        i => i.Value,
        StringComparer.OrdinalIgnoreCase);

    return this.secrets;
  }
}