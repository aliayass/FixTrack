using FixTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FixTrack.Infrastructure.Persistence.Configurations;

public class ServiceOrderConfiguration : IEntityTypeConfiguration<ServiceOrder>
{
    public void Configure(EntityTypeBuilder<ServiceOrder> builder)
    {
        builder.HasKey(x => x.ServiceOrderId);

        builder.Property(x => x.ServiceNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(x => x.ServiceNumber)
            .IsUnique();

        builder.Property(x => x.DeviceType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Brand)
            .IsRequired()
            .HasMaxLength(80);

        builder.Property(x => x.Model)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.SerialNumber)
            .HasMaxLength(100);

        builder.Property(x => x.PhysicalCondition)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.CustomerComplaint)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.ReceivedAccessories)
            .HasMaxLength(500);

        builder.Property(x => x.Diagnosis)
            .HasMaxLength(2000);

        builder.Property(x => x.EstimatedCost)
            .HasPrecision(18, 2);

        builder.HasOne(x => x.Customer)
            .WithMany(x => x.ServiceOrders)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}