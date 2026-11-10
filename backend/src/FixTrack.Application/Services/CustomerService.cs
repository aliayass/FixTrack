
using FixTrack.Application.DTOs.Customers;
using FixTrack.Application.Interfaces.Repositories;
using FixTrack.Application.Interfaces.Services;
using FixTrack.Domain.Entities;

namespace FixTrack.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
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

        return MapToResponse(customer);
    }

    public async Task<IReadOnlyList<CustomerResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var customers = await _customerRepository.GetAllAsync(cancellationToken);

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
            return null;

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
            return null;

        customer.FullName = request.FullName;
        customer.PhoneNumber = request.PhoneNumber;
        customer.Email = request.Email;

        await _customerRepository.SaveChangesAsync(cancellationToken);

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
            return false;

        _customerRepository.Remove(customer);

        await _customerRepository.SaveChangesAsync(cancellationToken);

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
