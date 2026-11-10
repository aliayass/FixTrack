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

// Swagger kayıtları
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database bağlantısı
var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

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
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
