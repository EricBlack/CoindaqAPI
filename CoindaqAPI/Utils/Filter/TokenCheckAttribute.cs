/**************************************************************
 *  Filename:      TokenCheckAttribute
 *
 *  Description:  TokenCheckAttribute ClassFile.
 *
 *  Company:     Coindaq 
 *
 *  @Author:      ChaoShu
 *
 *  @Created:    2018/5/30/周三 16:06:12 
 **************************************************************/

using CoindaqAPI.Utils.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoindaqAPI.Utils.Filter
{
    public class TokenCheckAttribute : BaseFilter
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var logger = new Logger<TokenCheckAttribute>(new LoggerFactory().AddConsole());
            //接口免登实现，接口传入user-id和access_token，从redis中查找对应的用户信息

            string userId = context.HttpContext.Request.Headers["User-Id"];
            string accessToken = context.HttpContext.Request.Headers["Access-Token"];

            if (String.IsNullOrEmpty(userId) || String.IsNullOrEmpty(accessToken))
            {
                context.Result = Json(new JsonResultModel(ReturnCode.TokenError, "Api caller missed User-Id or Access-Token header parameter."));
                return;
            }

            bool result = TokenHelper.VerifyUserToken(userId, accessToken);

            if (!result)
            {
                logger.LogError("User token verify failed.");
                context.Result = Json(new JsonResultModel(ReturnCode.TokenError, "Access Token not correct."));
                return;
            }
            else
            {
                //重新设置token
                TokenHelper.SaveUserToken(userId, TimeSpan.FromHours(2));
            }

            base.OnActionExecuting(context);
        }
    }
}
