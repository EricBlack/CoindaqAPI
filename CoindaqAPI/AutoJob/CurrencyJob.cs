/**************************************************************
 *  Filename:      CurrencyJob
 *
 *  Description:  CurrencyJob ClassFile.
 *
 *  Company:     Coindaq 
 *
 *  @Author:      ChaoShu
 *
 *  @Created:    2018/5/21/周一 10:54:49 
 **************************************************************/

using CoindaqAPI.Utils;
using CoindaqAPI.Utils.FiatCurrency;
using CoindaqAPI.Utils.Redis;
using Microsoft.Extensions.Logging;
using Pomelo.AspNetCore.TimedJob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoindaqAPI.AutoJob
{
    public class CurrencyJob: Job
    {
        public CurrencySet CurrencySet { get; set; }
        private readonly ILogger logger;
        public CurrencyJob()
        {
            CurrencySet = new CurrencySet();
            logger = new Logger<CurrencyJob>(new LoggerFactory().AddConsole());
            //logger.LogInformation("Init currency job.");
        }

        [Invoke(Begin ="2018-05-21 00:00", Interval =1000*30, SkipWhileExecuting = true)]
        public void QueryCurrency(IServiceProvider services)
        {
            var list = CurrencySet.GetAllCurrency();
            //失败隔30秒再请求一次
            if(list.Count ==0)
            {
                //Console.WriteLine("查询异常");
                logger.LogError("Query currency exception.");
            }
            else
            {
                //添加所有Currency到Redis数据库
                foreach(var item in list)
                {
                    try
                    {
                        RedisInstance.GetInstance().Set(item.Id, JsonHelper.ToJson(item), TimeSpan.FromSeconds(60));
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"Add coin {item.Name} to redis failed.");
                        logger.LogError($"Exception message: {ex.Message}");
                    }
                }
            }
        }
    }
}
