using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Reflection;
using LogService.Model;
using Nest;
using Elasticsearch.Net;
using System;
using LogService.DB;
using System.Collections.Generic;
using System.Linq;

namespace LogService
{
    public class Startup
    {
        const string defaultEntities = "Operator,Notification,AccrualPeriodity,Contract,PennaltyAccrualPeriod,User,Role,UserRole,Subdivision,Permission,Contragent,Okved,ContragentOkved,ContractDebtDashboard" +
            ",Lawsuit,ContractExecution,ContractContragent,ImportLog,LandCategoryRvi,DocumentOwnership,ContractDocumentOwnership,ContractContragentPenalty,DocumentOwnershipRightholder" + 
            ",DocumentOwnershipRightholderPermission,DocumentOwnershipRosreestr,PaymentImported,Payment,InvoicePayment,Claim,LawsuitClaim,RegionPlanIndicator,BankInterestRate" +
            ",NotificationContragent,ContractPeriodSummary,ContractCharge,TaxRate,CorrectionCoefficient,MunicipalMovableEstate,MunicipalImmovableEstate,MunicipalUnitaryInformation,Address,LinkedPayment";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            string connection = Configuration.GetConnectionString("LogServiceConnection");
            var node = new Uri(connection);
            var pool = new SingleNodeConnectionPool(node);
            var settings = new ConnectionSettings(pool)
                .RequestTimeout(TimeSpan.FromMinutes(2))
#if DEBUG
                .EnableDebugMode()
#endif
                ;
            var client = new ElasticClient(settings);

            if (!client.Indices.Exists(Indices.Index("log")).Exists)
            {
                var logCreateIndexResponse = client.Indices.Create("log", c => c
                    .Map<ElasticLog>(m => m.AutoMap()));
            }

            UpdateDB(client);

            services.AddSingleton(client);
            services.AddHealthChecks().AddElasticsearch(connection);
            services.AddControllers();
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new OpenApiInfo { Title = "LogService", Version = "v1" });
            });
            services.AddScoped<IDBLogger, ElasticLogger>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Model.IDBLogger DBLogger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(s => s.SwaggerEndpoint("/swagger/v1/swagger.json", "LogService v1"));
            var healthOptions = new HealthCheckOptions();
            healthOptions.ResponseWriter = async (c, r) =>
            {
                c.Response.ContentType = "application/json";
                var result = JsonConvert.SerializeObject(new
                {
                    status = r.Status.ToString(),
                    version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion
                });
                await c.Response.WriteAsync(result);
            };
            app.UseHealthChecks("/health", healthOptions);
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        /// <summary>
        /// Автоматическое создание не существующих индексов
        /// </summary>
        /// <param name="indexNames"></param>
        private void UpdateDB(ElasticClient _client)
        {
            List<string> indexNames = defaultEntities.ToLower().Split(',').ToList();

            foreach (string indexName in indexNames)
            {
                if (!_client.Indices.Exists(Indices.Index(indexName)).Exists)
                {
                    var rawLogCreateIndexResponse = _client.Indices.Create(indexName, c => c
                        .Map<ElasticLogData<object>>(m => m.AutoMap()));

                    if (!rawLogCreateIndexResponse.IsValid)
                    {
                        //_logger.LogError($"Ошибка создания индекса {indexName}: {rawLogCreateIndexResponse.ServerError?.Error?.Reason}");
                    }
                }
            }
        }
    }
}
