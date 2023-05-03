using Azure.Core;
using Azure.Identity;

namespace LargeDataSet
{
    public static class ManagedIdentityCredentialHelper
    {
        public static TokenCredential GetCredential(IWebHostEnvironment environment)
        {
            //set option to use the local identity in development
            var credentialOptions = new DefaultAzureCredentialOptions();

            credentialOptions.ExcludeEnvironmentCredential = true;
            credentialOptions.ExcludeInteractiveBrowserCredential = true;
            credentialOptions.ExcludeManagedIdentityCredential = true;
            credentialOptions.ExcludeSharedTokenCacheCredential = true;
            credentialOptions.ExcludeVisualStudioCodeCredential = true;
            credentialOptions.ExcludeVisualStudioCredential = true;
            credentialOptions.ExcludeAzureCliCredential = true;


            if (environment.IsDevelopment())
            {
                //exclude all options except AzureCliCredential - my preferred option
                //this will speed up the credential acquisition process
                credentialOptions.ExcludeAzureCliCredential = false;
            }
            else
            {
                //when running in Azure
                credentialOptions.ExcludeManagedIdentityCredential = false;
            }

            var credentials = new DefaultAzureCredential(credentialOptions);

            return credentials;

           

        }
    }
}
