using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CoindaqAPI.Utils;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace CoindaqAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string platform = "";
            string portStr = "";

            bool platformResult = ParseConfig.ParseInfo("DeploySystem\\Platform", out platform);
            bool portResult = ParseConfig.ParseInfo("DeploySystem\\Port", out portStr);
            var logger = new Logger<Program>(new LoggerFactory().AddConsole());
            if (platformResult && portResult)
            {
                IWebHost host;
                if ("WIN".Equals(platform))
                {
                    host = new WebHostBuilder()
                        .UseKestrel()
                        .UseContentRoot(Directory.GetCurrentDirectory())
                        .UseIISIntegration()
                        .UseStartup<Startup>()
                        .UseApplicationInsights()
                        .Build();

                    //Console.WriteLine("Api service is launched at Windows. ");
                    logger.LogInformation("Api service is launched at Windows.");
                }
                else if ("LINUX".Equals(platform))
                {
                    if (ValidateHelper.IsInt32Value(portStr))
                    {
                        int port = int.Parse(portStr);
                        port = port == 0 ? 80 : port;
                        host = new WebHostBuilder()
                            .UseKestrel()
                            .UseContentRoot(Directory.GetCurrentDirectory())
                            .UseStartup<Startup>()
                            .UseUrls("http://0.0.0.0:" + port)
                            .UseApplicationInsights()
                            .Build();

                        //Console.WriteLine("Api service is launched at Linux. ");
                        logger.LogInformation("Api service is launched at Linux.");
                    }
                    else
                    {
                        //Console.Error.WriteLine("Server config error: port config error.");
                        logger.LogError("Server config error: port config error.");
                        return;
                    }
                }
                else
                {
                    //Console.Error.WriteLine("Server config error: unsupported platform config.");
                    logger.LogError("Server config error: unsupported platform config.");
                    return;
                }
                host.Run();
            }
            else
            {
                //Console.Error.WriteLine("Server config error: without platform config.");
                logger.LogError("Server config error: without platform config.");
                return;
            }
        }
    }
}