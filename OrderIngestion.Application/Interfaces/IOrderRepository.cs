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
        Task<(InsertResult Status, int OrderId, string StatusMessage)> InsertOrderAsync(OrderRequest request);
        Task<PagedResult<OrderDTO>> GetOrdersWithItemsAsync(int page, int pageSize, string? orderNumber, string? customerEmail);
    }
}
