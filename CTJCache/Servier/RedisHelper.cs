using CTCache.interfaces;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CTCache.Servier
{
    public class RedisHelper : IRedisHelper
    {

        IDatabase db = RedisClient.redisClient.InitConnect(new RedisConfigModel { DatabaseID = 0, ConnectionString = "localhost:6379" });

        #region string
        /// <summary>
        /// 保存单个key value
        /// </summary>
        /// <param name="value">保存的值</param>
        /// <param name="expiry">过期时间</param>
        public bool SetStringKey(string key, string value, TimeSpan? expiry = default(TimeSpan?))
        {
            return db.StringSet(key, value, expiry);
        }


        /// <summary>
        /// 保存多个key value（事务）
        /// </summary>
        /// <param name="value">保存的值</param>
        /// <param name="expiry">过期时间</param>
        public bool SetStringKeys(Dictionary<string, string> values, TimeSpan? expiry = default(TimeSpan?))
        {
            var tran = db.CreateTransaction();
            foreach (var value in values)
            {
                tran.StringSetAsync(value.Key, value.Value);
            }
            return tran.Execute();
        }

        /// <summary>
        /// 修改单个key value
        /// </summary>
        /// <param name="value">保存的值</param>
        /// <param name="expiry">过期时间</param>
        public bool EditStringKey(string key, string value, TimeSpan? expiry = default(TimeSpan?))
        {
            if (db == null)
            {
                return false;
            }

            db.KeyDelete(key);
            return SetStringKey(key, value, expiry);
        }

        /// <summary>
        /// 获取单个key的值
        /// </summary>
        public RedisValue GetStringKey(string key)
        {
            return db.StringGet(key);
        }


        /// <summary>
        /// 获取一个key的对象
        /// </summary>
        public T GetStringKey<T>(string key) where T : class
        {
            if (db == null)
            {
                return default;
            }
            var value = db.StringGet(key);
            if (value.IsNullOrEmpty)
            {
                return default;
            }
            return JsonConvert.DeserializeObject<T>(value);
        }

        /// <summary>
        /// 保存一个对象
        /// </summary>
        /// <param name="obj"></param>
        public bool SetStringKey<T>(string key, T obj, TimeSpan? expiry = default(TimeSpan?)) where T : class
        {
            if (db == null)
            {
                return false;
            }
            string json = JsonConvert.SerializeObject(obj);
            return db.StringSet(key, json, expiry);
        }


        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="obj"></param>
        public bool DeleteKey(string key)
        {
            if (db == null)
            {
                return false;
            }
            return db.KeyDelete(key);
        }
        #endregion

        #region list

        /// <summary>
        ///  在列表头部插入值。如果键不存在，先创建再插入值
        /// </summary>
        /// <param name="value">保存的值</param> 
        /// <param name="expiry">过期时间</param>
        public long ListLeftPush(string redisKey, string value, TimeSpan? expiry = default(TimeSpan?))
        {
            if (expiry != null)
            {
                db.KeyExpire(redisKey, expiry);
            }
            return db.ListLeftPush(redisKey, value);
        }

        /// <summary>
        ///  在列表尾部插入值。如果键不存在，先创建再插入值
        /// </summary>
        /// <param name="value">保存的值</param>
        /// <param name="expiry">过期时间</param>
        public long ListRightPush(string redisKey, string value, TimeSpan? expiry = default(TimeSpan?))
        {
            if (expiry != null)
            {
                db.KeyExpire(redisKey, expiry);
            }
            return db.ListRightPush(redisKey, value);
        }

        /// <summary>
        ///  根据下标修改值
        /// </summary>
        /// <param name="value">保存的值</param>
        /// <param name="expiry">过期时间</param>
        public void ListSetByIndex(string redisKey, string value, long index, TimeSpan? expiry = default(TimeSpan?))
        {
            if (index < (ListLength(redisKey) - 1))
            {
                db.ListSetByIndex(redisKey, index, value);
                if (expiry != null)
                {
                    db.KeyExpire(redisKey, expiry);
                }
            }
        }

        /// <summary>
        /// 列表长度
        /// </summary>
        /// <param name="redisKey"></param>

        /// <returns></returns>
        public long ListLength(string redisKey)
        {
            return db.ListLength(redisKey);
        }

        /// <summary>
        /// 查询列表数据
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public IEnumerable<string> ListRange(string redisKey)
        {
            var result = db.ListRange(redisKey);
            return result.Select(o => o.ToString());
        }

        /// <summary>
        /// 根据索引获取指定位置数据
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>

        /// <returns></returns>
        public IEnumerable<string> ListRange(string redisKey, int start, int stop)
        {
            var result = db.ListRange(redisKey, start, stop);
            return result.Select(o => o.ToString());
        }

        /// <summary>
        /// 删除List中的元素 并返回删除的个数
        /// </summary>
        /// <param name="redisKey">key</param>
        /// <param name="redisValue">元素</param>
        /// <param name="type">大于零 : 从表头开始向表尾搜索，小于零 : 从表尾开始向表头搜索，等于零：移除表中所有与 VALUE 相等的值</param>

        /// <returns></returns>
        public long ListDelRange(string redisKey, string redisValue, long type = 0)
        {
            return db.ListRemove(redisKey, redisValue, type);
        }

        /// <summary>
        /// 清空List
        /// </summary>
        /// <param name="redisKey"></param>

        public void ListClear(string redisKey)
        {
            db.ListRemove(redisKey, 1, 0);

        }

        #endregion

        #region hash

        /// <summary>
        /// 保存(修改)多个对象集合  非序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">key</param>
        /// <param name="list">数据集合</param>
        /// <param name="getModelId">字段key</param>
        public void HashSet<T>(string redisKey, IEnumerable<T> list, Func<T, string> getModelId, TimeSpan? span = null) where T : class
        {
            try
            {
                List<HashEntry> listHashEntry = new List<HashEntry>();
                foreach (var item in list)
                {
                    string json = JsonConvert.SerializeObject(item);
                    listHashEntry.Add(new HashEntry(getModelId(item), json));
                }
                db.HashSet(redisKey, listHashEntry.ToArray());

                if (span != null)
                {
                    //设置过期时间
                    db.KeyExpire(redisKey, span);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        ///  根据hashkey获取所有的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisKey"></param>

        /// <returns></returns>
        public List<T> HashGetAll<T>(string redisKey) where T : class
        {
            List<T> result = new List<T>();
            try
            {
                HashEntry[] arr = db.HashGetAll(redisKey);
                foreach (var item in arr)
                {
                    if (!item.Value.IsNullOrEmpty)
                    {
                        var t = JsonConvert.DeserializeObject<T>(item.Value);
                        if (t != null)
                        {
                            result.Add(t);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// 从 hash 中移除多个指定字段
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="hashField"></param>
        /// <returns></returns>
        public long HashDelete(string redisKey, IEnumerable<RedisValue> hashField)
        {
            try
            {
                return db.HashDelete(redisKey, hashField.ToArray());
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 删除 hash值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="hashField"></param>
        /// <returns></returns>
        public long HashDelete(string redisKey)
        {
            try
            {
                return db.HashDelete(redisKey, db.HashKeys(redisKey));
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region sets

        /// <summary>
        /// set添加多个对象集合 
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="value"></param>

        /// <returns></returns>
        public long SetAddList<T>(string redisKey, IEnumerable<T> list, TimeSpan? span = null) where T : class
        {
            try
            {
                List<RedisValue> listRedisValue = new List<RedisValue>();
                foreach (var item in list)
                {
                    string json = JsonConvert.SerializeObject(item);
                    listRedisValue.Add(json);
                }
                if (span != null)
                {
                    //设置过期时间
                    db.KeyExpire(redisKey, span);
                }
                return db.SetAdd(redisKey, listRedisValue.ToArray());
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// 根据key获取所有Set对象集合 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisKey"></param>

        /// <returns></returns>
        public List<T> GetAllMembers<T>(string redisKey) where T : class
        {
            List<T> result = new List<T>();
            try
            {
                var arr = db.SetMembers(redisKey);
                foreach (var item in arr)
                {
                    if (!item.IsNullOrEmpty)
                    {
                        var t = JsonConvert.DeserializeObject<T>(item);
                        if (t != null)
                        {
                            result.Add(t);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
            return result;
        }

        /// <summary>
        /// 根据value删除Set对象集合 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public long SetRemove(string redisKey, IEnumerable<RedisValue> hashField)
        {
            return db.SetRemove(redisKey, hashField.ToArray());
        }


        #endregion

        #region  Sorted Sets

        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="member"></param>
        /// <param name="score"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public bool ZSortadd(string key, string member, double score)
        {
            try
            {
                if ((object)key == null)
                    return false;
                else
                    return db.SortedSetAdd(key, member, score);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 移除某个元素
        /// </summary>
        /// <param name="key"></param>
        /// <param name="memebr"></param>
        /// <returns></returns>
        public bool ZSortedRemove(string key, string memebr)
        {
            try
            {


                if ((object)key == null)
                    return false;
                else
                    return db.SortedSetRemove(key, memebr);

            }
            catch (Exception)
            {
                return false;
            }
        }


        /// <summary>
        /// 获取集合的所有值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="memebr"></param>
        /// <returns></returns>
        public SortedSetEntry[] ZSortedRemoveAll(string key)
        {
            try
            {
                return db.SortedSetRangeByRankWithScores(key);
            }
            catch (Exception)
            {
                return new SortedSetEntry[] { };
            }
        }

        #endregion

        #region 事务
        /// <summary>
        /// 创建事务
        /// </summary>
        /// <returns></returns>
        public ITransaction CreateTransaction()
        {
            return db.CreateTransaction();
        }
        #endregion

    }
}
