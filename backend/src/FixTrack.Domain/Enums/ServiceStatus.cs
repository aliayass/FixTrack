using System;
using System.Collections.Generic;
using System.Text;

namespace FixTrack.Domain.Enums;

public enum ServiceStatus
{
    Received,
    Diagnosed,
    WaitingForApproval,
    Repairing,
    ReadyForPickup,
    Delivered,
    Canceled
}
