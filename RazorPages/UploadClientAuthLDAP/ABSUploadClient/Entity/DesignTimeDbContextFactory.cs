using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ABSUploadClient.Entity
{
	class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PaymentOrdersContext>
	{
		public PaymentOrdersContext CreateDbContext(string[] args)
		{
			IConfiguration config = new ConfigurationBuilder()
			 .SetBasePath(Directory.GetCurrentDirectory())
			 .AddJsonFile("appsettings.json")
			 .Build();

			string connString = config
				.GetConnectionString("DefaultConnection");

			var options = new DbContextOptionsBuilder()
				.UseSqlServer(connString)
				.Options;

			return new PaymentOrdersContext(options);
		}
	}
}