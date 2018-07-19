/**************************************************************
 *  Filename:      RedisScript
 *
 *  Description:  RedisScript ClassFile.
 *
 *  Company:     Coindaq 
 *
 *  @Author:      ChaoShu
 *
 *  @Created:    2018/6/6/周三 10:18:19 
 **************************************************************/

using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoindaqAPI.Utils.Redis
{
    public class RedisScript
    {
        public string Host { get; set; }
        public string Password { get; set; }
        ConnectionMultiplexer Client { get; set; }

        public static RedisScript RedisClient { get; set; }
        private static Redlock redlock = null;
        private static object _locker = new Object();

        public static RedisScript GetInstance()
        {
            if(RedisClient == null)
            {
                lock (_locker)
                {
                    if (RedisClient == null)
                    {
                        RedisClient = new RedisScript();
                    }
                }
            }
            return RedisClient;
        }
        private RedisScript()
        {
            //获取配置参数
            string redisHost, redisPwd, dbName;
            ParseConfig.ParseInfo("RedisConfig\\Redis_Engine\\Connection", out redisHost);
            ParseConfig.ParseInfo("RedisConfig\\Redis_Engine\\Password", out redisPwd);
            ParseConfig.ParseInfo("RedisConfig\\Redis_Engine\\DataBase", out dbName);

            Host = redisHost;
            Password = redisPwd;
            Client = ConnectionMultiplexer.Connect($"{Host}, password={Password}, allowAdmin = true");
        }

        public string ExecuteScript(string script, string key, string value)
        {
            var redisResult = Client.GetDatabase().ScriptEvaluate(
                LuaScript.Prepare(script),
                new { key=(RedisKey)key, value=value});

            return redisResult.ToJson();
        }

        public bool ExecuteScript(string commandJson, out string message)
        {
            message = "";
            try
            {
                var db = Client.GetDatabase(0);
                RedisResult returnValue = db.Execute("EVALSHA", new object[] {
                    "2a3b179577623abfbcc09958f5e8c99382983a1f",
                    "2",
                    "placeICO",
                    commandJson
                });

                if (returnValue.IsNull)
                {
                    message = "Return Empty";
                    return false;
                }
                int result = Int32.Parse(returnValue.ToString().Replace("{", "").Replace("}", ""));
                if (result == 2)
                {
                    message = "Success";
                    return true;
                }
                else if (result == 1)
                {
                    message = "Insufficient balance";
                    return false;
                }
                else
                {
                    message = "Other error";
                    return false;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }
    }
}
