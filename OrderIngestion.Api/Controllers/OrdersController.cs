using Microsoft.AspNetCore.Mvc;
using OrderIngestion.Application.Enums;
using OrderIngestion.Application.Models;
using OrderIngestion.Application.Services;
using OrderIngestion.Application.Validators;

namespace OrderIngestion.Api.Controllers
{
    [ApiController]
    [Route("api/v1/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _service;

        public OrdersController(OrderService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequest request)
        {
            var validation = OrderValidator.Validate(request);

            if (!validation.IsValid)
                return BadRequest(validation);

            var result = await _service.InsertOrderAsync(request);

            return result.Status switch
            {
                InsertResult.Duplicate => Conflict(new { message = "Duplicate RequestId detected" }),
                InsertResult.Error => StatusCode(500, new { message = "Unexpected server error" }),
                _ => Created("", new { orderId = result.OrderId })
            };
        }
    }
}
