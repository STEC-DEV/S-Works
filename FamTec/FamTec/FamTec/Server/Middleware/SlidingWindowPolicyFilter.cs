using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.RateLimiting;

namespace FamTec.Server.Middleware
{
    public class SlidingWindowPolicyFilter : IAsyncActionFilter
    {
        private readonly PartitionedRateLimiter<HttpContext> _rateLimiter;

        public SlidingWindowPolicyFilter(PartitionedRateLimiter<HttpContext> rateLimiter)
        {
            _rateLimiter = rateLimiter;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var lease = await _rateLimiter.AcquireAsync(context.HttpContext);

            if (!lease.IsAcquired)
            {
                context.HttpContext.Response.StatusCode = 429; // Too Many Requests
                await context.HttpContext.Response.WriteAsync("Rate limit exceeded. Please try again later.");
                return;
            }

            await next();
        }
    }
}
