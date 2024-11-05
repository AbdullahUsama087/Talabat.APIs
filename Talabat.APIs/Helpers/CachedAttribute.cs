using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using Talabat.Core.Services;

namespace Talabat.APIs.Helpers
{
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLiveInSeconds;

        public CachedAttribute(int timeToLiveInSeconds)
        {
            _timeToLiveInSeconds = timeToLiveInSeconds;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();

            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);

            var cachedResponse = await cacheService.GetCachedResponseAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedResponse))
            {
                var contentResult = new ContentResult()
                {
                    Content = cachedResponse,
                    ContentType = "application/json",
                    StatusCode = 200
                };
                context.Result = contentResult; // Store Response Data in Context
                return;
            }

            var executedEndPointContext = await next(); // Will Execute the Endpoint

            if (executedEndPointContext.Result is OkObjectResult okObjectResult)
            {
                await cacheService.CacheResponseAsync(cacheKey, okObjectResult.Value, TimeSpan.FromSeconds(_timeToLiveInSeconds));

            }
        }

        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            // {{url}}/api/products?pageIndex=1&pageSize=5&sort=name

            var keyBuilder = new StringBuilder();

            keyBuilder.Append(request.Path);// api/products

            // pageIndex=1
            // pageSize=5
            // sort=name

            // api/products
            foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
            {
                keyBuilder.Append($"|{key}-{value}");
                // api/Products/pageIndex-1
                // api/Products/pageIndex-1|pageSize-5
                // api/Products/pageIndex-1|pageSize-5|sort-name
            }

            return keyBuilder.ToString();
        }
    }
}
