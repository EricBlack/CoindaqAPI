/**************************************************************
 *  Filename:      TiantianMsg
 *
 *  Description:  TiantianMsg ClassFile.
 *
 *  Company:     Coindaq 
 *
 *  @Author:      ChaoShu
 *
 *  @Created:    2018/5/15/周二 16:01:34 
 **************************************************************/

using Grpc.Core;
using Microsoft.Extensions.Logging;
using Proto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Proto.UserService;

namespace CoindaqAPI.Utils.MsgServices
{
    public class TiantianMsg
    {
        public static MsgConfig Config { get; set; }

        public MsgConfig MsgConfig { get; set; }
        public UserServiceClient Client { get; set; }

        private readonly ILogger logger;
        public TiantianMsg(UserServiceClient client)
        {
            this.MsgConfig = Config;
            this.MsgConfig.Format = "src={0}&pwd={1}&ServiceID=SEND&dest={2}&sender={3}&msg={4}&codec=8";
            Client = client;

            logger = new Logger<TiantianMsg>(new LoggerFactory().AddConsole());
        }

        public bool SendMessage(string dest, string message, out string returnCode, out string returnMsg)
        {
            if (MsgConfig.EnableMsg == false)
            {
                returnCode = "Message_Disable";
                returnMsg = "Message service is not enable.";
                return true;
            }
            //验证是否超限
            try
            {
                var reply = Client.MessageInHour(new ForgetPhoneReq() { Phone = dest });
                if(reply.Count >= MsgConfig.MsgLimit)
                {
                    returnCode = "Message_Limit";
                    returnMsg = "Message sent in an hour is over limited.";
                    return false;
                }
            }
            catch (RpcException ex)
            {
                //Console.WriteLine("Query email limit status got exception.");
                logger.LogError("Query email limit status got exception.");
                returnCode = "RPC_Error";
                returnMsg = $"Query email limit status got exception:{ex.Message}";
                return false;
            }

            MsgInfo info = new MsgInfo(dest, message);

            return SendMessage(info, out returnCode, out returnMsg);
        }

        private bool SendMessage(MsgInfo msgInfo, out string code, out string message)
        {
            string hexMsg = EncodeHexStr(8, msgInfo.Msg);
            string postString = string.Format(MsgConfig.Format, new string[] { MsgConfig.Account, MsgConfig.Password, msgInfo.Dest, MsgConfig.Sender, hexMsg });
            byte[] postData = System.Text.Encoding.ASCII.GetBytes(postString);
            string rs = DoPostRequest(MsgConfig.ServiceUrl, postData);

            code = MessageCode(rs);
            message = MessageMessage(rs);

            if (Convert.ToInt64(rs) >0)
                return true;
            return false;
        }

