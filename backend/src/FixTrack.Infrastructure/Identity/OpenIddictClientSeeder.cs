using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace FixTrack.Infrastructure.Identity;

public static class OpenIddictClientSeeder
{

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var manager = scope.ServiceProvider
            .GetRequiredService<IOpenIddictApplicationManager>();

        const string clientId = "fixtrack-postman";

        var descriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = clientId,
            DisplayName = "FixTrack Postman Development Client",
            ClientType = ClientTypes.Public,
            ConsentType = ConsentTypes.Implicit,

            RedirectUris =
        {
            new Uri("https://oauth.pstmn.io/v1/browser-callback")
        },

            Permissions =
        {
            Permissions.Endpoints.Authorization,
            Permissions.Endpoints.Token,

            Permissions.GrantTypes.AuthorizationCode,
            Permissions.GrantTypes.RefreshToken,

            Permissions.ResponseTypes.Code,

            Permissions.Scopes.Profile,
            Permissions.Scopes.Roles
        },

            Requirements =
        {
            Requirements.Features.ProofKeyForCodeExchange
        }
        };

        var existingApplication =
            await manager.FindByClientIdAsync(clientId);

        if (existingApplication is null)
        {
            await manager.CreateAsync(descriptor);
        }
        else
        {
            await manager.UpdateAsync(existingApplication, descriptor);
        }
    }

}