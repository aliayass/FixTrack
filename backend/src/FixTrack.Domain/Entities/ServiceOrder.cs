using FixTrack.Domain.Enums;

namespace FixTrack.Domain.Entities;

public class ServiceOrder
{
    public Guid ServiceOrderId { get; private set; } = Guid.NewGuid();

    public required string ServiceNumber { get; set; }

    public required Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public required Guid CreatedByUserId { get; set; }

    public required string DeviceType { get; set; }

    public required string Brand { get; set; }

    public required string Model { get; set; }

    public string? SerialNumber { get; set; }

    public required string PhysicalCondition { get; set; }

    public required string CustomerComplaint { get; set; }

    public string? ReceivedAccessories { get; set; }

    public string? Diagnosis { get; set; }

    public decimal? EstimatedCost { get; set; }

    public ApprovalDecision ApprovalDecision { get; set; }
        = ApprovalDecision.NotRequested;

    public ServiceStatus Status { get; set; }
        = ServiceStatus.Received;

    public DateTime ReceivedAt { get; private set; }
        = DateTime.UtcNow;

    public DateTime? ExpectedCompletionAt { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public ICollection<ServiceOrderAssignment> Assignments { get; set; }
    = new List<ServiceOrderAssignment>();
    public ICollection<ServiceStatusHistory> StatusHistories { get; set; }
    = new List<ServiceStatusHistory>();
}