
using FixTrack.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FixTrack.API.Pages.Account;

public class LoginModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public LoginModel(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    [BindProperty]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    public bool RememberMe { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
    }


    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(ReturnUrl) ||
            !Url.IsLocalUrl(ReturnUrl))
        {
            ErrorMessage = "Geçersiz giriş isteği.";
            return Page();
        }

        var result = await _signInManager.PasswordSignInAsync(
            Email,
            Password,
            RememberMe,
            lockoutOnFailure: true);

        if (result.Succeeded)
        {
            return LocalRedirect(ReturnUrl);
        }

        if (result.IsLockedOut)
        {
            ErrorMessage =
                "Hesabınız çok fazla başarısız giriş nedeniyle geçici olarak kilitlendi.";
        }
        else
        {
            ErrorMessage = "E-posta adresi veya şifre hatalı.";
        }

        return Page();
    }


    public async Task<IActionResult> OnPostLogoutAsync()
    {
        await _signInManager.SignOutAsync();

        return RedirectToPage("/Account/Login");
    }
}