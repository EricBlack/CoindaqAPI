/**************************************************************
 *  Filename:      BaseController
 *
 *  Description:  BaseController ClassFile.
 *
 *  Company:     Coindaq 
 *
 *  @Author:      ChaoShu
 *
 *  @Created:    2018/5/11/周五 12:34:56 
 **************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CoindaqAPI.Utils.Extensions;
using CoindaqAPI.Utils.Security;

namespace CoindaqAPI.Utils
{
    public class BaseController : Controller, IDisposable
    {
        public Channel Channel { get; set; }

        public BaseController(string serviceConfig)
        {
            string value = "";
            bool result = ParseConfig.ParseInfo(serviceConfig, out value);
            if (result)
            {
                var baseService = new BaseService(value);
                Channel = baseService.Channel;
            }
            else
                Channel = null;
        }

        public new void Dispose()
        {
            Channel.ShutdownAsync().Wait();
        }

        protected JObject GetJsonBody()
        {
            try
            {
                string bodyContent;
                using (StreamReader streamReader = new StreamReader(Request.Body))
                    bodyContent = streamReader.ReadToEnd();

                return JObject.Parse(bodyContent);
            }
            catch (JsonReaderException)
            {
                return null;
            }
        }

        public JsonResult Json(JsonResultModel data)
        {
            //添加token返回信息
            string userId = GetHeader("User-Id");
            if (!String.IsNullOrEmpty(userId))
            {
                data.Token = TokenHelper.GetUserToken(userId);
            }
            else
            {
                data.Token = null;
            }

            if (data.Data == null)
            {
                return base.Json(data, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            else
            {
                return base.Json(data);
            }
        }

        public JsonResult Json(JsonResultModel data, bool token)
        {
            if (data.Data == null)
            {
                return base.Json(data, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            else
            {
                return base.Json(data);
            }
        }

        public string GetSession(string key)
        {
            return HttpContext.Session.GetString(key);
        }

        public void SetSession(string key, string value)
        {
            HttpContext.Session.SetString(key, value);
        }

        public void RemoveSession(string key)
        {
            HttpContext.Session.Remove(key);
        }

        public T GetSession<T>(string key)
        {
            return HttpContext.Session.Get<T>(key);
        }

        public void SetSession<T>(string key, T value)
        {
            HttpContext.Session.Set<T>(key, value);
        }

        public string GetHeader(string key)
        {
            return HttpContext.Request.Headers[key];
        }
    }
}
