using FixTrack.Application.DTOs.Customers;
using FixTrack.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace FixTrack.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(ICustomerService customerService, ILogger<CustomersController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<CustomerResponse>>> GetAll(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Received request to list customers.");

            var customers = await _customerService.GetAllAsync(cancellationToken);

            return Ok(customers);
        }

        [HttpGet("{customerId:guid}")]
        public async Task<ActionResult<CustomerResponse>> GetById(Guid customerId, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Received request to retrieve customer {CustomerId}.", customerId);

            var customer = await _customerService.GetByIdAsync(customerId, cancellationToken);

            if (customer is null)
            {
                _logger.LogWarning("Customer {CustomerId} not found.", customerId);
                return NotFound();
            }

            return Ok(customer);
        }

        [HttpPost]
        public async Task<ActionResult<CustomerResponse>> Create(CreateCustomerRequest request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Received request to create a new customer.");

            var createdCustomer = await _customerService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { customerId = createdCustomer.CustomerId }, createdCustomer);
        }

        [HttpPut("{customerId:guid}")]
        public async Task<ActionResult<CustomerResponse>> Update(
            Guid customerId,
            UpdateCustomerRequest request,
            CancellationToken cancellationToken)
        {
            _logger.LogDebug(
                "Received request to update customer {CustomerId}.",
                customerId);

            var customer = await _customerService.UpdateAsync(
                customerId,
                request,
                cancellationToken);

            if (customer is null)
            {
                _logger.LogWarning(
                    "Customer {CustomerId} not found for update.",
                    customerId);

                return NotFound();
            }

            return Ok(customer);
        }

        [HttpDelete("{customerId:guid}")]
        public async Task<IActionResult> Delete(Guid customerId, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Received request to delete customer {CustomerId}.", customerId);
            var deleted = await _customerService.DeleteAsync(customerId, cancellationToken);
            if (!deleted)
            {
                _logger.LogWarning("Customer {CustomerId} not found for deletion.", customerId);
                return NotFound();
            }
            return NoContent();
        }
    }
}
