using System;
using System.Collections.Generic;
using System.Text;

namespace FixTrack.Domain.Entities;

public class ServiceOrderAssignment
{
    public Guid AssignmentId { get; private set; } = Guid.NewGuid();
    public required Guid ServiceOrderId { get; set; }
    public ServiceOrder ServiceOrder { get; set; } = null!;

    public required Guid TechnicianId { get; set; }
    public required Guid AssignedByUserId { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UnassignedAt { get; set; }


}
