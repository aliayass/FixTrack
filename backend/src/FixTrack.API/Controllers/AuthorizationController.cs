
using FixTrack.Infrastructure.Identity;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace FixTrack.API.Controllers;

[AllowAnonymous]
public class AuthorizationController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthorizationController(
        UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet("~/connect/authorize")]
    [HttpPost("~/connect/authorize")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Authorize()
    {
        var request = HttpContext.GetOpenIddictServerRequest()
            ?? throw new InvalidOperationException(
                "OpenIddict authorization request bulunamadı.");

        // Kullanıcının Identity cookie oturumunu kontrol et.
        var result = await HttpContext.AuthenticateAsync(
            IdentityConstants.ApplicationScheme);

        // Kullanıcı giriş yapmamışsa Login sayfasına yönlendir.
        if (!result.Succeeded || result.Principal is null)
        {
            // prompt=none geldiyse kullanıcı etkileşimi isteyemeyiz.
            if (request.HasPromptValue(PromptValues.None))
            {
                return Forbid(
                    authenticationSchemes:
                        OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(
                        new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error]
                                = Errors.LoginRequired,

                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription]
                                = "Kullanıcı girişi gerekli."
                        }));
            }

            // OAuth isteğinin bütün parametrelerini koru.
            var parameters = Request.HasFormContentType
                ? Request.Form.ToList()
                : Request.Query.ToList();

            return Challenge(
    authenticationSchemes: IdentityConstants.ApplicationScheme,
    properties: new AuthenticationProperties
    {
        RedirectUri = Request.PathBase
            + Request.Path
            + QueryString.Create(
                Request.HasFormContentType
                    ? Request.Form.ToList()
                    : Request.Query.ToList())
    });
        }

        // Cookie üzerinden giriş yapmış kullanıcıyı bul.
        var user = await _userManager.GetUserAsync(result.Principal)
            ?? throw new InvalidOperationException(
                "Oturum açmış kullanıcı bulunamadı.");

        // OpenIddict'in kullanacağı token kimliğini oluştur.
        var identity = new ClaimsIdentity(
            authenticationType:
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
            nameType: Claims.Name,
            roleType: Claims.Role);

        identity.SetClaim(
            Claims.Subject,
            await _userManager.GetUserIdAsync(user));

        identity.SetClaim(
            Claims.Name,
            await _userManager.GetUserNameAsync(user));

        identity.SetClaims(
    Claims.Role,
    [.. await _userManager.GetRolesAsync(user)]);

        // İstemcinin istediği scope'ları principal'a ekle.
        identity.SetScopes(request.GetScopes());

        // Hangi claim'lerin hangi token içerisinde bulunacağını belirle.
        identity.SetDestinations(GetDestinations);

        // Authorization Code üretilmesini OpenIddict'e devret.
        return SignIn(
            new ClaimsPrincipal(identity),
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private static IEnumerable<string> GetDestinations(Claim claim)
    {
        switch (claim.Type)
        {
            case Claims.Subject:
                yield return Destinations.AccessToken;
                yield return Destinations.IdentityToken;
                yield break;

            case Claims.Name:
                yield return Destinations.AccessToken;

                if (claim.Subject!.HasScope(Scopes.Profile))
                {
                    yield return Destinations.IdentityToken;
                }

                yield break;

            case Claims.Role:
                yield return Destinations.AccessToken;

                if (claim.Subject!.HasScope(Scopes.Roles))
                {
                    yield return Destinations.IdentityToken;
                }

                yield break;

            default:
                yield break;
        }
    }
}
