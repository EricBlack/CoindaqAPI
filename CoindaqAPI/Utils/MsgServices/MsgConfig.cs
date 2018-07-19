/**************************************************************
 *  Filename:      MsgConfig
 *
 *  Description:  MsgConfig ClassFile.
 *
 *  Company:     Coindaq 
 *
 *  @Author:      ChaoShu
 *
 *  @Created:    2018/5/15/周二 16:04:25 
 **************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoindaqAPI.Utils.MsgServices
{
    public class MsgConfig
    {
        public string ServiceUrl { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string Sender { get; set; }
        public string Format { get; set; }
        public bool EnableMsg { get; set; }
        public int MsgLimit { get; set; }

        public static MsgConfig GetMsgConfig()
        {
            MsgConfig config = new MsgConfig();
            string message = "";
            bool result = false;
            //Enable Info
            if (result = ParseConfig.ParseInfo("MsgConfig\\Enable", out message))
            {
                //config.EnableMsg = (message.Trim().ToLower() =="true") ? true : false;
                config.EnableMsg = Convert.ToBoolean(message);
            }
            else
            {
                config.EnableMsg = false;
            }
            //Url Info
            if (result = ParseConfig.ParseInfo("MsgConfig\\MsgUrl", out message))
            {
                config.ServiceUrl = message;
            }
            //Account Info
            if (result = ParseConfig.ParseInfo("MsgConfig\\Account", out message))
            {
                config.Account = message;
            }
            //Password Info
            if (result = ParseConfig.ParseInfo("MsgConfig\\Password", out message))
            {
                config.Password = message;
            }
            //Sender Info
            if (result = ParseConfig.ParseInfo("MsgConfig\\Sender", out message))
            {
                config.Sender = message;
            }
            //Message Count Limit
            if (result = ParseConfig.ParseInfo("MsgConfig\\MsgLimit", out message))
            {
                config.MsgLimit = Int32.Parse(message);
            }

            return config;
        }
    }
}
