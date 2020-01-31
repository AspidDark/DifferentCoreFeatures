using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EFDataAccess.Models
{
    public class Customer
    {
        [Key]   
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public byte[] Photo { get; set; }

        // Заказанные покупателем товары
        // public List<Product> Products { get; set; }
        public List<CustomerProduct> CustomerProducts { get; set; }
    }
}
