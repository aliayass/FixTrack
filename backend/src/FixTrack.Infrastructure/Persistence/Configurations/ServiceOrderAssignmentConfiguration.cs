using FixTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FixTrack.Infrastructure.Persistence.Configurations;

public class ServiceOrderAssignmentConfiguration
    : IEntityTypeConfiguration<ServiceOrderAssignment>
{
    public void Configure(EntityTypeBuilder<ServiceOrderAssignment> builder)
    {
        builder.HasKey(x => x.AssignmentId);

        builder.HasOne(x => x.ServiceOrder)
            .WithMany(x => x.Assignments)   
            .HasForeignKey(x => x.ServiceOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ServiceOrderId)
            .IsUnique()
            .HasFilter("\"UnassignedAt\" IS NULL"); 


    }
}