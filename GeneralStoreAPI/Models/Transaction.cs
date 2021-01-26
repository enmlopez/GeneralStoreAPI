using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GeneralStoreAPI.Models
{
    public class Transaction
    {
        [Key]
        public int IdK { get; set; }

        [ForeignKey(nameof(Customer))] //attribute
        [Required]
        public int CustomerId { get; set; } //navigational property
        public virtual Customer Customer { get; set; } //virtual property

        [ForeignKey(nameof(Product))]
        [Required]
        public string ProductSKU { get; set; }
        public virtual Product Product { get; set; }

        [Required]
        public int ItemCount { get; set; }
        public DateTime DateOfTransaction { get; set; }



    }
}