using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderIngestion.Application.Models
{
    public class OrderRequest
    {
        public string RequestId { get; set; }
        public string OrderNumber { get; set; }
        public CustomerDto Customer { get; set; }
        public List<OrderItemDto> Items { get; set; }
    }
}
