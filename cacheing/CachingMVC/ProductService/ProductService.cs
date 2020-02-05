using CachingMVC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CachingMVC.Services
{
    public class ProductService
    {
        private MobileContext db;
        private IMemoryCache cache;
        public ProductService(MobileContext context, IMemoryCache memoryCache)
        {
            db = context;
            cache = memoryCache;
        }

        public void Initialize()
        {
            if (!db.Products.Any())
            {
                db.Products.AddRange(
                    new Product { Name = "iPhone 8", Company = "Apple", Price = 600 },
                    new Product { Name = "Galaxy S9", Company = "Samsung", Price = 550 },
                    new Product { Name = "Pixel 2", Company = "Google", Price = 500 }
                );
                db.SaveChanges();
            }
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await db.Products.ToListAsync();
        }

        public void AddProduct(Product product)
        {
            db.Products.Add(product);
            int n = db.SaveChanges();
            if (n > 0)
            {
                cache.Set(product.Id, product, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });
            }
        }

        public async Task<Product> GetProduct(int id)
        {
            Product product = null;
            if (!cache.TryGetValue(id, out product))
            {
                product = await db.Products.FirstOrDefaultAsync(p => p.Id == id);
                if (product != null)
                {
                    cache.Set(product.Id, product,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                }
            }
            return product;
        }
    }
}