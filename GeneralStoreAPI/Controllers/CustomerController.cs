using GeneralStoreAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace GeneralStoreAPI.Controllers
{
    public class CustomerController : ApiController
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        //POST
        //Task<IHttpActionResult> Returns all of different response types provided by API Controller class
        [HttpPost]
        public async Task<IHttpActionResult> CreateCustomer([FromBody] Customer model) //I'm expecting a body in my request to have a customer in it
        {

            if (model is null)
            {
                return BadRequest("Your request body cannot be empty");
            }
            if (ModelState.IsValid)
            {
                _context.Customers.Add(model);
                int changeCount = await _context.SaveChangesAsync();

                return Ok("Customer was created.");
            }
            return BadRequest(ModelState);
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetAllCustomers()
        {
            List<Customer> customers = await _context.Customers.ToListAsync();
            return Ok(customers);
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetCustomerById([FromUri] int id)
        {
            Customer customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                return Ok(customer);
            }
            return NotFound();
        }
        [HttpPut]
        public async Task<IHttpActionResult> UpdateCustomer([FromUri] int id, [FromBody] Customer updatedCustomer)
        {
            if (id != updatedCustomer?.Id) //? Check to see if updatedCustomer.Id is null, if it is it comes back as false but does not check if its null
            {
                return BadRequest("Id does not match");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Customer customer = await _context.Customers.FindAsync(id);
            if (customer is null)
            {
                return NotFound();
            }
            customer.FirstName = updatedCustomer.FirstName;
            customer.LastName = updatedCustomer.LastName;

            await _context.SaveChangesAsync();

            return Ok("The Customer was updated");
        }
        [HttpDelete]
        public async Task<IHttpActionResult> RemoveCustomer([FromUri] int id, Customer deleteCustomer)
        {
            if (id != deleteCustomer?.Id)
            {
                return BadRequest("Id does not match");
            }
            Customer customer = await _context.Customers.FindAsync(id);
            if (customer is null)
            {
                return BadRequest();
            }
            _context.Customers.Remove(customer);

            if (await _context.SaveChangesAsync() == 1)
            {
                return Ok($"The customer with ID: {customer.Id} was deleted");
            }
            return InternalServerError();
        }
    }
}
