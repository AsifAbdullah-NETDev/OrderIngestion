using Dapper;
using OrderIngestion.Application.Enums;
using OrderIngestion.Application.Interfaces;
using OrderIngestion.Application.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
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

                //var table = new DataTable();
                //table.Columns.Add("SKU");
                //table.Columns.Add("Quantity");
                //table.Columns.Add("Price");

                //foreach (var i in request.Items)
                //    table.Rows.Add(i.Sku, i.Quantity, i.Price);

                //var p = new DynamicParameters();
                //p.Add("@RequestId", Guid.Parse(request.RequestId));
                //p.Add("@OrderNumber", request.OrderNumber);
                //p.Add("@CustomerName", request.Customer.Name);
                //p.Add("@CustomerEmail", request.Customer.Email);
                //p.Add("@Items", itemsJson, DbType.String);

                //p.Add("@OrderId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                //p.Add("@StatusCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
                //p.Add("@StatusMessage", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);


                //var result = await conn.QuerySingleAsync<(int orderId, int statusCode, string statusMessage)>(
                //    "SP_InsertOrderWithItems",
                //    p,
                //    commandType: CommandType.StoredProcedure
                //);

                var itemsJson = System.Text.Json.JsonSerializer.Serialize(request.Items);

                var parameters = new
                {
                    RequestId = Guid.Parse(request.RequestId),
                    OrderNumber = request.OrderNumber,
                    CustomerName = request.Customer.Name,
                    CustomerEmail = request.Customer.Email,
                    Items = itemsJson
                };

                // Call PostgreSQL function
                var result = await conn.QuerySingleAsync<(int orderId, int statusCode, string statusMessage)>(
                    @"SELECT * FROM SP_InsertOrderWithItems(
                        @RequestId,
                        @OrderNumber,
                        @CustomerName,
                        @CustomerEmail,
                        @Items::jsonb
                    )",
                    parameters
                );

                //var statusCode = p.Get<int>("@StatusCode");
                //var orderId = p.Get<int?>("@OrderId") ?? 0;
                //var statusMessage = p.Get<string>("@StatusMessage");

                if (result.statusCode == -1)
                    return (InsertResult.Duplicate, 0, result.statusMessage);

                if (result.statusCode != 0)
                    return (InsertResult.Error, 0, result.statusMessage);

                return (InsertResult.Success, result.orderId, result.statusMessage);
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

            //var parameters = new DynamicParameters();
            //parameters.Add("@Page", page);
            //parameters.Add("@PageSize", pageSize);
            //parameters.Add("@OrderNumber", orderNumber);
            //parameters.Add("@CustomerEmail", customerEmail);

            //using var multi = await conn.QueryMultipleAsync(
            //    "SP_GetOrdersWithItems",
            //    parameters,
            //    commandType: CommandType.StoredProcedure
            //);

            var parameters = new
            {
                Page = page,
                PageSize = pageSize,
                OrderNumber = orderNumber,
                CustomerEmail = customerEmail
            };

            var jsonResult = await conn.QuerySingleAsync<string>(
                @"SELECT sp_getorderswithitems(
                    @Page,
                    @PageSize,
                    @OrderNumber,
                    @CustomerEmail
                )::text",
                parameters
            );


            using JsonDocument doc = JsonDocument.Parse(jsonResult);

            List<OrderDTO> orders = JsonSerializer.Deserialize<List<OrderDTO>>(
                doc.RootElement.GetProperty("items").GetRawText()
            ) ?? new List<OrderDTO>();

            int totalCount = doc.RootElement.GetProperty("totalCount").GetInt32();

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
