using Microsoft.AspNetCore.Mvc;

namespace OrderWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        // In-memory list to store orders
        private static List<Order> orders = new List<Order>
        {
            new Order
            {
                OrderId = 1,
                CustomerId = 1,
                OrderedOn = DateTime.UtcNow.AddDays(-1),
                OrderDetails = new List<OrderDetail>
                {
                    new OrderDetail { ProductId = 101, Quantity = 2, UnitPrice = 50.00M },
                    new OrderDetail { ProductId = 102, Quantity = 1, UnitPrice = 100.00M }
                }
            },
            new Order
            {
                OrderId = 2,
                CustomerId = 2,
                OrderedOn = DateTime.UtcNow,
                OrderDetails = new List<OrderDetail>
                {
                    new OrderDetail { ProductId = 103, Quantity = 1, UnitPrice = 75.00M }
                }
            }
        };

        // To track the next available OrderId
        private static int nextOrderId = 3;

        // Get all orders
        [HttpGet]
        public IEnumerable<Order> GetOrders()
        {
            return orders;
        }

        // Get order by ID
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
            {
            var order = orders.FirstOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                return NotFound("Order not found");
            }
            return Ok(order);
        }

        // Add a new order
        [HttpPost]
        public IActionResult Create([FromBody] Order newOrder)
        {
            newOrder.OrderId = nextOrderId++; // Automatically set the next OrderId
            newOrder.OrderedOn = DateTime.UtcNow; // Automatically set the order date
            orders.Add(newOrder);
            return CreatedAtAction(nameof(GetById), new { id = newOrder.OrderId }, newOrder);
        }

        // Update an existing order
        [HttpPut]
        public IActionResult Update([FromBody] Order updatedOrder)
        {
            var order = orders.FirstOrDefault(o => o.OrderId == updatedOrder.OrderId);
            if (order == null)
            {
                return NotFound("Order not found");
            }

            // Update the order fields
            order.CustomerId = updatedOrder.CustomerId;
            order.OrderedOn = updatedOrder.OrderedOn;
            order.OrderDetails = updatedOrder.OrderDetails;

            return Ok("Order updated successfully");
        }

        // Delete an order
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var order = orders.FirstOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                return NotFound("Order not found");
            }

            orders.Remove(order);
            return Ok("Order deleted successfully");
        }
    }

    // Order class model
    public class Order
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderedOn { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }

    // OrderDetail class model
    public class OrderDetail
    {
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
