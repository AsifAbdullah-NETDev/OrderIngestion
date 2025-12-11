using Dapper;
using OrderIngestion.Application.Enums;
using OrderIngestion.Application.Interfaces;
using OrderIngestion.Application.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderIngestion.Infrastructure.Data
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DapperContext _context;

        public OrderRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<(InsertResult Status, int OrderId)> InsertOrderAsync(OrderRequest request)
        {
            try
            {
                using var conn = _context.CreateConnection();

                var table = new DataTable();
                table.Columns.Add("SKU");
                table.Columns.Add("Quantity");
                table.Columns.Add("Price");

                foreach (var i in request.Items)
                    table.Rows.Add(i.Sku, i.Quantity, i.Price);

                var p = new DynamicParameters();
                //p.Add("@RequestId", Guid.Parse(request.RequestId));
                p.Add("@RequestId", request.RequestId);
                p.Add("@OrderNumber", request.OrderNumber);
                p.Add("@CustomerName", request.Customer.Name);
                p.Add("@CustomerEmail", request.Customer.Email);
                p.Add("@Items", table.AsTableValuedParameter("OrderItemType"));

                var result = await conn.ExecuteScalarAsync<int>(
                    "InsertOrderWithItems",
                    p,
                    commandType: CommandType.StoredProcedure
                );

                if (result == -1)
                    return (InsertResult.Duplicate, 0);

                return (InsertResult.Success, result);
            }
            catch(Exception ex)
            {
                return (InsertResult.Error, 0);
            }
        }
    }
}
