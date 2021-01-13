using Microsoft.EntityFrameworkCore;
using System.Text;

namespace DPSService.DB
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
          : base(options)
        {
        }

        public DbSet<GraceContact> GraceContacts { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<ContractData> ContractData { get; set; }

        public DbSet<CalculationResult> CalculationResults { get; set; }

        public DbSet<SupplementaryAgreement> SupplementaryAgreements { get; set; }

        public DbSet<InterestCharge> InterestCharges { get; set; }

        public DbSet<IntegrationEvent> IntegrationEvents { get; set; }
    }
}
