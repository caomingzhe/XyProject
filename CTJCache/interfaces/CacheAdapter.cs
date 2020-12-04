using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CTCache.interfaces
{
    /// <summary>
    /// 缓存适配器
    /// </summary>
   public class CacheAdapter: ICacheTarget
    {

        private readonly IRedisHelper _redisHelper;

        private readonly IMemoryCacheHelper _memoryCacheHelper;

        public CacheAdapter(IRedisHelper redisHelper, IMemoryCacheHelper memoryCacheHelper)
        {
            _redisHelper = redisHelper;
            _memoryCacheHelper = memoryCacheHelper;
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <returns></returns>
        public  bool Add(string key, object value)
        {
          return  _redisHelper.SetStringKey(key,value);
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expiresSliding">过期时间</param>
        /// <returns></returns>
        public  bool Add(string key, object value, TimeSpan expires)
        {
            return _redisHelper.SetStringKey(key, value, expires);
          
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public  bool Remove(string key)
        {
            return _redisHelper.DeleteKey(key);
        }


        /// <summary>
        /// 修改缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">新的缓存Value</param>
        /// <returns></returns>
        public  bool Replace(string key, object value)
        {
            return _redisHelper.EditStringKey(key,value.ToString());
        }


        /// <summary>
        /// 修改缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">新的缓存Value</param>
        /// <param name="expiresSliding">过期时长</param>
        /// <returns></returns>
        public  bool Replace(string key, object value, TimeSpan expires)
        {
            return _redisHelper.EditStringKey(key, value.ToString(),expires);
        }


        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public  object Get(string key)
        {
          return  _redisHelper.GetStringKey(key);
        }


        /// <summary>
        /// 创建事务
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public ITransaction CreateTransaction()
        {
            return _redisHelper.CreateTransaction();
        }

    }
}
