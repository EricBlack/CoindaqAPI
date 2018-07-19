using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CoindaqAPI.Utils.MsgServices;
using CoindaqAPI.Utils;
using Microsoft.Extensions.FileProviders;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System;

namespace CoindaqAPI
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("Config/appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // 将配置文件作为单例注入容器
            services.AddSingleton(Configuration);

            //配置任务Job
            services.AddTimedJob();

            //Add session service  
            services.AddSession(o => {
                o.IdleTimeout = TimeSpan.FromHours(2);
            });

            //Add Tiantian Msg
            TiantianMsg.Config = MsgConfig.GetMsgConfig();
            services.AddScoped<TiantianMsg>();

            // Add framework services.
            services.AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver
                = new Newtonsoft.Json.Serialization.DefaultContractResolver());//JSON首字母小写解决;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            //使用默认静态目录
            app.UseDefaultFiles();
            app.UseStaticFiles();

            //设置上传文件访问路径
            var info = new UploadInfo();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(info.Directory),
                RequestPath = "/StaticFiles",
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=600");
                }
            });

            //设置二维码文件访问路径
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "QRCodeFiles")),
                RequestPath = "/QrCode",
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=600");
                }
            });
            
            //使用任务Job
            app.UseTimedJob();
            
            //使用session  
            app.UseSession();
            //允许跨域请求
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());

            app.UseMvc();
        }
    }
}
