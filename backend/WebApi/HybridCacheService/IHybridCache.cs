namespace WebApi.HybridCacheService
{
  
    public interface IHybridCache 
    {
        Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan cacheDuration);
    }

}
