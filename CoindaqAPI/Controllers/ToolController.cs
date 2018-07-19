using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using CoindaqAPI.Utils;
using CoindaqAPI.Utils.FiatCurrency;
using CoindaqAPI.Utils.Filter;
using CoindaqAPI.Utils.Redis;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoindaqAPI.Controllers
{
    [Route("/api/v1/[controller]")]
    [Exception]
    public class ToolController : BaseController, IDisposable
    {
        public UploadInfo Info { get; set; }
        private readonly ILogger logger;
        public ToolController(IHostingEnvironment env):
            base("ServicesUrl\\UserService")
        {
            Info = new UploadInfo();
            logger = new Logger<ToolController>(new LoggerFactory().AddConsole());
        }

        /// <summary>
        /// Upload file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("UploadFile")]
        [TokenCheck]
        public async Task<ActionResult> UploadFile([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Json(new JsonResultModel(ReturnCode.ParameterError, "Upload file should not be blank."));
            }

            //验证上传大小
            if (file.Length > Info.MaxSize)
            {
                return Json(new JsonResultModel(ReturnCode.ParameterError, "Upload file size is over limited."));
            }

            //验证文件格式
            string oldName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            string extName = oldName.Substring(oldName.LastIndexOf('.')).Replace("\"", "");
            if (!Info.SuffixLimit.Contains(extName.Replace(".", "").ToLower()))
            {
                return Json(new JsonResultModel(ReturnCode.ParameterError, "Upload file suffix is not allowed."));
            }

            string shortName = $"{Guid.NewGuid()}{extName}";
            string newName = Path.Combine(Info.Directory, shortName);
            using (var stream = new FileStream(newName, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            JObject data = new JObject();
            data["url"] = $"/StaticFiles/{shortName}";
            return Json(new JsonResultModel(ReturnCode.Success, "Upload file successful.", data));
        }

        /// <summary>
        ///  Query currency info
        /// </summary>
        /// <param name="currencyIds"></param>
        /// <returns></returns>
        [HttpGet("QueryCurrencyInfo")]
        [TokenCheck]
        public ActionResult QueryCurrencyInfo([FromQuery]string[] currencyIds)
        {
            if(currencyIds.Length == 0)
            {
                return Json(new JsonResultModel(ReturnCode.ParameterError, "Parameter is not correct."));
            }
            List<CurrencyInfo> currencyInfoList = new List<CurrencyInfo>();
            foreach(var currencyId in currencyIds)
            {
                try
                {
                    var currencyInfo = RedisInstance.GetInstance().Get<CurrencyInfo>(currencyId);
                    //若查询为空，等待0.5s重新查找
                    if(currencyInfo == null)
                    {
                        Thread.Sleep(500);
                        currencyInfo = RedisInstance.GetInstance().Get<CurrencyInfo>(currencyId);
                    }
                    currencyInfoList.Add(currencyInfo);
                }
                catch (Exception ex)
                {
                    //Console.WriteLine("Query currency from redis got issue: ", ex.Message);
                    logger.LogError("Query currency from redis got issue: " + ex.Message);
                }
            }

            return Json(new JsonResultModel(ReturnCode.Success, "Query successful.", currencyInfoList));
        }

        [HttpGet("QueryCurrencyPriceInfo")]
        public ActionResult QueryCurrencyPriceInfo([FromQuery]string name, [FromQuery]string convert)
        {
            var currencySet = new CurrencySet();
            string result = currencySet.GetCurrencyPriceInfo(name, convert);
            var jsonList = JsonConvert.DeserializeObject(result);

            return Json(new JsonResultModel(ReturnCode.Success, "Query currency price info successful.", jsonList));
        }
    }
}