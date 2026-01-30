using Strategico.Inventory.Api.Services;

namespace Strategico.Inventory.Api.Middleware;

public class WarehouseMiddleware
{
    private readonly RequestDelegate _next;

    public WarehouseMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IWarehouseProvider warehouseProvider)
    {
        // Only apply to POST /api/orders
        if (context.Request.Method == HttpMethods.Post &&
            context.Request.Path.StartsWithSegments("/api/v1/orders"))
        {
            var header = context.Request.Headers["X-Warehouse-Id"].FirstOrDefault();

            if (!Guid.TryParse(header, out var wid))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Missing or invalid X-Warehouse-Id header");
                return;
            }

            warehouseProvider.WarehouseId = wid;
        }

        await _next(context);
    }
}
