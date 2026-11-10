
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FixTrack.Domain.Entities;

namespace FixTrack.Application.Interfaces.Repositories;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(
        Guid customerId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Customer>> GetAllAsync(
        CancellationToken cancellationToken = default);

    void Add(Customer customer);

    void Remove(Customer customer);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
