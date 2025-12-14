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

        public async Task<(InsertResult Status, int OrderId, string StatusMessage)> InsertOrderAsync(OrderRequest request)
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
                p.Add("@RequestId", Guid.Parse(request.RequestId));
                //p.Add("@RequestId", request.RequestId);
                p.Add("@OrderNumber", request.OrderNumber);
                p.Add("@CustomerName", request.Customer.Name);
                p.Add("@CustomerEmail", request.Customer.Email);
                p.Add("@Items", table.AsTableValuedParameter("OrderItemType"));

                p.Add("@OrderId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                p.Add("@StatusCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
                p.Add("@StatusMessage", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);


                var result = await conn.ExecuteAsync(
                    "SP_InsertOrderWithItems",
                    p,
                    commandType: CommandType.StoredProcedure
                );

                var statusCode = p.Get<int>("@StatusCode");
                var orderId = p.Get<int?>("@OrderId") ?? 0;
                var statusMessage = p.Get<string>("@StatusMessage");

                if (statusCode == -1)
                    return (InsertResult.Duplicate, 0, statusMessage);

                if (statusCode != 0)
                    return (InsertResult.Error, 0, statusMessage);

                return (InsertResult.Success, orderId, statusMessage);
            }
            catch (Exception ex)
            {
                return (InsertResult.Error, 0, ex.Message);
            }
        }

        public async Task<PagedResult<OrderDTO>> GetOrdersWithItemsAsync(
            int page,
            int pageSize,
            string? orderNumber = null,
            string? customerEmail = null
        )
        {
            using var conn = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Page", page);
            parameters.Add("@PageSize", pageSize);
            parameters.Add("@OrderNumber", orderNumber);
            parameters.Add("@CustomerEmail", customerEmail);

            using var multi = await conn.QueryMultipleAsync(
                "SP_GetOrdersWithItems",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var items = (await multi.ReadAsync<OrderItemDTO>()).ToList();

            var orders = (await multi.ReadAsync<OrderDTO>()).ToList();

            var totalCount = await multi.ReadSingleAsync<int>();

            // Map items to orders
            var orderDict = orders.ToDictionary(o => o.OrderId);
            foreach (var item in items)
            {
                if (orderDict.TryGetValue(item.OrderId, out var order))
                    order.Items.Add(item);
            }

            return new PagedResult<OrderDTO>
            {
                Items = orders,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

    }
}
