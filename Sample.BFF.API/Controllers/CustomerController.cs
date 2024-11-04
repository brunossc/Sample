using Microsoft.AspNetCore.Mvc;
using Sample.BFF.API.Services;
using static Sample.SideCar.Dtos.CustomersDtos;

namespace Sample.BFF.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerServiceClient _customerServiceClient;

        public CustomerController(CustomerServiceClient customerServiceClient)
        {
            _customerServiceClient = customerServiceClient;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer(CreateCustomerDto customerDto)
        {
            var customer = await _customerServiceClient.CreateCustomer(customerDto);
            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer(string id)
        {
            var customer = await _customerServiceClient.GetCustomer(id);
            if (customer is null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(string id, UpdateCustomerDto customerDto)
        {
            if (id != customerDto.Id)
            {
                return BadRequest();
            }

            var updatedCustomer = await _customerServiceClient.UpdateCustomer(id, customerDto);
            return Ok(updatedCustomer);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(string id)
        {
            var success = await _customerServiceClient.DeleteCustomer(id);
            return success ? NoContent() : NotFound();
        }
    }
}
