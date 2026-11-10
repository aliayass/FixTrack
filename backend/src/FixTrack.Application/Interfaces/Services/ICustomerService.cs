
using FixTrack.Application.DTOs.Customers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FixTrack.Application.Interfaces.Services;

public interface ICustomerService
{
    Task<CustomerResponse> CreateAsync(
        CreateCustomerRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CustomerResponse>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<CustomerResponse?> GetByIdAsync(
        Guid customerId,
        CancellationToken cancellationToken = default);

    Task<CustomerResponse?> UpdateAsync(
        Guid customerId,
        UpdateCustomerRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        Guid customerId,
        CancellationToken cancellationToken = default);
}
