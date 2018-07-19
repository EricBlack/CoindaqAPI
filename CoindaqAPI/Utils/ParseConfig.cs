/**************************************************************
 *  Filename:      ParseConfig
 *
 *  Description:  ParseConfig ClassFile.
 *
 *  Company:     Coindaq 
 *
 *  @Author:      ChaoShu
 *
 *  @Created:    2018/5/11/周五 13:44:46 
 **************************************************************/

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoindaqAPI.Utils
{
    public class ParseConfig
    {
        public static string ConfigString { get; set; }

        public static void GetConfigString()
        {
            ConfigString = File.ReadAllText(@"Config/appsettings.json");
        }

        public static bool ParseInfo(string configPath, out string stringValue)
        {
            if (ConfigString == null)
                GetConfigString();

            string configInfo = ConfigString;
            string[] pathList = configPath.Split('\\');
            stringValue = "";

            foreach (var item in pathList)
            {
                JObject config = JObject.Parse(configInfo);

                if (config.ToString().Contains(item))
                {
                    stringValue = config[item].ToString();
                    configInfo = stringValue;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

    }
}
