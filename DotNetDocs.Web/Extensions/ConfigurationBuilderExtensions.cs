using System;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;

namespace DotNetDocs.Web.Extensions
{
    public static class ConfigurationBuilderExtensions
    {
        const string Enabled = "ASPNETCORE_HOSTINGSTARTUP__KEYVAULT__CONFIGURATIONENABLED";
        const string Vault = "ASPNETCORE_HOSTINGSTARTUP__KEYVAULT__CONFIGURATIONVAULT";

        public static void ConfigureKeyVault(this IConfigurationBuilder config)
        {
            if (bool.TryParse(Environment.GetEnvironmentVariable(Enabled),
                out bool isKeyVaultEnabled) && isKeyVaultEnabled)
            {
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var keyVaultClient = new KeyVaultClient(
                    new KeyVaultClient.AuthenticationCallback(
                        azureServiceTokenProvider.KeyVaultTokenCallback));

                var keyVaultEndpoint = Environment.GetEnvironmentVariable(Vault);
                config.AddAzureKeyVault(
                    keyVaultEndpoint, keyVaultClient, new DefaultKeyVaultSecretManager());
            }
        }
    }
}
