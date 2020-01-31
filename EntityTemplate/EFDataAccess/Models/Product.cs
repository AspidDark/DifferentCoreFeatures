using System;
using System.Collections.Generic;
using System.Text;

namespace EFDataAccess.Models
{
    public class Product
    {
        //add repository  https://www.youtube.com/watch?v=LDRxo6wDIE0&t=8s
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }

        // Список покупателей, заказавших этот товар
        // public List<Customer> Customers { get; set; }
        public List<CustomerProduct> CustomerProducts { get; set; }
    }
}
