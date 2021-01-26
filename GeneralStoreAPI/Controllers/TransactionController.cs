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
    public class TransactionController : ApiController
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        [HttpPost]
        public async Task<IHttpActionResult> CreateTransaction([FromBody] Transaction model)
        {
            Product product = await _context.Products.FindAsync(model.ProductSKU);
            Customer customer = await _context.Customers.FindAsync(model.CustomerId);

            if (model is null)
            {
                return BadRequest("Your request body cannot be empty");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (model.CustomerId != customer.Id)
            {
                return BadRequest("Customer Id does not match in database.");
            }
            if (model.ProductSKU != product.SKU)
            {
                return BadRequest("SKU does not match in database.");

            }
            if (model.ItemCount > product.NumberInInventory)
            {
                return BadRequest("Not enough inventory.");
            }
            int newItemCount = product.NumberInInventory - model.ItemCount;
            product.NumberInInventory = newItemCount;

            model.DateOfTransaction = DateTime.UtcNow.Date;

            _context.Transactions.Add(model);

            await _context.SaveChangesAsync();

            return Ok("Transaction was successfully created");
            //    //verify product is in stock
            //    //check that there is enough to complete the transaction
            //    //remove products that were bought
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetAll()
        {
            List<Transaction> transactionList = await _context.Transactions.ToListAsync();
            //if (transactionList is null)
            //{
            //    return BadRequest("No Transactions to erase.");
            //}
            return Ok(transactionList);
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetTransactionByCustomerId([FromBody] int customerId, [FromUri] int id)
        {
            //Should save transaction in costumer POCO? in order to find customer and also bring up transactions?
            //If I know transaction id, why also need customerId? makes method pointless

            Transaction transaction = await _context.Transactions.FindAsync(id);
            //Customer customer = await _context.Customers.FindAsync(customerId);
            if (customerId != transaction.CustomerId)
            {
                return BadRequest("Customer ID does not match in database");
            }
            if (transaction != null)
            {
                return Ok(transaction);
            }
            return NotFound();
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetTransactionById([FromUri] int transactionId)
        {
            Transaction transaction = await _context.Transactions.FindAsync(transactionId);
            if (transaction != null)
            {
                return Ok(transaction);
            }
            return NotFound();
        }
        [HttpPut]
        public async Task<IHttpActionResult> UpdateTransaction([FromUri] int id, [FromBody] Transaction updatedTransaction/*[FromBody]int updatedNumberInventory*/)
        {
            Transaction transaction = await _context.Transactions.FindAsync(id);
            Product product = await _context.Products.FindAsync(transaction.ProductSKU);

            if (id != transaction.IdK)
            {
                return BadRequest("Transaction ID not found in database.");
            }
            if (transaction is null)
            {
                return NotFound();
            }
            product.NumberInInventory += transaction.ItemCount;

            Product productToUpdate = await _context.Products.FindAsync(updatedTransaction.ProductSKU);

            if (updatedTransaction.ItemCount > productToUpdate.NumberInInventory)
            {
                return BadRequest("Not enough inventory to update database");
            }
            productToUpdate.NumberInInventory = productToUpdate.NumberInInventory - updatedTransaction.ItemCount;
            transaction.ItemCount = updatedTransaction.ItemCount;
            transaction.ProductSKU = updatedTransaction.ProductSKU;
            transaction.CustomerId = updatedTransaction.CustomerId;

            await _context.SaveChangesAsync();

            return Ok($"Transacation ID: {id} was updated");
        }
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteTransaction([FromUri] int id)
        {
            Transaction transaction = await _context.Transactions.FindAsync(id);
            Product updatedProduct = await _context.Products.FindAsync(transaction.ProductSKU);
            if (transaction is null)
            {
                return NotFound();
            }
            updatedProduct.NumberInInventory += transaction.ItemCount; //number in inventory = number in inventory + itemcount.

            _context.Transactions.Remove(transaction);

            //if(await _context.SaveChangesAsync() == 1)
            //{
            //    return Ok("Transaction was deleted");
            //}
            //return InternalServerError();
            await _context.SaveChangesAsync();
            return Ok("Transaction was deleted");
        }
    }
}
