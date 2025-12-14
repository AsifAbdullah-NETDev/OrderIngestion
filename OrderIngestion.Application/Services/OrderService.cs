using OrderIngestion.Application.Models;
using OrderIngestion.Application.Interfaces;
using OrderIngestion.Application.Enums;

namespace OrderIngestion.Application.Services;

public class OrderService
{
    private readonly IOrderRepository _repo;
    private readonly ILogisticsGateway _logistics;

    public OrderService(IOrderRepository repo, ILogisticsGateway logistics)
    {
        _repo = repo;
        _logistics = logistics;
    }

    public async Task<(InsertResult Status, int OrderId, string StatusMessage)> InsertOrderAsync(OrderRequest request)
    {
        var result = await _repo.InsertOrderAsync(request);

        if (result.Status == InsertResult.Success)
            _ = _logistics.NotifyAsync(request.OrderNumber);

        return result;
    }
    public async Task<PagedResult<OrderDTO>> GetOrdersWithItemsAsync(
        int page,
        int pageSize,
        string? orderNumber = null,
        string? customerEmail = null
    )
    {
        var result = await _repo.GetOrdersWithItemsAsync(page, pageSize, orderNumber, customerEmail);

        if (result.Items.Count > 100)
        {
            _ = Task.Run(() => Console.WriteLine("Large page retrieved."));
        }

        return result;
    }
}
