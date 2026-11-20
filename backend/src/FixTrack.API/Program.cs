using FixTrack.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using FixTrack.Application.Interfaces.Repositories;
using FixTrack.Application.Interfaces.Services;
using FixTrack.Application.Services;
using FixTrack.Infrastructure.Persistence;
using FixTrack.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Controller kayıtları
builder.Services.AddControllers();

// Razor Pages kayıtları
builder.Services.AddRazorPages();

// Swagger kayıtları
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database bağlantısı
var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString);
    options.UseOpenIddict<Guid>();
});
// OpenIddict yapılandırması
builder.Services.AddOpenIddict()

    // OpenIddict veritabanı altyapısı
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
            .UseDbContext<AppDbContext>()
            .ReplaceDefaultEntities<Guid>();
    })

    // OpenIddict Authorization Server
    .AddServer(options =>
    {
        // Endpoint adresleri
        options.SetAuthorizationEndpointUris("/connect/authorize");
        options.SetTokenEndpointUris("/connect/token");

        // Desteklenen OAuth akışları
        options.AllowAuthorizationCodeFlow();
        options.AllowRefreshTokenFlow();

        // Authorization Code akışında PKCE zorunlu
        options.RequireProofKeyForCodeExchange();

        // Desteklenen scope'lar
        options.RegisterScopes(
            "openid",
            "profile",
            "roles",
            "offline_access");

        // Token süreleri
        options.SetAccessTokenLifetime(TimeSpan.FromMinutes(15));
        options.SetRefreshTokenLifetime(TimeSpan.FromDays(14));

        // Geliştirme ortamı için sertifikalar
        options.AddDevelopmentEncryptionCertificate();
        options.AddDevelopmentSigningCertificate();

        // ASP.NET Core entegrasyonu
        options.UseAspNetCore()
            .EnableAuthorizationEndpointPassthrough();
    })
    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    }); ;

builder.Services.AddDataProtection();

// Identity yapılandırması
builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    // Kullanıcı ayarları
    options.User.RequireUniqueEmail = true;

    // Parola politikası
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;

    // Hesap kilitleme politikası
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
.AddRoles<IdentityRole<Guid>>()
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders()
.AddSignInManager();

builder.Services
    .AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddIdentityCookies();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";

    options.Cookie.HttpOnly = true;
    options.Cookie.Name = "FixTrack.Auth";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
});

builder.Services.AddAuthorization();

// Dependency Injection kayıtları
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

var app = builder.Build();

// Middleware


if (app.Environment.IsDevelopment())
{
    await IdentityRoleSeeder.SeedAsync(app.Services);

    await BootstrapAdminSeeder.SeedAsync(
       app.Services,
       app.Configuration);

    await OpenIddictClientSeeder.SeedAsync(app.Services);

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapRazorPages();

app.Run();