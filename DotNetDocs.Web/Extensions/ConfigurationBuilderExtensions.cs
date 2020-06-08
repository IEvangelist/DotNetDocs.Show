using System;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;

namespace DotNetDocs.Web
{
    public static class ConfigurationBuilderExtensions
    {
        const string Vault = "ASPNETCORE_HOSTINGSTARTUP__KEYVAULT__CONFIGURATIONVAULT";

        public static IConfigurationBuilder ConfigureKeyVault(this IConfigurationBuilder config) =>
            config.AddAzureKeyVault(
                Environment.GetEnvironmentVariable(Vault),
                new KeyVaultClient(
                    new KeyVaultClient.AuthenticationCallback(
                    new AzureServiceTokenProvider().KeyVaultTokenCallback)),
                new DefaultKeyVaultSecretManager());
    }
}
