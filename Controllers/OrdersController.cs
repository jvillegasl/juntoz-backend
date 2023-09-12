using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderStoreApi.Models;
using OrderStoreApi.Services;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrdersService _ordersService;

    public OrdersController(OrdersService ordersService)
    {
        _ordersService = ordersService;
    }

    [HttpGet]
    public async Task<List<Order>> Get() => await _ordersService.GetAsync();

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Order>> Get(string id)
    {
        var order = await _ordersService.GetAsync(id);

        if (order is null)
        {
            return NotFound();
        }

        return order;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] NewOrder newOrder)
    {
        int id = await _ordersService.CreateAsync(newOrder);

        return CreatedAtAction(nameof(Get), new { id }, newOrder);
    }
}