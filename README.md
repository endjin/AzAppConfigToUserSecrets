# AzAppConfigToUserSecrets

A dotnet global tool for extracting settings from Azure App Configuration Service and stashing them as User Secrets.

## Installing the tool

Use the following command to install the .NET Global Tool:

`dotnet tool install --global AzAppConfigToUserSecrets.Cli`

To invoke the tool you need to use the name `actus` (**A**pp**C**onfig**T**o**U**ser**S**ecrets)

You can uninstall the tool with the following command:

`dotnet tool uninstall --global AzAppConfigToUserSecrets.Cli`

## Usage

`actus export --tenant-id <TENANT-ID> --user-secrets-id <USER-SECRETS-ID> --endpoint "https://<YOURAPP>.azconfig.io"`

This will spawn an interactive authentication flow.

## Notes

You can configure your application to use User Secrets by right clicking on the project in Visual Studio 2022 Solution Explorer and selecting the "Manage User Secrets" option from the context menu.

User Secrets are stored in `%APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json` on Windows and `~/.microsoft/usersecrets/<user_secrets_id>/secrets.json` on Linux/macOS.

Note: User Secrets does not encrypt the stored secrets and shouldn't be treated as a trusted store. They are stored in plain text in one of the above locations.

## Licenses

[![GitHub license](https://img.shields.io/badge/License-Apache%202-blue.svg)](https://raw.githubusercontent.com/endjin/AzAppConfigToUserSecrets/blob/main/LICENSE)

AzAppConfigToUserSecrets is available under the Apache 2.0 open source license.

For any licensing questions, please email [&#108;&#105;&#99;&#101;&#110;&#115;&#105;&#110;&#103;&#64;&#101;&#110;&#100;&#106;&#105;&#110;&#46;&#99;&#111;&#109;](&#109;&#97;&#105;&#108;&#116;&#111;&#58;&#108;&#105;&#99;&#101;&#110;&#115;&#105;&#110;&#103;&#64;&#101;&#110;&#100;&#106;&#105;&#110;&#46;&#99;&#111;&#109;)

## Project Sponsor

This project is sponsored by [endjin](https://endjin.com), a UK based Microsoft Gold Partner for Cloud Platform, Data Platform, Data Analytics, DevOps, and a Power BI Partner.

For more information about our products and services, or for commercial support of this project, please [contact us](https://endjin.com/contact-us). 

We produce two free weekly newsletters; [Azure Weekly](https://azureweekly.info) for all things about the Microsoft Azure Platform, and [Power BI Weekly](https://powerbiweekly.info).

Keep up with everything that's going on at endjin via our [blog](https://endjin.com/blog), follow us on [Twitter](https://twitter.com/endjin), or [LinkedIn](https://www.linkedin.com/company/1671851/).

Our other Open Source projects can be found on [GitHub](https://github.com/endjin)

## Code of conduct

This project has adopted a code of conduct adapted from the [Contributor Covenant](http://contributor-covenant.org/) to clarify expected behavior in our community. This code of conduct has been [adopted by many other projects](http://contributor-covenant.org/adopters/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [&#104;&#101;&#108;&#108;&#111;&#064;&#101;&#110;&#100;&#106;&#105;&#110;&#046;&#099;&#111;&#109;](&#109;&#097;&#105;&#108;&#116;&#111;:&#104;&#101;&#108;&#108;&#111;&#064;&#101;&#110;&#100;&#106;&#105;&#110;&#046;&#099;&#111;&#109;) with any additional questions or comments.