using Microsoft.Extensions.Caching.Memory;

namespace Vayosoft.Caching
{
    public interface IDistributedMemoryCache : IMemoryCache
    {
        MemoryCacheEntryOptions GetDefaultCacheEntryOptions();
    }
}
