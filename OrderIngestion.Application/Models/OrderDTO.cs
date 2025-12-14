using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderIngestion.Application.Models
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = null!;
        public Guid RequestId { get; set; }
        public DateTime CreatedAt { get; set; }
        public CustomerDTO Customer { get; set; } = null!;
        public List<OrderItemDTO> Items { get; set; } = new();
    }
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}
