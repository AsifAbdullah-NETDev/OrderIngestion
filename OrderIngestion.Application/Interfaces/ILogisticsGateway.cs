using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderIngestion.Application.Interfaces
{
    public interface ILogisticsGateway
    {
        Task NotifyAsync(string orderNumber);
    }
}
