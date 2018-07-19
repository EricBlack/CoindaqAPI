/**************************************************************
 *  Filename:      UploadInfo
 *
 *  Description:  UploadInfo ClassFile.
 *
 *  Company:     Coindaq 
 *
 *  @Author:      ChaoShu
 *
 *  @Created:    2018/5/16/周三 14:19:43 
 **************************************************************/

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoindaqAPI.Utils
{
    public class UploadInfo
    {
        public string Directory { get; set; }
        public int MaxSize { get; set; } //KB为单位
        public List<string> SuffixLimit { get; set; } //文件上传后缀限制

        private readonly ILogger logger;
        public UploadInfo()
        {
            Directory = GetUploaddir();
            MaxSize = GetSizeLimit();
            SuffixLimit = GetSuffixLimt();

            logger = new Logger<UploadInfo>(new LoggerFactory().AddConsole());
        }

        private string GetUploaddir()
        {
            string defaultPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "UploadFiles");
            string settingPath = "";
            bool result = ParseConfig.ParseInfo("UploadConfig\\Path", out settingPath);
            if (result && settingPath != "")
            {
                if (System.IO.Directory.Exists(settingPath))
                    //Console.WriteLine("Setting upload directory is not exist, use default instead.");
                    logger.LogInformation("Setting upload directory is not exist, use default instead.");
                else
                    return settingPath;
            }
            return defaultPath;
        }

        private int GetSizeLimit()
        {
            string fileSize;
            bool result = ParseConfig.ParseInfo("UploadConfig\\MaxSize", out fileSize);
            if (result)
                return int.Parse(fileSize);

            return 4000000;
        }

        private List<string> GetSuffixLimt()
        {
            string suffixInfo;
            bool result = ParseConfig.ParseInfo("UploadConfig\\SuffixLimit", out suffixInfo);
            if (result)
                return suffixInfo.Replace(" ", "").Split(",").ToList();

            return new List<string>();
        }

        public bool IsFormatCorrect(int fileSize, string suffix, out string message)
        {
            message = "";

            if (fileSize >= MaxSize)
            {
                message = "文件上传超过限制";
                return false;
            }
            if (SuffixLimit.Contains(suffix.ToLower()))
            {
                message = "文件上传格式错误";
                return false;
            }

            return true;
        }
    }
}
