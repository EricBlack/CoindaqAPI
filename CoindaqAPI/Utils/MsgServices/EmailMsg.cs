/**************************************************************
 *  Filename:      EmailMsg
 *
 *  Description:  EmailMsg ClassFile.
 *
 *  Company:     Coindaq 
 *
 *  @Author:      ChaoShu
 *
 *  @Created:    2018/5/17/周四 16:37:14 
 **************************************************************/

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoindaqAPI.Utils.MsgServices
{
    public class EmailMsg
    {
        #region [ 属性(Emai配置) ]  
        private string _smtpHost = string.Empty;
        private int _smtpPort = -1;
        private string _fromAddress = string.Empty;
        private string _fromPassword = string.Empty;
        private string _fromDisplayName = string.Empty;

        private readonly ILogger logger;

        /// <summary>  
        /// smtp 服务器   
        /// </summary>  
        public string SmtpHost
        {
            get
            {
                if (string.IsNullOrEmpty(_smtpHost))
                {
                    ParseConfig.ParseInfo("EmailConfig\\SmtpHost", out _smtpHost);
                }
                return _smtpHost;
            }
        }

        /// <summary>  
        /// smtp 服务器端口  默认为25  
        /// </summary>  
        public int SmtpPort
        {
            get
            {
                if (_smtpPort == -1)
                {
                    string portInfo;
                    ParseConfig.ParseInfo("EmailConfig\\SmtpPort", out portInfo);
                    _smtpPort = Int32.Parse(portInfo);
                }
                return _smtpPort;
            }
        }

        /// <summary>  
        /// 发送者 Eamil 地址  
        /// </summary>  
        public string FromEmailAddress
        {
            get
            {
                if (string.IsNullOrEmpty(_fromAddress))
                {
                    ParseConfig.ParseInfo("EmailConfig\\FromAddress", out _fromAddress);
                }
                return _fromAddress;
            }
        }

        /// <summary>  
        /// 发送者 Eamil 密码  
        /// </summary>  
        public string FromEmailPassword
        {
            get
            {
                if (string.IsNullOrEmpty(_fromPassword))
                {
                    ParseConfig.ParseInfo("EmailConfig\\EmailPassword", out _fromPassword);
                }

                return _fromPassword;
            }
        }

        /// <summary>
        /// 发送邮件配置信息
        /// </summary>
        public string FromDisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(_fromDisplayName))
                {
                    ParseConfig.ParseInfo("EmailConfig\\DisplayName", out _fromDisplayName);
                }
                return _fromDisplayName;
            }
        }
        #endregion

        #region [ 属性(Email相关) ]  
        public List<MailAddress> ToList { get; set; }
        public MailAddress To { get; set; }
        public MailAddress From { get; set; }
        public Encoding BodyEncoding { get; set; }
        public string Body { get; set; }
        public Encoding SubjectEncoding { get; set; }
        public string Subject { get; set; }
        public bool IsBodyHtml { get; set; }
        public List<Attachment> AttachmentList { get; set; }
        public bool IsEnableSSL { get; set; }
        #endregion

        public EmailMsg(string toUser, string subject, string body, List<Attachment> attachments = null, bool isBodyHtml = true, bool isEnableSsl = false)
        {
            To = new MailAddress(toUser);
            From = new MailAddress(FromEmailAddress, FromDisplayName);
            Subject = subject;
            SubjectEncoding = Encoding.UTF8;
            BodyEncoding = Encoding.UTF8;
            Body = body;
            AttachmentList = attachments;
            IsBodyHtml = isBodyHtml;
            IsEnableSSL = isEnableSsl;

            logger = new Logger<EmailMsg>(new LoggerFactory().AddConsole());
        }

        /// <summary>
        /// 同步发送邮件
        /// </summary>
        /// <returns>是否成功</returns>
        public bool Send(out string msg)
        {
            try
            {
                MailMessage message = new MailMessage(From, To);
                // 接收人邮箱地址
                //message.To.Add(To);
                //message.From = From;
                message.BodyEncoding = BodyEncoding;
                message.Body = Body;
                //GB2312
                message.SubjectEncoding = SubjectEncoding;
                message.Subject = Subject;
                message.IsBodyHtml = IsBodyHtml;
                message.Priority = MailPriority.High;

                //添加附件
                if (AttachmentList !=null && AttachmentList.Count > 0)
                {
                    foreach (Attachment attachment in this.AttachmentList)
                    {
                        message.Attachments.Add(attachment);
                    }
                }

                SmtpClient smtpclient = new SmtpClient();
                //SSL连接
                smtpclient.EnableSsl = true;
                smtpclient.UseDefaultCredentials = false;
                smtpclient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpclient.Host = SmtpHost;
                smtpclient.Port = SmtpPort;
                smtpclient.Credentials = new System.Net.NetworkCredential(FromEmailAddress, FromEmailPassword);
                smtpclient.Timeout = 20000;

                smtpclient.Send(message);

                msg = "Email send success.";
                logger.LogInformation("Complete send email.");

                return true;
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Email send failed: " + ex.Message);
                logger.LogError("Email send failed: " + ex.Message);
                msg = ex.Message;

                return false;
            }
        }

        /// <summary>
        /// 异步发送邮件 独立线程
        /// </summary>
        /// <returns></returns>
        public void SendByThread()
        {
            new Thread(new ThreadStart(delegate ()
            {
                try
                {
                    SmtpClient smtp = new SmtpClient();
                    //邮箱的smtp地址
                    smtp.Host = SmtpHost;
                    //端口号
                    smtp.Port = SmtpPort;
                    //构建发件人的身份凭据类
                    smtp.Credentials = new NetworkCredential(FromEmailAddress, FromEmailPassword);
                    //构建消息类
                    MailMessage objMailMessage = new MailMessage();
                    //设置优先级
                    objMailMessage.Priority = MailPriority.High;
                    //消息发送人
                    objMailMessage.From = new MailAddress(FromEmailAddress, FromDisplayName, Encoding.UTF8);
                    //收件人
                    objMailMessage.To.Add(To);
                    //标题
                    objMailMessage.Subject =Subject;
                    //标题字符编码
                    objMailMessage.SubjectEncoding = SubjectEncoding;
                    //正文
                    objMailMessage.Body = Body;
                    objMailMessage.IsBodyHtml = IsBodyHtml;
                    //内容字符编码
                    objMailMessage.BodyEncoding =BodyEncoding;
                    //是否包含附件
                    if (AttachmentList != null && AttachmentList.Count > 0)
                    {
                        foreach (Attachment attachment in this.AttachmentList)
                        {
                            objMailMessage.Attachments.Add(attachment);
                        }
                    }
                    //发送
                    smtp.Send(objMailMessage);
                }
                catch (Exception ex)
                {
                    //Console.WriteLine("Email send failed: " + ex.Message);
                    logger.LogError("Email send failed: " + ex.Message);
                }
            })).Start();
        }

        public void SendEmailByThread()
        {
            new Thread(new ThreadStart(delegate ()
            {
                try
                {
                    string message = "";
                    Send(out message);
                }
                catch (Exception ex)
                {
                    //Console.WriteLine("Email send failed: " + ex.Message);
                    logger.LogError("Email send failed: " + ex.Message);
                }
            })).Start();
        }

    }
}
