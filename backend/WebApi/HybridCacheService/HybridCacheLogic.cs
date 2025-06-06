using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace WebApi.HybridCacheService
{
    public class HybridCacheLogic : IHybridCache
    {
        //Because its a combination of in-memory caching and distributed redis caching.
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _redisCache;

        public HybridCacheLogic(IMemoryCache memoryCache, IDistributedCache redisCache)
        {
            _memoryCache = memoryCache;
            _redisCache = redisCache;
        }

        public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan cacheDuration)
        {
            // Retriving from Memory Cache
            if (_memoryCache.TryGetValue(key, out T? memoryValue))
            {
                return memoryValue;
            }

            // Here i'm retriving from Redis Cache
            var redisValue = await _redisCache.GetStringAsync(key);
            if (!string.IsNullOrEmpty(redisValue))
            {
                var redisData = JsonSerializer.Deserialize<T>(redisValue);
               
                // Store it in memory cache for faster access next time
                _memoryCache.Set(key, redisData, cacheDuration);
                return redisData;
            }

            //Here i'm getting from DB and API Call
            var data = await factory();

            if (data != null)
            {
                // Store in Redis
                var serialized = JsonSerializer.Serialize(data);
                await _redisCache.SetStringAsync(key, serialized, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = cacheDuration
                });

                // Store in Memory
                _memoryCache.Set(key, data, cacheDuration);
            }

            return data;
        }



    }
}
