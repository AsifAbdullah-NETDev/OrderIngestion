using OrderIngestion.Application.Enums;
using OrderIngestion.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderIngestion.Application.Interfaces
{
    public interface IOrderRepository
    {
        Task<(InsertResult Status, int OrderId)> InsertOrderAsync(OrderRequest request);
    }
}
