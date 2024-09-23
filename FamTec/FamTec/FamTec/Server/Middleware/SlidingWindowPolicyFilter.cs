using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
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
                // 요청이 제한에 걸린 경우
                context.HttpContext.Response.StatusCode = 429; // Too Many Requests
                await context.HttpContext.Response.WriteAsync("Rate limit exceeded. Please try again later.");
                return;
            }

            // 요청이 허용된 경우 다음 작업을 계속 진행합니다.
            await next();
        }
    }
}
