using System.IO;
using System.Linq;
using System.Reflection;
using ABSUploadClient.HealthCheks;
using ABSUploadClient.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using UploadClient.Installers;

namespace ABSUploadClient
{
	public class Startup
	{
		private IConfiguration configuration;

		public void ConfigureServices(IServiceCollection services)
		{
			this.configuration = new ConfigurationBuilder()
				 .SetBasePath(Directory.GetCurrentDirectory())
				 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				 .Build();

			services.AddSingleton(this.configuration);

			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie(options => options.LoginPath = "/Account/Login");

			var authorizationConfig = this.configuration
				.GetSection("Authorization")
				.Get<AuthorizationConfig>();

			// по-умолчанию  [Authorize] = [Authorize(Roles = "Role1,Role2,...")],
			// где Role1,Role2 = authorizationConfig.AllowedRoles 
			services.AddAuthorization(options => options.DefaultPolicy =
				new AuthorizationPolicy(
					new[] { new RolesAuthorizationRequirement(authorizationConfig.AllowedRoles) },
					new[] { CookieAuthenticationDefaults.AuthenticationScheme }
				));

			services.InstallServicesInAssemblies(this.configuration);

			services.AddRazorPages()
				.AddRazorPagesOptions(options =>
				{
					options.Conventions.AuthorizeFolder("/");
					options.Conventions.AllowAnonymousToPage("/Account/Login");
				});
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
				app.UseDeveloperExceptionPage();
			else
			{
				app.UseExceptionHandler("/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseHealthChecks("/health", new HealthCheckOptions
			{
				ResponseWriter = async (context, report) =>
				{
					context.Response.ContentType = "application/json";

					var response = new HealthCheckResponse
					{
						Status = report.Status.ToString(),
						Checks = report.Entries.Select(x => new HealthCheck
						{
							Component = x.Key,
							Status = x.Value.Status.ToString(),
							Description = x.Value.Description
						}),
						Duration = report.TotalDuration,
						Version = GetAssemblyVersion()
					};
					await context.Response.WriteAsync(JsonConvert.SerializeObject(response).Replace("\"Checks\":", "\"Checks\":\r\n").Replace("},{", "},\r\n{").Replace("}],", "}],\r\n")).ConfigureAwait(false);
				}
			});

			app.UseEndpoints(endpoints => endpoints.MapRazorPages());
		}

		public static string GetAssemblyVersion()
		{
			return Assembly
				.GetExecutingAssembly()
				.GetName()
				.Version
				.ToString();
		}
	}
}