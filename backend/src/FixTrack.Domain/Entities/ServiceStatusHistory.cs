using FixTrack.Domain.Enums;

namespace FixTrack.Domain.Entities;

public class ServiceStatusHistory
{
    public Guid HistoryId { get; private set; } = Guid.NewGuid();

    public required Guid ServiceOrderId { get; init; }
    public ServiceOrder ServiceOrder { get; set; } = null!;

    public ServiceStatus? PreviousStatus { get; init; }

    public required ServiceStatus NewStatus { get; init; }

    public required Guid ChangedByUserId { get; init; }

    public DateTime ChangedAt { get; private set; } = DateTime.UtcNow;

    public string? Note { get; init; }
}