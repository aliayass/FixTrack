using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FixTrack.Domain.Entities;

public class Customer
{
    public Guid CustomerId { get; private set; } = Guid.NewGuid();
    public required string FullName { get; set; }
    public required string PhoneNumber { get; set; }
    public string? Email { get; set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public ICollection<ServiceOrder> ServiceOrders { get; set; } = new List<ServiceOrder>();

}
