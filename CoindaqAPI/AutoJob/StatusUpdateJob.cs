/**************************************************************
 *  Filename:      StatusUpdateJob
 *
 *  Description:  StatusUpdateJob ClassFile.
 *
 *  Company:     Coindaq 
 *
 *  @Author:      ChaoShu
 *
 *  @Created:    2018/6/5/周二 18:58:09 
 **************************************************************/

using CoindaqAPI.Utils;
using CoindaqAPI.Utils.FiatCurrency;
using CoindaqAPI.Utils.Redis;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Pomelo.AspNetCore.TimedJob;
using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoindaqAPI.AutoJob
{
    public class StatusUpdateJob : Job
    {
        private readonly ILogger logger;
        public Channel Channel { get; set; }
        public AutoJobService.AutoJobServiceClient Client { get; set; }
        public StatusUpdateJob()
        {
            logger = new Logger<CurrencyJob>(new LoggerFactory().AddConsole());
            string value = "";
            bool result = ParseConfig.ParseInfo("ServicesUrl\\ProjectService", out value);
            if (result)
            {
                var baseService = new BaseService(value);
                Channel = baseService.Channel;
                Client = new AutoJobService.AutoJobServiceClient(Channel);
            }
            else
                Channel = null;
        }

        [Invoke(Begin = "2018-05-21 01:00", Interval = 1000 * 60, SkipWhileExecuting = true)]
        public void UpdateProjectStatus(IServiceProvider services)
        {
            try
            {
                Client.UpdateProjectsStatus(new Empty { });
            }
            catch (RpcException ex)
            {
                logger.LogError(ex.Status.Detail);
            }
        }

        [Invoke(Begin = "2018-05-21 02:00", Interval = 1000 * 60, SkipWhileExecuting = true)]
        public void UpdateStageStatus(IServiceProvider services)
        {
            try
            {
                Client.UpdateStagesStatus(new Empty { });
            }
            catch (RpcException ex)
            {
                logger.LogError(ex.Status.Detail);
            }
        }

        [Invoke(Begin = "2018-05-21 03:00", Interval = 1000 * 60, SkipWhileExecuting = true)]
        public void UpdateOrderDetailsStatus(IServiceProvider services)
        {
            try
            {
                Client.UpdateOtcDetailsStatus(new Empty { });
            }
            catch (RpcException ex)
            {
                logger.LogError(ex.Status.Detail);
            }
        }
    }
}
