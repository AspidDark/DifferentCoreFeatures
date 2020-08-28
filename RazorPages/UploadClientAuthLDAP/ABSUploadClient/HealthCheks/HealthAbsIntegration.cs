using ABSService;
using ABSUploadClient.Options;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace ABSUploadClient.HealthCheks
{
    public class HealthAbsIntegration : IHealthCheck
    {
		private readonly AbsOptions _absOptions;
		public HealthAbsIntegration(AbsOptions absOptions)
		{
			_absOptions = absOptions;
		}
		public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
			try
			{
				var client = GetServiceClient();

				List<string> creditContractNumbers = new List<string>
			{
				"TestNumerForCheck"
			};
				var requests = creditContractNumbers
					.Select(async x =>
					{
						var par = new DsLoanBrowseListByParamReq { Number = x };
						var res = await client.dsLoanBrowseListByParamAsync(par);
						return (Result: res.DsLoanBrowseListByParamRes, Number: x);
					});
				var responses = await Task.WhenAll(requests);
				await client.CloseAsync();
				if (!string.IsNullOrWhiteSpace(responses.FirstOrDefault().Result.Status))
				{
					return HealthCheckResult.Healthy();
				}
				return HealthCheckResult.Unhealthy();
			}
			catch (Exception ex)
			{
				return HealthCheckResult.Unhealthy(ex.Message);
			}

		}

		private LOANCREDITWSPORTTYPEClient GetServiceClient()
		{
			var client = new LOANCREDITWSPORTTYPEClient();
			client.Endpoint.Address = new EndpointAddress(this._absOptions.Path);
			client.ClientCredentials.UserName.UserName = this._absOptions.UserName;
			client.ClientCredentials.UserName.Password = this._absOptions.Password;
			client.Endpoint.Binding.SendTimeout =
				TimeSpan.FromSeconds(this._absOptions.SendTimeout);
			return client;
		}
	}
}
