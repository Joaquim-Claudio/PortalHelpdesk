using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Extensions.Options;
using PortalHelpdesk.Configurations;

public class GraphClientFactory
{
    private readonly MicrosoftGraphConfig _config;

    public GraphClientFactory(IOptions<MicrosoftGraphConfig> options)
    {
        _config = options.Value;
    }

    public GraphServiceClient Create()
    {
        var credential = new ClientSecretCredential(
            tenantId: _config.TenantId,
            clientId: _config.ClientId,
            clientSecret: _config.ClientSecret
        );

        return new GraphServiceClient(credential);
    }
}
