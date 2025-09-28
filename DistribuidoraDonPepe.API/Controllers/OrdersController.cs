using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DistribuidoraDonPepe.API.Data;
using DistribuidoraDonPepe.API.Models;

namespace DistribuidoraDonPepe.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly DistribuidoraDonPepeContext _context;

    public OrdersController(DistribuidoraDonPepeContext context)
    {
        _context = context;
    }

    // GET: api/Orders
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    // GET: api/Orders/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetOrder(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        return order;
    }

    // PUT: api/Orders/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutOrder(int id, Order order)
    {
        if (id != order.Id)
        {
            return BadRequest();
        }

        order.UpdatedDate = DateTime.UtcNow;
        _context.Entry(order).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!OrderExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Orders
    [HttpPost]
    public async Task<ActionResult<Order>> PostOrder(CreateOrderRequest request)
    {
        // Validate customer exists
        var customer = await _context.Customers.FindAsync(request.CustomerId);
        if (customer == null || !customer.IsActive)
        {
            return BadRequest("Customer not found or inactive.");
        }

        // Validate products exist and have enough stock
        var productIds = request.OrderItems.Select(oi => oi.ProductId).ToList();
        var products = await _context.Products
            .Where(p => productIds.Contains(p.Id) && p.IsActive)
            .ToListAsync();

        if (products.Count != productIds.Count)
        {
            return BadRequest("One or more products not found or inactive.");
        }

        // Check stock availability
        foreach (var item in request.OrderItems)
        {
            var product = products.First(p => p.Id == item.ProductId);
            if (product.Stock < item.Quantity)
            {
                return BadRequest($"Insufficient stock for product {product.Name}. Available: {product.Stock}, Requested: {item.Quantity}");
            }
        }

        // Create order
        var order = new Order
        {
            CustomerId = request.CustomerId,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            ShippingAddress = request.ShippingAddress,
            Notes = request.Notes,
            CreatedDate = DateTime.UtcNow
        };

        // Create order items and calculate totals
        decimal subTotal = 0;
        foreach (var item in request.OrderItems)
        {
            var product = products.First(p => p.Id == item.ProductId);
            var orderItem = new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = product.Price,
                CreatedDate = DateTime.UtcNow
            };
            
            order.OrderItems.Add(orderItem);
            subTotal += orderItem.Total;

            // Update product stock
            product.Stock -= item.Quantity;
            product.UpdatedDate = DateTime.UtcNow;
        }

        order.SubTotal = subTotal;
        order.Tax = subTotal * 0.13m; // 13% tax
        order.Total = order.SubTotal + order.Tax;

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetOrder", new { id = order.Id }, order);
    }

    // PATCH: api/Orders/5/status
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest request)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        order.Status = request.Status;
        order.UpdatedDate = DateTime.UtcNow;

        if (request.Status == OrderStatus.Shipped && !order.ShippedDate.HasValue)
        {
            order.ShippedDate = DateTime.UtcNow;
        }
        else if (request.Status == OrderStatus.Delivered && !order.DeliveredDate.HasValue)
        {
            order.DeliveredDate = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET: api/Orders/customer/5
    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<IEnumerable<Order>>> GetCustomerOrders(int customerId)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    // DELETE: api/Orders/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        // Can only delete pending orders
        if (order.Status != OrderStatus.Pending)
        {
            return BadRequest("Only pending orders can be deleted.");
        }

        // Restore product stock
        foreach (var item in order.OrderItems)
        {
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product != null)
            {
                product.Stock += item.Quantity;
                product.UpdatedDate = DateTime.UtcNow;
            }
        }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool OrderExists(int id)
    {
        return _context.Orders.Any(e => e.Id == id);
    }
}

// DTOs for order creation
public class CreateOrderRequest
{
    public int CustomerId { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public List<CreateOrderItemRequest> OrderItems { get; set; } = new List<CreateOrderItemRequest>();
}

public class CreateOrderItemRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class UpdateOrderStatusRequest
{
    public OrderStatus Status { get; set; }
}