/**************************************************************
 *  Filename:      RedisInstance
 *
 *  Description:  RedisInstance ClassFile.
 *
 *  Company:     Coindaq 
 *
 *  @Author:      ChaoShu
 *
 *  @Created:    2018/5/21/周一 12:15:36 
 **************************************************************/

using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoindaqAPI.Utils.Redis
{
    public class RedisInstance
    {
        private static object _locker = new Object();
        private static RedisInstance _instance = null;
        private static Redlock redlock = null;
        private ConnectionMultiplexer _conn;
        private string _connStr;

        public static RedisInstance GetInstance()
        {
            if (_instance == null)
            {
                lock (_locker)
                {
                    if (_instance == null || _instance._conn.IsConnected == false)
                    {
                        _instance = new RedisInstance();
                    }
                }
            }
            return _instance;
        }
        public RedisInstance()
        {
            string redisServer, dbName;
            ParseConfig.ParseInfo("RedisConfig\\Redis_Default\\Connection", out redisServer);
            ParseConfig.ParseInfo("RedisConfig\\Redis_Default\\DataBase", out dbName);
            _connStr = string.Format("{0}, {1}", redisServer, dbName);
            _conn = ConnectionMultiplexer.Connect(_connStr);
            redlock = new Redlock(_conn);
        }

        public IDatabase GetDatabase()
        {
            try
            {
                return _conn.GetDatabase();
            }
            catch
            {
                _conn = ConnectionMultiplexer.Connect(_connStr);
                return _conn.GetDatabase();
            }
        }

        public bool SetExpire(string key, int seconds)
        {
            return GetDatabase().KeyExpire(key, DateTime.Now.AddSeconds(seconds));
        }

        private string MergeKey(string key)
        {
            return key;
        }

        public T Get<T>(string key)
        {
            key = MergeKey(key);
            return GetDatabase().StringGet(key).ToString().ToModel<T>();
        }

        public string Get(string key)
        {
            key = MergeKey(key);
            return GetDatabase().StringGet(key);
        }

        public bool Set(string key, string value)
        {
            key = MergeKey(key);
            return GetDatabase().StringSet(key, value);
        }

        public bool Set(string key, string value, TimeSpan span)
        {
            key = MergeKey(key);
            return GetDatabase().StringSet(key, value, span);
        }

        /// <summary>
        /// 判断在缓存中是否存在该key的缓存数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Exists(string key)
        {
            key = MergeKey(key);
            return GetDatabase().KeyExists(key);  //可直接调用
        }

        /// <summary>
        /// 移除指定key的缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            key = MergeKey(key);
            return GetDatabase().KeyDelete(key);
        }
    }
}
