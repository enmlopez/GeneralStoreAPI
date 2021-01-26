using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GeneralStoreAPI.Models
{
    public class Product
    {
        [Key]
        public string SKU { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public double Cost { get; set; }

        [Required]
        public int NumberInInventory { get; set; }

        public bool IsInStock
        {
            get
            {
                bool isInStock = true;
                if(NumberInInventory >= 0)
                {
                    return isInStock;
                }
                return false;
            }
        }
        //public virtual List<Transaction> Transactions { get; set; } //displays a history of transactions for a specific customer
    }
}