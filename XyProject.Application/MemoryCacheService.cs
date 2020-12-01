using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using XyProject.Contracts;

namespace XyProject.Application
{
    public class MemoryCacheService : IMemoryCacheService
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="absoluteExpiration">绝对过期时间</param>
        /// <param name="absoluteExpirationRelativeToNow">相对于当前时间的绝对到期时间</param>
        /// <param name="slidingExpiration">滑动过期时间</param>
        /// <param name="postEviction">回调列表</param>
        /// <returns></returns>
        public bool SetCache<T>(string key, T value, DateTime? absoluteExpiration = default, TimeSpan? absoluteExpirationRelativeToNow = default, TimeSpan? slidingExpiration = default, List<IChangeToken> changeTokens = default, List<PostEvictionDelegate> postEviction = default, CacheItemPriority Priority = CacheItemPriority.Normal, long? size = default)
        {
            try
            {
                MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
                options.AbsoluteExpiration = absoluteExpiration;
                options.AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
                options.SlidingExpiration = slidingExpiration;
                options.Priority = Priority;
                options.Size = size;

                if (changeTokens != null)
                {
                    foreach (var item in changeTokens)
                    {
                        options.ExpirationTokens.Add(item);
                    }
                }

                if (postEviction != null)
                {
                    foreach (var item in postEviction)
                    {
                        options.PostEvictionCallbacks.Add(new PostEvictionCallbackRegistration() { EvictionCallback = item });
                    }
                }

                _memoryCache.Set(key, value, options);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        public T GetCache<T>(string key)
        {
            bool exists = _memoryCache.TryGetValue<T>(key, out T value);
            if (exists)
            {
                return value;
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// 获取缓存，若不存在则添加
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="absoluteExpiration">绝对过期时间</param>
        /// <param name="absoluteExpirationRelativeToNow">相对于当前时间的绝对到期时间</param>
        /// <param name="slidingExpiration">滑动过期时间</param>
        /// <param name="postEviction">回调列表</param>
        /// <returns></returns>
        public T GetOrSetCache<T>(string key, T value, DateTime? absoluteExpiration = default, TimeSpan? absoluteExpirationRelativeToNow = default, TimeSpan? slidingExpiration = default, List<IChangeToken> changeTokens = default, List<PostEvictionDelegate> postEviction = default, CacheItemPriority Priority = CacheItemPriority.Normal, long? size = default)
        {
            T cacheValue = default;

            cacheValue = _memoryCache.GetOrCreate<T>(key, (m) =>
            {
                m.AbsoluteExpiration = absoluteExpiration;
                m.AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow;
                m.SlidingExpiration = slidingExpiration;
                m.Priority = Priority;
                m.Size = size;

                if (changeTokens != null)
                {
                    foreach (var item in changeTokens)
                    {
                        m.ExpirationTokens.Add(item);
                    }
                }

                if (postEviction != null)
                {
                    foreach (var item in postEviction)
                    {
                        m.PostEvictionCallbacks.Add(new PostEvictionCallbackRegistration() { EvictionCallback = item });
                    }
                }
                m.Value = value;

                return value;
            });

            return cacheValue;
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool RemoveCache(string key)
        {
            try
            {
                _memoryCache.Remove(key);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
