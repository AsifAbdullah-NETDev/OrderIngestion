using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderIngestion.Application.Interfaces;
using OrderIngestion.Infrastructure.Data;
using OrderIngestion.Infrastructure.Gateways;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderIngestion.Infrastructure.Extensions
{
    public static class InfrastructureRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<DapperContext>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<ILogisticsGateway, LogisticsGateway>();
            return services;
        }
    }
}
