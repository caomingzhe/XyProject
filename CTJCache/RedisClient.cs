using StackExchange.Redis;
using System;

namespace CTCache
{
    /// <summary>
    /// redis 操作
    /// </summary>
    public class RedisClient
    {

        private static readonly object Locker = new object();


        private ConnectionMultiplexer redisMultiplexer;

        //库
        private static IDatabase db = null;

        private static RedisClient _redisClient = null;

        public static RedisClient redisClient
        {
            get
            {
                if (_redisClient == null)
                {
                    lock (Locker)
                    {
                        if (_redisClient == null)
                        {
                            _redisClient = new RedisClient();

                        }
                    }
                }
                return _redisClient;
            }
        }
         

        public IDatabase InitConnect(RedisConfigModel Configuration)
        {
            try
            {

                redisMultiplexer = ConnectionMultiplexer.Connect(new ConfigurationOptions
                {
                    AbortOnConnectFail = false,
                    AllowAdmin = false,
                    SyncTimeout = 5000,
                    ConnectTimeout = 10000,
                    EndPoints = { Configuration.ConnectionString } 

                });
                db = redisMultiplexer.GetDatabase(Configuration.DatabaseID);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                redisMultiplexer = null;
                db = null;
            }

            return db;
        }


        public RedisClient()
        {

        }

    }
}
