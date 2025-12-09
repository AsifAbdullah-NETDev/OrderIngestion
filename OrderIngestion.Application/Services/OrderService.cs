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

    public async Task<(InsertResult Status, int OrderId)> InsertOrderAsync(OrderRequest request)
    {
        var result = await _repo.InsertOrderAsync(request);

        if (result.Status == InsertResult.Success)
            _ = _logistics.NotifyAsync(request.OrderNumber);

        return result;
    }
}
