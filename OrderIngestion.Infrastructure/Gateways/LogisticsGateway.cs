using OrderIngestion.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderIngestion.Infrastructure.Gateways
{
    public class LogisticsGateway : ILogisticsGateway
    {
        public async Task NotifyAsync(string orderNumber)
        {
            await Task.Delay(2000);
            Console.WriteLine($"Logistics notified for {orderNumber}");
        }
    }
}
