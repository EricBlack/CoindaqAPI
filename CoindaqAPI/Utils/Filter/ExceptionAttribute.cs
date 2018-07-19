/**************************************************************
 *  Filename:      ExceptionAttribute
 *
 *  Description:  ExceptionAttribute ClassFile.
 *
 *  Company:     Coindaq 
 *
 *  @Author:      ChaoShu
 *
 *  @Created:    2018/5/11/周五 12:39:16 
 **************************************************************/

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoindaqAPI.Utils.Filter
{
    public class ExceptionAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var logger = new Logger<ExceptionAttribute>(new LoggerFactory().AddConsole());
            //Console.WriteLine("------------------------------" + DateTime.Now.ToString() + "---------------------------------");
            Exception ex = context.Exception;
            if (ex != null)
            {
                //Console.WriteLine("[ErrorType]" + ex.GetType());
                //Console.WriteLine("[HelpLink]" + ex.HelpLink);
                //Console.WriteLine("[Message]" + ex.Message);
                //Console.WriteLine("[Source]" + ex.Source);
                //Console.WriteLine("[StackTrace]" + ex.StackTrace);
                logger.LogError("[ErrorType]" + ex.GetType());
                logger.LogError("[HelpLink]" + ex.HelpLink);
                logger.LogError("[Message]" + ex.Message);
                logger.LogError("[Source]" + ex.Source);
                logger.LogError("[StackTrace]" + ex.StackTrace);
            }
            else
            {
                //Console.WriteLine("Exception is NULL");
                logger.LogError("Exception is NULL");
            }

            context.Result = new JsonResult(new JsonResultModel(ReturnCode.UnknownError, "Unknown issue happened, please check your request  parameter first."), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            base.OnException(context);
        }
    }
}