        //字符编码成HEX
        private static String EncodeHexStr(int dataCoding, String realStr)
        {
            string strhex = "";
            try
            {
                Byte[] bytSource = null;
                if (dataCoding == 15)
                {
                    bytSource = Encoding.GetEncoding("GBK").GetBytes(realStr);
                }
                else if (dataCoding == 3)
                {
                    bytSource = Encoding.GetEncoding("ISO-8859-1").GetBytes(realStr);
                }
                else if (dataCoding == 8)
                {
                    bytSource = Encoding.BigEndianUnicode.GetBytes(realStr);
                }
                else
                {
                    bytSource = Encoding.ASCII.GetBytes(realStr);
                }
                for (int i = 0; i < bytSource.Length; i++)
                {
                    strhex = strhex + bytSource[i].ToString("X2");

                }
            }
            catch (System.Exception)
            {
                return realStr;
            }
            return strhex;
        }
        //hex编码还原成字符
        private static String DecodeHexStr(int dataCoding, String hexStr)
        {
            String strReturn = "";
            try
            {
                int len = hexStr.Length / 2;
                byte[] bytSrc = new byte[len];
                for (int i = 0; i < len; i++)
                {
                    string s = hexStr.Substring(i * 2, 2);
                    bytSrc[i] = Byte.Parse(s, System.Globalization.NumberStyles.AllowHexSpecifier);
                }

                if (dataCoding == 15)
                {
                    strReturn = Encoding.GetEncoding("GBK").GetString(bytSrc);
                }
                else if (dataCoding == 3)
                {
                    strReturn = Encoding.GetEncoding("ISO-8859-1").GetString(bytSrc);
                }
                else if (dataCoding == 8)
                {
                    strReturn = Encoding.BigEndianUnicode.GetString(bytSrc);
                }
                else
                {
                    strReturn = System.Text.ASCIIEncoding.ASCII.GetString(bytSrc);
                }
            }
            catch (System.Exception)
            {
                return hexStr;
            }
            return strReturn;
        }
        //POST方式发送得结果
        private static String DoPostRequest(string url, byte[] bData)
        {
            System.Net.HttpWebRequest hwRequest;
            System.Net.HttpWebResponse hwResponse;

            string strResult = string.Empty;
            try
            {
                hwRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                hwRequest.Timeout = 30000;
                hwRequest.Method = "POST";
                hwRequest.ContentType = "application/x-www-form-urlencoded";
                hwRequest.ContentLength = bData.Length;

                System.IO.Stream smWrite = hwRequest.GetRequestStream();
                smWrite.Write(bData, 0, bData.Length);
                smWrite.Close();
            }
            catch (System.Exception err)
            {
                WriteErrLog(err.ToString());
                return strResult;
            }

            //get response
            try
            {
                hwResponse = (HttpWebResponse)hwRequest.GetResponse();
                StreamReader srReader = new StreamReader(hwResponse.GetResponseStream(), Encoding.ASCII);
                strResult = srReader.ReadToEnd();
                srReader.Close();
                hwResponse.Close();
            }
            catch (System.Exception err)
            {
                WriteErrLog(err.ToString());
            }

            return strResult;
        }
        //GET方式发送得结果
        private static String DoGetRequest(string url)
        {
            HttpWebRequest hwRequest;
            HttpWebResponse hwResponse;

            string strResult = string.Empty;
            try
            {
                hwRequest = (System.Net.HttpWebRequest)WebRequest.Create(url);
                hwRequest.Timeout = 30000;
                hwRequest.Method = "GET";
                hwRequest.ContentType = "application/x-www-form-urlencoded";
            }
            catch (System.Exception err)
            {
                WriteErrLog(err.ToString());
                return strResult;
            }
            //get response
            try
            {
                hwResponse = (HttpWebResponse)hwRequest.GetResponse();
                StreamReader srReader = new StreamReader(hwResponse.GetResponseStream(), Encoding.ASCII);
                strResult = srReader.ReadToEnd();
                srReader.Close();
                hwResponse.Close();
            }
            catch (System.Exception err)
            {
                WriteErrLog(err.ToString());
            }

            return strResult;
        }
        private static void WriteErrLog(string strErr)
        {
            Console.WriteLine(strErr);
            System.Diagnostics.Trace.WriteLine(strErr);
        }
        private string MessageCode(string code)
        {
            string message;
            if(Convert.ToInt64(code) > 0)
            {
                message = "SEND_SUC";
                return message;
            }
            switch (code)
            {
                case "-01":
                    message = "SEND_ERROR";
                    break;
                case "-02":
                    message = "NOT_ENOUGHCREDITS";
                    break;
                case "-03":
                    message = "ACCOUNT_BLOCKED";
                    break;
                case "-04":
                    message = "NETWORK_NOTCOVERED";
                    break;
                case "-05":
                    message = "DEST_NUMBER_EXCEED_MAX";
                    break;
                case "-06":
                    message = "INVALID_USER_OR_PASS";
                    break;
                case "-07":
                    message = "MISSING_DESTINATION_ADDRESS";
                    break;
                case "-08":
                    message = "MISSING_SMSTEXT";
                    break;
                case "-09":
                    message = "MISSING_SENDERNAME";
                    break;
                case "-10":
                    message = "DEST_INVALIDFORMAT";
                    break;
                case "-11":
                    message = "MISSING_USERNAME";
                    break;
                case "-12":
                    message = "MISSING_PASS";
                    break;
                case "-13":
                    message = "NETWORK_FAIL";
                    break;
                case "-14":
                    message = "INTERAL_ERROR";
                    break;
                case "-15":
                    message = "INVALID_DESTINATION_ADDRESS";
                    break;
                case "-16":
                    message = "INVALID_SMS_MAX_LENGTH";
                    break;
                case "-17":
                    message = "BLACKWORD_IN_SMS";
                    break;
                case "-18":
                    message = "DEST_IN_BLACKLIST";
                    break;
                case "-19":
                    message = "INVALID_DCS";
                    break;
                case "-20":
                    message = "INTERAL_ERROR_SERVICE_NO_START";
                    break;
                default:
                    message = "Unknown Issue";
                    break;
            }
            return message;
        }
        private string MessageMessage(string code)
        {
            string message;
            if (Convert.ToInt64(code) > 0)
            {
                message = "Message send successful.";
                return message;
            }
            switch (code)
            {
                case "-01":
                    message = "系统维护中,请联系客服";
                    break;
                case "-02":
                    message = "当前账号余额不足";
                    break;
                case "-03":
                    message = "帐号停止";
                    break;
                case "-04":
                    message = "目的号码运营商不在服务覆盖范围";
                    break;
                case "-05":
                    message = "目的手机号码数量超长（30/次，超30个请自行做循环）";
                    break;
                case "-06":
                    message = "用户或密码错误";
                    break;
                case "-07":
                    message = "目的号码不能为空";
                    break;
                case "-08":
                    message = "短信内容不能为空";
                    break;
                case "-09":
                    message = "源号码不能为空";
                    break;
                case "-10":
                    message = "DEST参数格式错误";
                    break;
                case "-11":
                    message = "用户名空";
                    break;
                case "-12":
                    message = "密码空";
                    break;
                case "-13":
                    message = "网络错误";
                    break;
                case "-14":
                    message = "内部错误";
                    break;
                case "-15":
                    message = "非法手机号码，手机号码格式不对";
                    break;
                case "-16":
                    message = "短信内容超长！（UNICODE最大70个字符，Alphabet编码（英文即以此方式传输）最大160字符）";
                    break;
                case "-17":
                    message = "短信内容含有非法字符";
                    break;
                case "-18":
                    message = "目的手机号码限制";
                    break;
                case "-19":
                    message = "短信内容编码不对（比如发中文、韩文、日文而用Alphabet编码方式）";
                    break;
                case "-20":
                    message = "预处理服务没有启动";
                    break;
                default:
                    message = "未知错误";
                    break;
            }
            return message;
        }

