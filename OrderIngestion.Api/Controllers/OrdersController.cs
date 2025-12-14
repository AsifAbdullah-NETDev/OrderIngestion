using Microsoft.AspNetCore.Mvc;
using OrderIngestion.Application.Enums;
using OrderIngestion.Application.Models;
using OrderIngestion.Application.Services;
using OrderIngestion.Application.Validators;

namespace OrderIngestion.Api.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _service;

        public OrdersController(OrderService service)
        {
            _service = service;
        }

        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequest request)
        {
            var validation = OrderValidator.Validate(request);

            if (!validation.IsValid)
                return BadRequest(validation);

            var result = await _service.InsertOrderAsync(request);

            return result.Status switch
            {
                //InsertResult.Duplicate => Conflict(new { message = "Duplicate RequestId detected" }),
                InsertResult.Duplicate => Conflict(new { message = result.StatusMessage }),
                InsertResult.Error => StatusCode(500, new { message = "Unexpected server error" }),
                _ => Created("", new { orderId = result.OrderId })
            };
        }
        [HttpGet("GetOrders")]
        public async Task<IActionResult> GetOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? orderNumber = null, [FromQuery] string? customerEmail = null
        )
        {
            var result = await _service.GetOrdersWithItemsAsync(page, pageSize, orderNumber, customerEmail);
            return Ok(result);
        }
    }
}
