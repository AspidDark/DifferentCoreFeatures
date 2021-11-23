using Microsoft.EntityFrameworkCore;

namespace Ef6NewFeatures
{
    public class Customer
    {
        public int Id { get; set; }
        
        [Precision(16,2)]
        public decimal Hzhzhz { get; set; }
    }
}
