/**************************************************************
 *  Filename:      LoginCheckAttribute
 *
 *  Description:  LoginCheckAttribute ClassFile.
 *
 *  Company:     Coindaq 
 *
 *  @Author:      ChaoShu
 *
 *  @Created:    2018/5/11/周五 12:40:53 
 **************************************************************/

using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoindaqAPI.Utils.Filter
{
    public class LoginCheckAttribute : BaseFilter
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var logger = new Logger<LoginCheckAttribute>(new LoggerFactory().AddConsole());
            string id = context.HttpContext.Session.GetString("Id");
            if (String.IsNullOrEmpty(id))
            {
                //接口免登实现，接口传入access_token，从db中查找对应的用户信息
                string accessToken = context.HttpContext.Request.Headers["Access-Token"];
                if (!String.IsNullOrEmpty(accessToken))
                {
                    try
                    {
                        var userClient = new UserService.UserServiceClient(Channel);
                        var reply = userClient.VerifyUserToken(new UserReq(){ Token = accessToken });
                    }
                    catch (RpcException ex)
                    {
                        //Console.WriteLine("Token验证错误: " + ex.Status.Detail);
                        logger.LogError("Token verify failed due to : " +ex.Status.Detail);
                        context.Result = Json(new JsonResultModel(ReturnCode.ParameterError, "Access Token not correct."));
                        return;
                    }
                }
                else
                {
                    context.Result = Json(new JsonResultModel(ReturnCode.LoginTimeOut, "User not login"));
                    return;
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
