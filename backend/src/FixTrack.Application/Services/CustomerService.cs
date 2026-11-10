
using FixTrack.Application.DTOs.Customers;
using FixTrack.Application.Interfaces.Repositories;
using FixTrack.Application.Interfaces.Services;
using FixTrack.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace FixTrack.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(
        ICustomerRepository customerRepository,
        ILogger<CustomerService> logger)
    {
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task<CustomerResponse> CreateAsync(
        CreateCustomerRequest request,
        CancellationToken cancellationToken = default)
    {
        var customer = new Customer
        {
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email
        };

        _customerRepository.Add(customer);

        await _customerRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Customer {CustomerId} created successfully.",
            customer.CustomerId);

        return MapToResponse(customer);
    }

    public async Task<IReadOnlyList<CustomerResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var customers = await _customerRepository.GetAllAsync(
            cancellationToken);

        _logger.LogDebug(
            "Retrieved {CustomerCount} customers.",
            customers.Count);

        return customers.Select(MapToResponse).ToList();
    }

    public async Task<CustomerResponse?> GetByIdAsync(
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(
            customerId,
            cancellationToken);

        if (customer is null)
        {
            _logger.LogDebug(
                "Customer {CustomerId} was not found.",
                customerId);

            return null;
        }

        return MapToResponse(customer);
    }

    public async Task<CustomerResponse?> UpdateAsync(
        Guid customerId,
        UpdateCustomerRequest request,
        CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(
            customerId,
            cancellationToken);

        if (customer is null)
        {
            _logger.LogDebug(
                "Customer {CustomerId} could not be updated because it was not found.",
                customerId);

            return null;
        }

        customer.FullName = request.FullName;
        customer.PhoneNumber = request.PhoneNumber;
        customer.Email = request.Email;

        await _customerRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Customer {CustomerId} updated successfully.",
            customerId);

        return MapToResponse(customer);
    }

    public async Task<bool> DeleteAsync(
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(
            customerId,
            cancellationToken);

        if (customer is null)
        {
            _logger.LogDebug(
                "Customer {CustomerId} could not be deleted because it was not found.",
                customerId);

            return false;
        }

        _customerRepository.Remove(customer);

        await _customerRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Customer {CustomerId} deleted successfully.",
            customerId);

        return true;
    }

    private static CustomerResponse MapToResponse(Customer customer)
    {
        return new CustomerResponse
        {
            CustomerId = customer.CustomerId,
            FullName = customer.FullName,
            PhoneNumber = customer.PhoneNumber,
            Email = customer.Email,
            CreatedAt = customer.CreatedAt
        };
    }
}
