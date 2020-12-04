using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CTCache.interfaces
{
    public interface IRedisHelper
    {
        #region  string 操作
        /// <summary>
        /// 保存单个key value
        /// </summary>
        /// <param name="value">保存的值</param>
        /// <param name="expiry">过期时间</param>
        bool SetStringKey(string key, string value, TimeSpan? expiry = default(TimeSpan?));

        /// <summary>
        /// 保存多个key value（事务）
        /// </summary>
        /// <param name="value">保存的值</param>
        /// <param name="expiry">过期时间</param>
        bool SetStringKeys(Dictionary<string,string> values, TimeSpan? expiry = default(TimeSpan?));

        /// <summary>
        /// 修改单个key value
        /// </summary>
        /// <param name="value">保存的值</param>
        /// <param name="expiry">过期时间</param>
        bool EditStringKey(string key, string value, TimeSpan? expiry = default(TimeSpan?));

        /// <summary>
        /// 获取单个key的值
        /// </summary>
        RedisValue GetStringKey(string key);

        /// <summary>
        /// 获取一个key的对象
        /// </summary>
        T GetStringKey<T>(string key) where T : class;

        /// <summary>
        /// 保存一个对象
        /// </summary>
        /// <param name="obj"></param>
        bool SetStringKey<T>(string key, T obj, TimeSpan? expiry = default(TimeSpan?)) where T : class;

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="obj"></param>
        bool DeleteKey(string key);

        #endregion

        #region list 操作

        /// <summary>
        ///  在列表头部插入值。如果键不存在，先创建再插入值
        /// </summary>
        /// <param name="value">保存的值</param>
        /// <param name="expiry">过期时间</param>
        long ListLeftPush(string redisKey, string value, TimeSpan? expiry = default(TimeSpan?));

        /// <summary>
        ///  在列表尾部插入值。如果键不存在，先创建再插入值
        /// </summary>
        /// <param name="value">保存的值</param>
        /// <param name="expiry">过期时间</param>
        long ListRightPush(string redisKey, string value, TimeSpan? expiry = default(TimeSpan?));


        /// <summary>
        ///  根据下标修改值
        /// </summary>
        /// <param name="value">保存的值</param>
        /// <param name="expiry">过期时间</param>
        void ListSetByIndex(string redisKey, string value,long index, TimeSpan? expiry = default(TimeSpan?));

        /// <summary>
        /// 列表长度
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        long ListLength(string redisKey);

        /// <summary>
        /// 查询列表数据
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        IEnumerable<string> ListRange(string redisKey);

        /// <summary>
        /// 根据索引获取指定位置数据
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        IEnumerable<string> ListRange(string redisKey, int start, int stop);

        /// <summary>
        /// 删除List中的元素 并返回删除的个数
        /// </summary>
        /// <param name="redisKey">key</param>
        /// <param name="redisValue">元素</param>
        /// <param name="type">大于零 : 从表头开始向表尾搜索，小于零 : 从表尾开始向表头搜索，等于零：移除表中所有与 VALUE 相等的值</param>
        /// <param name="db"></param>
        /// <returns></returns>
        long ListDelRange(string redisKey, string redisValue, long type = 0);

        /// <summary>
        /// 清空List
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="db"></param>
        void ListClear(string redisKey);


        #endregion

        #region hash 操作

        /// <summary>
        /// 保存(修改)多个对象集合  非序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">key</param>
        /// <param name="list">数据集合</param>
        /// <param name="getModelId">字段key</param>
        void HashSet<T>(string redisKey, IEnumerable<T> list, Func<T, string> getModelId, TimeSpan? span = null) where T : class;

        /// <summary>
        ///  根据hashkey获取所有的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        List<T> HashGetAll<T>(string redisKey) where T : class;

        /// <summary>
        /// 从 hash 中移除多个指定字段
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="hashField"></param>
        /// <returns></returns>
        long HashDelete(string redisKey, IEnumerable<RedisValue> hashField);

        /// <summary>
        /// 删除 hash值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="hashField"></param>
        /// <returns></returns>
        long HashDelete(string redisKey);

        #endregion

        #region sets 操作

        /// <summary>
        /// set添加多个对象集合 
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="value"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        long SetAddList<T>(string redisKey, IEnumerable<T> list, TimeSpan? span = null) where T : class;

        /// <summary>
        /// 根据key获取所有Set对象集合 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="db"></param>
        /// <returns></returns>
         List<T> GetAllMembers<T>(string redisKey) where T : class;

        /// <summary>
        /// 根据value删除Set对象集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        long SetRemove(string redisKey, IEnumerable<RedisValue> hashField);

        #endregion

        #region  Sorted Sets 操作
        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="member"></param>
        /// <param name="score"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        bool ZSortadd(string key, string member, double score);

        /// <summary>
        /// 移除某个元素
        /// </summary>
        /// <param name="key"></param>
        /// <param name="memebr"></param>
        /// <returns></returns>
        bool ZSortedRemove(string key, string memebr);

        /// <summary>
        /// 获取集合的所有值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="memebr"></param>
        /// <returns></returns>
        SortedSetEntry[] ZSortedRemoveAll(string key);

        #endregion

        #region 事务 操作
        /// <summary>
        /// 创建事务
        /// </summary>
        /// <returns></returns>
        ITransaction CreateTransaction();
        #endregion
    }
}
