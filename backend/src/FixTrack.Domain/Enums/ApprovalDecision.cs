using System;
using System.Collections.Generic;
using System.Text;

namespace FixTrack.Domain.Enums;

public enum ApprovalDecision
{
    NotRequested,
    NotRequired,
    Pending,
    Approved,
    Rejected
}
