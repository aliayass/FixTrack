namespace FixTrack.Application.DTOs.Customers;

public class CustomerResponse
{
    public Guid CustomerId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string? Email { get; set; }

    public DateTime CreatedAt { get; set; }
}