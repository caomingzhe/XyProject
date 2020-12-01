using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using XyProject.Contracts;

namespace XyProject.Application
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly ConnectionMultiplexer _conn;
        private IDatabase _db;

        #region 构造函数

        public RedisCacheService()
        {
            _conn = ConnectionMultiplexer.Connect("127.0.0.1:6379");
            _db = _conn.GetDatabase();
        }

        #endregion 构造函数

        #region String

        /// <summary>
        /// 保存单个key value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        public bool StringSet(string key, string value, TimeSpan? expiry = default)
        {
            return _db.StringSet(key, value, expiry);
        }

        /// <summary>
        /// 保存多个key value
        /// </summary>
        /// <param name="keyValues">键值对</param>
        /// <returns></returns>
        public bool StringSet(List<KeyValuePair<RedisKey, RedisValue>> keyValues)
        {
            return _db.StringSet(keyValues.ToArray());
        }

        /// <summary>
        /// 保存一个对象
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="key">Key</param>
        /// <param name="obj">Value</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        public bool StringSet<T>(string key, T obj, TimeSpan? expiry = default)
        {
            string json = JsonConvert.SerializeObject(obj);
            return _db.StringSet(key, json, expiry);
        }

        /// <summary>
        /// 获取单个key的值
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns></returns>
        public string StringGet(string key)
        {
            return _db.StringGet(key);
        }

        /// <summary>
        /// 获取多个Key
        /// </summary>
        /// <param name="listKey">Redis Key集合</param>
        /// <returns></returns>
        public RedisValue[] StringGet(List<string> listKey)
        {
            var keys = listKey.Select(redisKey => (RedisKey)redisKey).ToArray();
            return _db.StringGet(keys);
        }

        /// <summary>
        /// 获取一个key的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T StringGet<T>(string key)
        {
            return JsonConvert.DeserializeObject<T>(_db.StringGet(key));
        }

        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        public double StringIncrement(string key, double val = 1)
        {
            return _db.StringIncrement(key, val);
        }

        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        public double StringDecrement(string key, double val = 1)
        {
            return _db.StringDecrement(key, val);
        }

        #endregion

        #region Hash

        /// <summary>
        /// 判断某个数据是否已经被缓存
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="dataKey">数据中的key</param>
        /// <returns></returns>
        public bool HashExists(string key, string dataKey)
        {
            return _db.HashExists(key, dataKey);
        }

        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool HashSet<T>(string key, string dataKey, T t)
        {
            string json = JsonConvert.SerializeObject(t);

            return _db.HashSet(key, dataKey, json);
        }

        /// <summary>
        /// 移除hash中的某值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public bool HashDelete(string key, string dataKey)
        {
            return _db.HashDelete(key, dataKey);
        }

        /// <summary>
        /// 移除hash中的多个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKeys"></param>
        /// <returns></returns>
        public long HashDelete(string key, List<string> dataKeys)
        {
            return _db.HashDelete(key, dataKeys.Select(m => (RedisValue)m).ToArray());
        }

        /// <summary>
        /// 从hash表获取某个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public string HashGet(string key, string dataKey)
        {
            return _db.HashGet(key, dataKey);
        }

        /// <summary>
        /// 从hash表获取Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public T HashGet<T>(string key, string dataKey)
        {
            string value = _db.HashGet(key, dataKey);
            return JsonConvert.DeserializeObject<T>(value);
        }

        /// <summary>
        /// 从hash表获取List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public IList<T> HashGetList<T>(string key)
        {
            RedisValue[] values = _db.HashValues(key);

            List<T> result = new List<T>();
            foreach (var item in values)
            {
                var model = JsonConvert.DeserializeObject<T>(item);
                result.Add(model);
            }
            return result;
        }

        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        public double HashIncrement(string key, string dataKey, double val = 1)
        {
            return _db.HashIncrement(key, dataKey, val);
        }

        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        public double HashDecrement(string key, string dataKey, double val = 1)
        {
            return _db.HashDecrement(key, dataKey, val);
        }

        /// <summary>
        /// 获取hashkey所有Redis key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> HashKeys<T>(string key)
        {
            RedisValue[] values = _db.HashKeys(key);
            List<T> result = new List<T>();
            foreach (var item in values)
            {
                var model = JsonConvert.DeserializeObject<T>(item);
                result.Add(model);
            }

            return result;
        }

        #endregion

        #region List

        /// <summary>
        /// 移除指定ListId的内部List的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public long ListRemove<T>(string key, T value)
        {
            return _db.ListRemove(key, JsonConvert.SerializeObject(value));
        }

        /// <summary>
        /// 获取指定key的List
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> ListRange<T>(string key)
        {
            var values = _db.ListRange(key);

            List<T> result = new List<T>();
            foreach (var item in values)
            {
                var model = JsonConvert.DeserializeObject<T>(item);
                result.Add(model);
            }
            return result;
        }

        /// <summary>
        /// 入队
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public long ListRightPush<T>(string key, T value)
        {
            return _db.ListRightPush(key, JsonConvert.SerializeObject(value));
        }

        /// <summary>
        /// 出队
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T ListRightPop<T>(string key)
        {
            var value = _db.ListRightPop(key);
            return JsonConvert.DeserializeObject<T>(value);
        }

        /// <summary>
        /// 入栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public long ListLeftPush<T>(string key, T value)
        {
            return _db.ListLeftPush(key, JsonConvert.SerializeObject(value));
        }

        /// <summary>
        /// 出栈
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T ListLeftPop<T>(string key)
        {
            var value = _db.ListLeftPop(key);
            return JsonConvert.DeserializeObject<T>(value);
        }

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long ListLength(string key)
        {
            return _db.ListLength(key);
        }

        #endregion

        #region Set 集合

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="score"></param>
        public bool SetAdd<T>(string key, T value, double score)
        {
            return _db.SetAdd(key, JsonConvert.SerializeObject(value));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public bool SetRemove<T>(string key, T value)
        {
            return _db.SetRemove(key, JsonConvert.SerializeObject(value));
        }

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> SetRangeByRank<T>(string key)
        {
            var values = _db.SetMembers(key);

            List<T> result = new List<T>();
            foreach (var item in values)
            {
                var model = JsonConvert.DeserializeObject<T>(item);
                result.Add(model);
            }
            return result;
        }

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long SetLength(string key)
        {
            return _db.SetLength(key);
        }

        #endregion

        #region SortedSet 有序集合

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="score"></param>
        public bool SortedSetAdd<T>(string key, T value, double score)
        {
            return _db.SortedSetAdd(key, JsonConvert.SerializeObject(value), score);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public bool SortedSetRemove<T>(string key, T value)
        {
            return _db.SortedSetRemove(key, JsonConvert.SerializeObject(value));
        }

        /// <summary>
        /// 获取全部
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> SortedSetRangeByRank<T>(string key)
        {
            var values = _db.SortedSetRangeByRank(key);

            List<T> result = new List<T>();
            foreach (var item in values)
            {
                var model = JsonConvert.DeserializeObject<T>(item);
                result.Add(model);
            }
            return result;
        }

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long SortedSetLength(string key)
        {
            return _db.SortedSetLength(key);
        }

        #endregion

        #region key

        /// <summary>
        /// 删除单个key
        /// </summary>
        /// <param name="key">redis key</param>
        /// <returns>是否删除成功</returns>
        public bool KeyDelete(string key)
        {
            return _db.KeyDelete(key);
        }

        /// <summary>
        /// 删除多个key
        /// </summary>
        /// <param name="keys">rediskey</param>
        /// <returns>成功删除的个数</returns>
        public long KeyDelete(List<string> keys)
        {
            return _db.KeyDelete(keys.Select(m => (RedisKey)m).ToArray());
        }

        /// <summary>
        /// 判断key是否存储
        /// </summary>
        /// <param name="key">redis key</param>
        /// <returns></returns>
        public bool KeyExists(string key)
        {
            return _db.KeyExists(key);
        }

        /// <summary>
        /// 重新命名key
        /// </summary>
        /// <param name="key">就的redis key</param>
        /// <param name="newKey">新的redis key</param>
        /// <returns></returns>
        public bool KeyRename(string key, string newKey)
        {
            return _db.KeyRename(key, newKey);
        }

        /// <summary>
        /// 设置Key的时间
        /// </summary>
        /// <param name="key">redis key</param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool KeyExpire(string key, TimeSpan? expiry = default)
        {
            return _db.KeyExpire(key, expiry);
        }

        #endregion key

        #region 其他

        /// <summary>
        /// 获取事务
        /// </summary>
        /// <returns></returns>
        public ITransaction CreateTransaction()
        {
            return _db.CreateTransaction();
        }

        /// <summary>
        /// 获取批处理（管道模式）
        /// </summary>
        /// <returns></returns>
        public IBatch CreateBatch()
        {
            return _db.CreateBatch();
        }

        public void SetDatabase(int dbNum)
        {
            _db = _conn.GetDatabase(dbNum);
        }

        #endregion 其他
    }
}
