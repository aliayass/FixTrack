using FixTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FixTrack.Infrastructure.Persistence.Configurations;

public class ServiceStatusHistoryConfiguration
    : IEntityTypeConfiguration<ServiceStatusHistory>
{
    public void Configure(EntityTypeBuilder<ServiceStatusHistory> builder)
    {
        builder.HasKey(x => x.HistoryId);

        builder.HasOne(x => x.ServiceOrder)
            .WithMany(x => x.StatusHistories)
            .HasForeignKey(x => x.ServiceOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Note)
            .HasMaxLength(1000);
    }
}