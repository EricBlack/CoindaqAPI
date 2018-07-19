/**************************************************************
 *  Filename:      RedisTest
 *
 *  Description: RedisTest ClassFile.
 *
 *  Company:     Coindaq 
 *
 *  @Author:      ChaoShu
 *  @Created:    2018/5/21/周一 12:45:53 
 **************************************************************/

using CoindaqAPI.Utils;
using CoindaqAPI.Utils.Redis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;

namespace CoindaqTest
{
    [TestClass]
    public class RedisTest
    {
        public RedisInstance Instance { get; set; }

        [TestInitialize]
        public void Init()
        {
            Instance = RedisInstance.GetInstance();
        }

        [TestMethod]
        public void AddItem()
        {
            Instance.Set("bitcoin", "test", TimeSpan.FromSeconds(60));
            Thread.Sleep(50);
            Instance.Set("bitcoin", "test1", TimeSpan.FromSeconds(60));
            Thread.Sleep(50);

            Instance.Set("bitcoin", "test2", TimeSpan.FromSeconds(60));
            Thread.Sleep(50);

        }

        [TestMethod]
        public void GetItem()
        {
            string value = Instance.Get("bitcoin");
            Console.WriteLine(value);
            Assert.AreEqual("test", value);
        }

        [TestMethod]
        public void IsExist()
        {
            bool result = Instance.Exists("bitcoin");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void RemoveItem()
        {
            bool result = Instance.Remove("bitcoin");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsNotExist()
        {
            bool result = Instance.Exists("bitcoin");
            Assert.IsFalse(result);
        }


        [TestMethod]
        public void RedisScriptTest()
        {
            string script = "{\"user_id\": \"24\", \"project_id\": \"6\", \"stage_id\": \"9\", \"price\":\"1500000000\", \"base_coin\":\"100001\", \"target_coin\":\"100002\", \"pay_count\":\"250000000\", \"locktype\":\"2\"}";

            var redis = new RedisScript("192.168.1.145:6379", "123456");
            string message;
            bool result =   redis.ExecuteScript(script, out message);

            Console.WriteLine($"Result: {result}, Message:{message}");
        }

        [TestMethod]
        public void ConvertObjectToJson()
        {
            JObject data = new JObject();
            data["user_id"] = "24"; //用户Id
            data["project_id"] = 6; //项目id
            data["stage_id"] = 9;
            data["price"] = 1500000000; //价格
            data["base_coin"] = "100001";
            data["target_coin"] = "100002";
            data["pay_count"] = 250000000; //花费 
            data["locktype"] = 2; //锁定状态

            string jsonData = JsonHelper.ToJson(data, true);

            string message;
            bool result = RedisScript.GetInstance().ExecuteScript(jsonData, out message);

            Console.WriteLine($"Result: {result}, Message:{message}");
        }
    }
}
