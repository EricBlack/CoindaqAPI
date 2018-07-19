/**************************************************************
 *  Filename:      CurrencySet
 *
 *  Description:  CurrencySet ClassFile.
 *
 *  Company:     Coindaq 
 *
 *  @Author:      ChaoShu
 *
 *  @Created:    2018/5/19/周六 12:10:35 
 **************************************************************/

using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CoindaqAPI.Utils.FiatCurrency
{
    public class CurrencySet
    {
        public string BaseUrl = "https://api.coinmarketcap.com/v1/ticker";
        private readonly ILogger logger;

        public CurrencySet()
        {
            logger = new Logger<CurrencySet>(new LoggerFactory().AddConsole());
        }

        public List<CurrencyInfo> GetAllCurrency()
        {
            var currencyList = new List<CurrencyInfo>();
            try
            {
                string jsonString = RequestCurrencyInfo();
                return ConvertFromJson(jsonString);
            }
            catch (Exception ex)
            {
                //Console.WriteLine("获取Currency异常：", ex.Message);
                logger.LogError("Request currency got exception: " +ex.Message);
            }

            return currencyList;
        }

        public CurrencyInfo GetCurrency(string name)
        {
            CurrencyInfo info = new CurrencyInfo();
            try
            {
                string jsonString = RequestCurrencyInfo(name);
                var currencyList = ConvertFromJson(jsonString);
                if (currencyList.Count > 0)
                    info = currencyList[0];
            }
            catch (Exception ex)
            {
                //Console.WriteLine("获取Currency异常：", ex.Message);
                logger.LogError("Request currency got exception: " + ex.Message);
            }

            return info;
        }

        public string GetCurrencyPriceInfo(string name, string convert)
        {
            var client = new HttpClient();
            string jsonResult = client.GetStringAsync($"{BaseUrl}/{name}?convert={convert}").Result;

            return jsonResult;
        }

        private string RequestCurrencyInfo()
        {
            var client = new HttpClient();
            string jsonResult = client.GetStringAsync(BaseUrl).Result;

            return jsonResult;
        }

        private string RequestCurrencyInfo(string id)
        {
            var client = new HttpClient();
            string jsonResult = client.GetStringAsync($"{BaseUrl}/{id}/").Result;

            return jsonResult;
        }

        private List<CurrencyInfo> ConvertFromJson(string jsonResult)
        {
            List<CurrencyInfo> currencyInfos = new List<CurrencyInfo>();
            var jsonSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            currencyInfos = JsonConvert.DeserializeObject<List<CurrencyInfo>>(jsonResult, jsonSetting);

            return currencyInfos;
        }
    }
}
