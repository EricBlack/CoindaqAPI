/**************************************************************
 *  Filename:      EmailEvent
 *
 *  Description:  EmailEvent ClassFile.
 *
 *  Company:     Coindaq 
 *
 *  @Author:      ChaoShu
 *
 *  @Created:    2018/5/18/周五 11:23:05 
 **************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoindaqAPI.Utils.MsgServices
{
    public class EmailEvent
    {
        public static bool RegisterEmail(string userEmail, long userId, string activateCode, out string message)
        {

            string activateUrl = $"api/v1/user/ActivateEmailUser?";
            string apiHost;
            ParseConfig.ParseInfo("DeploySystem\\ApiHost", out apiHost);

            string registerTemplate = File.ReadAllText(@"wwwroot/EmailTemplate/RegisterTemplate.html");
            string body = registerTemplate.
                Replace("[UserEmail]", userEmail).
                Replace("[ApiHost]", apiHost).
                Replace("[ApiAddress]", activateUrl).
                Replace("[ActivateParameter]", $"userId={userId}&secret={activateCode}").
                Replace("[ActivateRandom]", Guid.NewGuid().ToString());

            EmailMsg msg = new EmailMsg(userEmail, "Register new account activate email", body);
            //同步发送邮件
            bool result = msg.Send(out message);
            return result;

            //异步邮件
            //msg.SendEmailByThread();
            //message = string.Empty;
            //return true;
        }

        public static bool ForgotPasswordEmail(string userEmail, string activateCode, out string message)
        {
            string resetPasswordTemplate = File.ReadAllText(@"wwwroot/EmailTemplate/ResetPasswordTemplate.html");
            string body = resetPasswordTemplate.
                Replace("[UserEmail]", userEmail).
                Replace("[VerifyCode]", activateCode);

            EmailMsg msg = new EmailMsg(userEmail, "Password reset email", body);

            //同步发送短信
            bool result = msg.Send(out message);
            return result;

            //异步发送短信
            //message = "";
            //msg.SendEmailByThread();
            //return true;
        }
    }
}
