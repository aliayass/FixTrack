using FixTrack.Domain.Entities;
using FixTrack.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FixTrack.Infrastructure.Persistence;

public class AppDbContext
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<ServiceOrder> ServiceOrders
        => Set<ServiceOrder>();

    public DbSet<ServiceOrderAssignment> ServiceOrderAssignments
        => Set<ServiceOrderAssignment>();

    public DbSet<ServiceStatusHistory> ServiceStatusHistories
        => Set<ServiceStatusHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);
    }
}