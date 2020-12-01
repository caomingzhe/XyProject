using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using XyProject.Contracts;

namespace XyProject.Contracts
{
    public interface IMemoryCacheService
    {
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
        bool SetCache<T>(string key, T value, DateTime? absoluteExpiration = default, TimeSpan? absoluteExpirationRelativeToNow = default, TimeSpan? slidingExpiration = default, List<IChangeToken> changeTokens = default, List<PostEvictionDelegate> postEviction = default, CacheItemPriority Priority = CacheItemPriority.Normal, long? size = default);

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        T GetCache<T>(string key);

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
        T GetOrSetCache<T>(string key, T value, DateTime? absoluteExpiration = default, TimeSpan? absoluteExpirationRelativeToNow = default, TimeSpan? slidingExpiration = default, List<IChangeToken> changeTokens = default, List<PostEvictionDelegate> postEviction = default, CacheItemPriority Priority = CacheItemPriority.Normal, long? size = default);

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool RemoveCache(string key);
    }
}
