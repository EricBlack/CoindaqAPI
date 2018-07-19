/**************************************************************
 *  Filename:      UserController
 *
 *  Description:  UserController ClassFile.
 *
 *  Company:     Coindaq 
 *
 *  @Author:      ChaoShu
 *
 *  @Created:    2018/5/11/周五 14:02:41 
 **************************************************************/

using CoindaqAPI.Utils;
using CoindaqAPI.Utils.Filter;
using Proto;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using Newtonsoft.Json.Linq;
using CoindaqAPI.Utils.MsgServices;
using CoindaqAPI.Utils.Security;

namespace CoindaqAPI.Controllers
{
    //[Produces("application/json")]
    [Route("/api/v1/[controller]")]
    [Exception]
    public class UserController : BaseController
    {
        public UserService.UserServiceClient Client { get; set; }
        public TwoFactorService.TwoFactorServiceClient FactorClient { get; set; }
        public TiantianMsg MsgService { get; set; }
        public UserController() :
            base("ServicesUrl\\UserService")
        {
            //BaseService.CheckChannelStatus(Channel);
            Client = new UserService.UserServiceClient(Channel);
            FactorClient = new TwoFactorService.TwoFactorServiceClient(Channel);
            MsgService = new TiantianMsg(Client);
        }

        /// <summary>
        /// Register new user
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        [HttpPost("Signup")]
        //[ProducesResponseType(typeof(UserReply), 200)]
        public IActionResult Signup([FromBody] RegisterReq register)
        {
            try
            {
                if (!ValidateHelper.IsEmailFormat(register.Email))
                    return Json(new JsonResultModel(ReturnCode.ParameterError, "Email format is not correct."));
                if (register.Password == "")
                    return Json(new JsonResultModel(ReturnCode.ParameterError, "Password should not blank."));
                if (register.DeviceId == "")
                    return Json(new JsonResultModel(ReturnCode.ParameterError, "Device id should not blank."));

                if(IsEnableInviteCode())
                {
                    register.EnableCode = true;
                    if(register.InviteCode == "")
                        return Json(new JsonResultModel(ReturnCode.ParameterError, "Miss invite code parameter cannot complete register."));
                }
                
                var reply = Client.Signup(register);
                JObject data = new JObject();
                data["Id"] = reply.Id;
                data["Email"] = reply.Email;
                data["Activated"] = reply.Activated.ToString();
                data["RegistIp"] = reply.RegistIp;
                data["DeviceId"] = reply.DeviceId;

                var factor = FactorClient.QueryFactor(new FactorReq() { UserId = reply.Id, Type = FactorType.EmailAuthType });

                //发送邮件
                string emailMessage;
                bool send = EmailEvent.RegisterEmail(reply.Email, reply.Id, factor.Code, out emailMessage);
                if (!send)
                {
                    return Json(new JsonResultModel(ReturnCode.MessageError, $"Email send failed: {emailMessage}"));
                }

                return Json(new JsonResultModel(ReturnCode.Success, "User register successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Login in
        /// </summary>
        /// <param name="auth"></param>
        /// <returns></returns>
        [HttpPost("Signin")]
        //[ProducesResponseType(typeof(UserReply), 200)]
        public IActionResult Signin([FromBody] AuthReq auth)
        {
            try
            {
                if (!ValidateHelper.IsEmailFormat(auth.Email))
                    return Json(new JsonResultModel(ReturnCode.ParameterError, "Email format is not correct."));

                var reply = Client.Signin(auth);
                //string token = Guid.NewGuid().ToString("N");
                string loginTime = DateTime.Now.ToString();
                SetSession("Id", reply.Id);
                SetSession("LoginTime", loginTime);

                var accessToken = TokenHelper.SaveUserToken(reply.Id.ToString(), TimeSpan.FromHours(2));
                var jsonModel = new JsonResultModel(ReturnCode.Success, "User login successful.", reply);
                jsonModel.Token = accessToken;

                return Json(jsonModel, true);
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Check user bind status
        /// </summary>
        /// <param name="forgetEmail"></param>
        /// <returns></returns>
        [HttpGet("CheckUserStatus")]
        public IActionResult CheckUserStatus([FromQuery] ForgetEmailReq forgetEmail)
        {
            try
            {
                if (!ValidateHelper.IsEmailFormat(forgetEmail.Email))
                    return Json(new JsonResultModel(ReturnCode.ParameterError, "Email format is not correct."));

                var reply = Client.CheckUserStatus(forgetEmail);
                return Json(new JsonResultModel(ReturnCode.Success, "Check user status successful.", reply));
                
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.QueryError, ex.Status.Detail));
            }
        }
        
        /// <summary>
        /// User logout
        /// </summary>
        /// <returns></returns>
        [HttpGet("Logout")]
        [TokenCheck]
        public IActionResult Logout()
        {
            try
            {
                int id = Convert.ToInt32(GetSession("Id"));
                
                RemoveSession("Id");
                RemoveSession("AccessToken");
                RemoveSession("LastLoginTime");
                var user = new IdReq { Id = id };
                var reply = Client.Logout(user);

                string userId = GetHeader("User-Id");
                string accessToken = GetHeader("Access-Token");
                bool check = TokenHelper.DeleteUserToken(userId);

                var resultModel = new JsonResultModel(ReturnCode.Success, "User logout successful.");
                resultModel.Token = null;

                return Json(resultModel);
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Activate user
        /// </summary>
        /// <param name="emailReq"></param>
        /// <returns></returns>
        [HttpGet("ActivateNewUser")]
        public IActionResult ActivateNewUser([FromQuery] ForgetEmailReq emailReq)
        {
            try
            {
                if (!ValidateHelper.IsEmailFormat(emailReq.Email))
                    return Json(new JsonResultModel(ReturnCode.ParameterError, "Email format is not correct."));

                var activaeFactor = Client.ActivateNewUser(emailReq);

                //发送邮件
                string emailMessage;
                bool send = EmailEvent.RegisterEmail(emailReq.Email, activaeFactor.UserId, activaeFactor.Code, out emailMessage);
                if (!send)
                {
                    return Json(new JsonResultModel(ReturnCode.MessageError, $"Email send failed: {emailMessage}"));
                }

                return Json(new JsonResultModel(ReturnCode.Success, "User active email send successful."));
            }
            catch (RpcException)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, "Activate link is expired."));
            }
        }

        /// <summary>
        /// Activate Email account
        /// </summary>
        /// <param name="activateReq"></param>
        /// <returns></returns>
        [HttpGet("ActivateEmailUser")]
        public IActionResult ActivateEmailUser([FromQuery] ActivateReq activateReq)
        {
            try
            {
                var reply = Client.ActivateEmailUser(activateReq);
                return Json(new JsonResultModel(ReturnCode.Success, "User activate successful."));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }
        /// <summary>
        /// Forgot password via email
        /// </summary>
        /// <param name="forgetEmail"></param>
        /// <returns></returns>
        [HttpPost("ForgetPasswordViaEmail")]
        public IActionResult ForgetPasswordViaEmail([FromBody] ForgetEmailReq forgetEmail)
        {
            try
            {
                if (!ValidateHelper.IsEmailFormat(forgetEmail.Email))
                    return Json(new JsonResultModel(ReturnCode.ParameterError, "Email format is not correct."));

                var reply = Client.ForgetPasswordViaEmail(forgetEmail);
                
                //发送邮件
                string emailMessage;
                bool send = EmailEvent.ForgotPasswordEmail(forgetEmail.Email, reply.Secrect, out emailMessage);
                if (!send)
                {
                    return Json(new JsonResultModel(ReturnCode.MessageError, $"Email send failed: {emailMessage}"));
                }

                return Json(new JsonResultModel(ReturnCode.Success, "Retrieve the password from email successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Forgot password via phone
        /// </summary>
        /// <param name="forgetPhone"></param>
        /// <returns></returns>
        [HttpPost("ForgetPasswordViaPhone")]
        public IActionResult ForgetPasswordViaPhone([FromBody] ForgetPhoneReq forgetPhone)
        {
            try
            {
                if (!ValidateHelper.IsEmailFormat(forgetPhone.Email))
                    return Json(new JsonResultModel(ReturnCode.ParameterError, "Email format is not correct."));

                if (!ValidateHelper.IsPhoneFormat(forgetPhone.Phone))
                    return Json(new JsonResultModel(ReturnCode.ParameterError, "Phone format is not correct."));

                var reply = Client.ForgetPasswordViaPhone(forgetPhone);

                //发送短信
                string msg_code = "";
                string msg_msg = "";
                string message = MsgContent(forgetPhone.Phone, reply.Secrect);
                bool result = MsgService.SendMessage(forgetPhone.Phone, message, out msg_code, out msg_msg);
                
                //保存发送记录到数据库
                var msgRecord = new RecordMessageReq() {
                    Destination = forgetPhone.Phone,
                    Message = message,
                    SendStatus = result ? MsgSendStatus.Success : MsgSendStatus.Failed,
                    ReturnMessage = msg_msg
                };

                if(TiantianMsg.Config.EnableMsg)
                    Client.RecordMessageInfo(msgRecord);

                if(result)
                    return Json(new JsonResultModel(ReturnCode.MessageError, msg_msg));

                return Json(new JsonResultModel(ReturnCode.Success, "Text message send to phone successful."));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Reset password
        /// </summary>
        /// <param name="modifyPassword"></param>
        /// <returns></returns>
        [HttpPut("ResetPassword")]
        public IActionResult ResetPassword([FromBody] ModifyPasswordReq modifyPassword)
        {
            try
            {
                var reply = Client.ResetPassword(modifyPassword);
                return Json(new JsonResultModel(ReturnCode.Success, "User password reset successful."));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Update password
        /// </summary>
        /// <param name="updatePassword"></param>
        /// <returns></returns>
        [HttpPut("UpdatePassword")]
        [TokenCheck]
        public IActionResult UpdatePassword([FromBody] UpdatePasswordReq updatePassword)
        {
            try
            {
                var reply = Client.UpdatePassword(updatePassword); 
                return Json(new JsonResultModel(ReturnCode.Success, "User password update successful."));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Send bind Message
        /// </summary>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        [HttpPost("SendBindMessage")]
        public IActionResult SendBindMessage([FromBody] SendMessageReq sendMessage)
        {
            try
            {
                if (!ValidateHelper.IsPhoneFormat(sendMessage.Phone))
                    return Json(new JsonResultModel(ReturnCode.ParameterError, "Phone format is not correct."));

                var reply = Client.SendBindMessage(sendMessage);

                //发送短信
                string msg_code = "";
                string msg_msg = "";
                string message = MsgContent(sendMessage.Phone, reply.Code);
                bool result = MsgService.SendMessage(sendMessage.Phone, message, out msg_code, out msg_msg);

                //保存发送记录到数据库
                var msgRecord = new RecordMessageReq()
                {
                    Destination = sendMessage.Phone,
                    Message = message,
                    SendStatus = result ? MsgSendStatus.Success : MsgSendStatus.Failed,
                    ReturnMessage = msg_msg
                };

                if (TiantianMsg.Config.EnableMsg)
                    Client.RecordMessageInfo(msgRecord);

                if (!result)
                    return Json(new JsonResultModel(ReturnCode.MessageError, msg_msg));

                return Json(new JsonResultModel(ReturnCode.Success, "Bind phone text message send to user successful."));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }
        
        /// <summary>
        /// Bind user phone info
        /// </summary>
        /// <param name="phoneReq"></param>
        /// <returns></returns>
        [HttpPut("BindUserPhone")]
        public IActionResult BindUserPhone([FromBody] BindPhoneReq phoneReq)
        {
            try
            {
                if (!ValidateHelper.IsPhoneFormat(phoneReq.Phone))
                    return Json(new JsonResultModel(ReturnCode.ParameterError, "Phone format is not correct."));

                var reply = Client.BindUserPhone(phoneReq);
                return Json(new JsonResultModel(ReturnCode.Success, "User phone binding successful."));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Update user information
        /// </summary>
        /// <param name="modifyUser"></param>
        /// <returns></returns>
        [HttpPut("UpdateUserInfo")]
        [TokenCheck]
        public IActionResult UpdateUserInfo([FromBody] ModifyUserReq modifyUser)
        {
            try
            {
                var reply = Client.UpdateUserInfo(modifyUser);
                return Json(new JsonResultModel(ReturnCode.Success, "User inforamtion update successful."));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Query user by id
        /// </summary>
        /// <param name="idReq"></param>
        /// <returns></returns>
        [HttpGet("QueryUserById")]
        public IActionResult QueryUserById([FromQuery] IdReq idReq)
        {
            try
            {
                var reply = Client.QueryUserById(idReq);
                if (reply.Id == 0)
                    reply = null;

                return Json(new JsonResultModel(ReturnCode.Success, "User information query successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.QueryError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Query user by filter
        /// </summary>
        /// <param name="queryUser"></param>
        /// <returns></returns>
        [HttpGet("QueryUsers")]
        [TokenCheck]
        public IActionResult QueryUsers([FromQuery] QueryUserReq queryUser)
        {
            try
            {
                var reply = Client.QueryUsers(queryUser);
                return Json(new JsonResultModel(ReturnCode.Success, "User information query successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.QueryError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Query user login history
        /// </summary>
        /// <param name="recordFilter"></param>
        /// <returns></returns>
        [HttpGet("QueryLoginRecords")]
        [TokenCheck]
        public IActionResult QueryLoginRecords([FromQuery] RecordFilterReq recordFilter)
        {
            try
            {
                var reply = Client.QueryLoginRecords(recordFilter);
                return Json(new JsonResultModel(ReturnCode.Success, "Login history query successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.QueryError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Update user payment password 
        /// </summary>
        /// <param name="passwordReq"></param>
        /// <returns></returns>
        [HttpPost("UpdatePaymentPassword")]
        [TokenCheck]
        public IActionResult UpdatePaymentPassword([FromBody] PaymentPasswordReq passwordReq)
        {
            try
            {
                var reply = Client.UpdatePaymentPassword(passwordReq);
                return Json(new JsonResultModel(ReturnCode.Success, "User payment password update successful."));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Verify user payment password
        /// </summary>
        /// <param name="passwordReq"></param>
        /// <returns></returns>
        [HttpPost("VerifyPaymentPassword")]
        [TokenCheck]
        public IActionResult VerifyPaymentPassword([FromBody] PaymentPasswordReq passwordReq)
        {
            try
            {
                var reply = Client.VerifyPaymentPassword(passwordReq);
                return Json(new JsonResultModel(ReturnCode.Success, "User payment password verification successful."));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Verify user financial Operation 
        /// </summary>
        /// <param name="idReq"></param>
        /// <returns></returns>
        [HttpPost("VerifyFinancialOperation")]
        [TokenCheck]
        public IActionResult VerifyFinancialOperation([FromQuery] IdReq idReq)
        {
            try
            {
                var reply = Client.VerifyFinancialOperation(idReq);
                return Json(new JsonResultModel(ReturnCode.Success, "User Financial check successful."));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.QueryError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Bind user google factor
        /// </summary>
        /// <param name="idReq"></param>
        /// <returns></returns>
        [HttpPost("BindUserGoogleFactor")]
        [TokenCheck]
        public IActionResult BindUserGoogleFactor ([FromBody] IdReq idReq)
        {
            try
            {
                var reply = Client.BindUserGoogleFactor(idReq);
                reply.ImageUrl = $"/QrCode/{reply.ImageUrl}";   //设置访问路径

                return Json(new JsonResultModel(ReturnCode.Success, "Google two factor binding successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Remove bind google factor
        /// </summary>
        /// <param name="idReq"></param>
        /// <returns></returns>
        [HttpPost("RemoveBindGoogleFactor")]
        [TokenCheck]
        public IActionResult RemoveBindGoogleFactor([FromBody] IdReq idReq)
        {
            try
            {
                var reply = Client.RemoveBindGoogleFactor(idReq);
                return Json(new JsonResultModel(ReturnCode.Success, "Google two factor removing binding successful."));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        private bool IsEnableInviteCode()
        {
            string msg = "";
            ParseConfig.ParseInfo("EnableInviteCode", out msg);
            bool result = Convert.ToBoolean(msg);

            return result;
        }

        private string MsgContent(string phoneNo, string code)
        {
            if (phoneNo.StartsWith("86") || phoneNo.StartsWith("+86"))
                return $"尊敬的Coindaq用户，您申请的验证码为：{code}，请勿泄漏于他人！【Coindaq】";

            return $"[Coindaq]: Your verification code request from Coindaq is : {code}";
        }
    }
}
