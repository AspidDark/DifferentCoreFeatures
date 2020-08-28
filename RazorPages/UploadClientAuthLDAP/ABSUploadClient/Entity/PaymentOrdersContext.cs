using ABSUploadClient.Entity.EntityModel;
using Microsoft.EntityFrameworkCore;

namespace ABSUploadClient.Entity
{
	public class PaymentOrdersContext : DbContext
	{
		public PaymentOrdersContext(DbContextOptions options) : base(options) { }

		public DbSet<PaymentOrderEntity> PaymentOrders { get; set; }

		public DbSet<PaymentBinding> PaymentBindings { get; set; }

		public DbSet<ModuleBrief> ModuleBriefs { get; set; }

		public DbSet<UploadedDocument> UploadedDocuments { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<ModuleBrief>().HasData(new ModuleBrief { Id = 1, Accout = "40702810010310008965", ModuleValue = "VTBO" });
			modelBuilder.Entity<ModuleBrief>().HasData(new ModuleBrief { Id = 2, Accout = "40702810210310028965", ModuleValue = "VTBR" });
			modelBuilder.Entity<ModuleBrief>().HasData(new ModuleBrief { Id = 3, Accout = "40702810469000031377", ModuleValue = "SBRF" });
			modelBuilder.Entity<ModuleBrief>().HasData(new ModuleBrief { Id = 4, Accout = "40702810310310000427", ModuleValue = "VTBFK" });
			modelBuilder.Entity<ModuleBrief>().HasData(new ModuleBrief { Id = 5, Accout = "40701810710310008965", ModuleValue = "VTBR2" });
		}
	}
}
