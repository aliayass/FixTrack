using System.ComponentModel.DataAnnotations;

namespace FixTrack.Application.DTOs.Customers;

public class CreateCustomerRequest
{
    [Required]
    [MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [EmailAddress]
    [MaxLength(254)]
    public string? Email { get; set; }
}