        const string SEND_SUC = "0"; // 发送成功
        const string SEND_ERROR = "-01"; // 系统维护中,请联系客服
        const string NOT_ENOUGHCREDITS = "-02"; // 当前账号余额不足
        const string ACCOUNT_BLOCKED = "-03"; // 帐号停止
        const string NETWORK_NOTCOVERED = "-04"; // 目的号码运营商不在服务覆盖范围
        const string DEST_NUMBER_EXCEED_MAX = "-05"; // 目的手机号码数量超长（30/次，超30个请自行做循环）
        const string INVALID_USER_OR_PASS = "-06"; // 用户或密码错误
        const string MISSING_DESTINATION_ADDRESS = "-07"; // 目的号码不能为空
        const string MISSING_SMSTEXT = "-08"; // 短信内容不能为空
        const string MISSING_SENDERNAME = "-09"; //源号码不能为空
        const string DEST_INVALIDFORMAT = "-10"; // DEST参数格式错误
        const string MISSING_USERNAME = "-11"; // 用户名空
        const string MISSING_PASS = "-12"; // 密码空
        const string NETWORK_FAIL = "-13"; // 网络错误
        const string INTERAL_ERROR = "-14"; // 内部错误
        const string INVALID_DESTINATION_ADDRESS = "-15"; // 非法手机号码，手机号码格式不对
        const string INVALID_SMS_MAX_LENGTH = "-16"; // 短信内容超长！（UNICODE最大70个字符，Alphabet编码（英文即以此方式传输）最大160字符）
        const string BLACKWORD_IN_SMS = "-17"; // 短信内容含有非法字符
        const string DEST_IN_BLACKLIST = "-18"; //目的手机号码限制
        const string INVALID_DCS = "-19"; // 短信内容编码不对（比如发中文、韩文、日文而用Alphabet编码方式）
        const string INTERAL_ERROR_SERVICE_NO_START = "-20"; // 预处理服务没有启动
    }
}
