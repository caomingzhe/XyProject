using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CTCache.interfaces
{
    /// <summary>
    /// 缓存方法
    /// </summary>
    public interface ICacheTarget
    {
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <returns></returns>
        bool Add(string key, object value);

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expiresSliding">过期时间</param>
        /// <returns></returns>
        bool Add(string key, object value, TimeSpan expires);

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        bool Remove(string key);

        /// <summary>
        /// 修改缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">新的缓存Value</param>
        /// <returns></returns>
        bool Replace(string key, object value);

        /// <summary>
        /// 修改缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">新的缓存Value</param>
        /// <param name="expiresSliding">过期时长</param>
        /// <returns></returns>
        bool Replace(string key, object value, TimeSpan expires);

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        object Get(string key);

        /// <summary>
        /// 创建事务
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        ITransaction CreateTransaction();

    }
}
