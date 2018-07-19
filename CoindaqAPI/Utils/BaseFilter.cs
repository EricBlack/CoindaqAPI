/**************************************************************
 *  Filename:      BaseFilter
 *
 *  Description:  BaseFilter ClassFile.
 *
 *  Company:     Coindaq 
 *
 *  @Author:      ChaoShu
 *  
 *  @Created:    2018/5/11/周五 12:29:05 
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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoindaqAPI.Utils
{
    public class BaseFilter : ActionFilterAttribute
    {
        public Channel Channel { get; set; }

        public BaseFilter()
        {
            string serviceUrl;
            ParseConfig.ParseInfo("ServicesUrl\\UserService", out serviceUrl);
            var options = new List<ChannelOption>
            {
                new ChannelOption("grpc.initial_reconnect_backoff_ms", 100),
            };

            Channel = new Channel(serviceUrl, ChannelCredentials.Insecure, options);
        }
        
        //获取传入的数据
        protected JObject GetJsonBody(HttpRequest request)
        {
            try
            {
                String bodyContent;
                using (StreamReader streamReader = new StreamReader(request.Body))
                    bodyContent = streamReader.ReadToEnd();
                MemoryStream stream = new MemoryStream();
                StreamWriter streamWriter = new StreamWriter(stream);
                streamWriter.Write(bodyContent);
                streamWriter.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                request.Body = stream;
                return JObject.Parse(bodyContent);
            }
            catch (JsonReaderException)
            {
                return null;
            }
        }

        protected void UpdateFormData(ActionExecutingContext context, JObject body)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(stream);
            streamWriter.Write(body.ToString());
            streamWriter.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            context.HttpContext.Request.Body = stream;
        }

        public JsonResult Json(JsonResultModel data)
        {
            if (data.Data == null)
            {
                return new JsonResult(data, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            else
            {
                return new JsonResult(data);
            }
        }

        protected bool IsTestModel(ActionContext context)
        {
            IConfiguration config = (IConfigurationRoot)context.HttpContext.RequestServices.GetService(typeof(IConfigurationRoot));
            return bool.Parse(config["TestModel"]);
        }
    }
}
