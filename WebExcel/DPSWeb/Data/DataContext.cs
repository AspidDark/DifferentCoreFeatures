using Microsoft.EntityFrameworkCore;

namespace DPSWeb.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
         : base(options)
        {
        }

        public DbSet<SupplementaryAgreement> SupplementaryAgreements { get; set; }

        public DbSet<InterestCharge> InterestCharges { get; set; }
    }
}

