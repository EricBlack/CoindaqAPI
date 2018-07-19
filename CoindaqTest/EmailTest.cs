/**************************************************************
 *  Filename:      EmailTest
 *
 *  Description: EmailTest ClassFile.
 *
 *  Company:     Coindaq 
 *
 *  @Author:      ChaoShu
 *  @Created:    2018/5/21/周一 10:14:42 
 **************************************************************/

using CoindaqAPI.Utils.FiatCurrency;
using CoindaqAPI.Utils.MsgServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using System.Net.Mail;
using System;
using System.Text;

namespace CoindaqTest
{
    [TestClass]
    public class EmailTest
    {
        [TestMethod]
        public void SendEmail()
        {
            System.Net.Mail.SmtpClient _smtpClient = new System.Net.Mail.SmtpClient();
            _smtpClient.EnableSsl = true;
            _smtpClient.UseDefaultCredentials = false;
            _smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            _smtpClient.Host = "smtp.qq.com";
            _smtpClient.Port = 587;
            _smtpClient.Credentials = new System.Net.NetworkCredential("blackeye286@qq.com", "yqhlkdmyosrxbjfd");
            //密码不是QQ密码，是qq账户设置里面的POP3/SMTP服务生成的key

            MailMessage _mailMessage = new MailMessage("blackeye286@qq.com", "blackeye286@126.com");
            _mailMessage.Subject = "Test information";//主题  
            _mailMessage.Body = "Ok test";//内容
            _mailMessage.BodyEncoding = Encoding.Default;//正文编码  
            _mailMessage.IsBodyHtml = true;//设置为HTML格式  
            _mailMessage.Priority = MailPriority.High;//优先级  

            try
            {
                _smtpClient.Send(_mailMessage);
                Console.WriteLine("发送成功");
            }
            catch (Exception e)
            {
                Console.WriteLine("发送失败");
                throw e;
            }
        }

        [TestMethod]
        public void SendEmailThread()
        {
            string emailMessage = "";
            bool send = EmailEvent.RegisterEmail("blackeye286@126.com", 38, "32425678", out emailMessage);
        }

        [TestMethod]
        public void SendEmailKit()
        {
            MailMessage mail = new MailMessage("blackeye286@126.com", "blackeye286@qq.com", "Test info", "sghjkhvcdsacscdcda");
            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient("smtp.126.com");
            client.Port = 587;
            client.Credentials = new System.Net.NetworkCredential("blackeye286@126.com", "blackeye286@126.com");
            client.EnableSsl = true;
            client.Send(mail);
            //MessageBox.Show("Mail Sent !", "Success", MessageBoxButtons.OK);
            Console.WriteLine("Success!");
        }

    }
}
