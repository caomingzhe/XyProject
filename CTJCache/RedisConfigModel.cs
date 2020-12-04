using System;
using System.Collections.Generic;
using System.Text;

namespace CTCache
{
    /// <summary>
    /// redis 连接配置
    /// </summary>
   public class RedisConfigModel
    {
        public string ConnectionString { get; set; }

        public int DatabaseID { get; set; }
    }
}
