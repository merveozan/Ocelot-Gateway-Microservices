using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerWebApi2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private static List<Customer> customers = new List<Customer>
        {
            new Customer { CustomerId = 1, CustomerName = "Charlie", MobileNumber = "654-321", Email = "charlie@example.com" },
            new Customer { CustomerId = 2, CustomerName = "Dave", MobileNumber = "210-987", Email = "dave@example.com" }
        };

        private static int nextId = 3;

        // Get all customers
        [HttpGet]
        //[Authorize(Roles = "Administrator")]
        public IEnumerable<Customer> GetCustomers()
        {
            return customers;
        }

        // Get customer by ID
        [HttpGet("{id:int}")]
        //[Authorize(Roles = "User")]
        public IActionResult GetById(int id)
        {
            var customer = customers.FirstOrDefault(c => c.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }

        // Add a new customer
        [HttpPost]
        //[Authorize(Roles = "Administrator")]
        public IActionResult Create([FromBody] Customer customer)
        {
            customer.CustomerId = nextId++;
            customers.Add(customer);
            return CreatedAtAction(nameof(GetById), new { id = customer.CustomerId }, customer);
        }

        // Update an existing customer 
        [HttpPut]
        //[Authorize(Roles = "Administrator,User")]
        public IActionResult Update([FromBody] Customer updatedCustomer)
        {
            var customer = customers.FirstOrDefault(c => c.CustomerId == updatedCustomer.CustomerId);
            if (customer == null)
            {
                return NotFound("Customer not found");
            }

            customer.CustomerName = updatedCustomer.CustomerName;
            customer.MobileNumber = updatedCustomer.MobileNumber;
            customer.Email = updatedCustomer.Email;

            return Ok("Customer updated successfully from CustomerWebApi2");
        }

        // Delete a customer (returning success message)
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var customer = customers.FirstOrDefault(c => c.CustomerId == id);
            if (customer == null)
            {
                return NotFound("Customer not found");
            }

            customers.Remove(customer);
            return Ok("Customer deleted successfully from CustomerWebApi2");
        }
    }

    public class Customer
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
    }
}
