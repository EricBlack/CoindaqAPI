/**************************************************************
 *  Filename:      BaseService
 *
 *  Description:  BaseService ClassFile.
 *
 *  Company:     Coindaq 
 *
 *  @Author:      ChaoShu
 *
 *  @Created:    2018/5/11/周五 13:27:07 
 **************************************************************/

using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoindaqAPI.Utils
{
    public class BaseService
    {
        public string ServiceUrl { get; set; }

        public Channel Channel { get; set; }

        public BaseService(string serviceUrl)
        {
            ServiceUrl = serviceUrl;
            var options = new List<ChannelOption>
            {
                new ChannelOption("grpc.initial_reconnect_backoff_ms", 100),
            };
            Channel = new Channel(ServiceUrl, ChannelCredentials.Insecure, options);
        }

        public static void CheckChannelStatus(Channel channel)
        {
            if (channel.State == ChannelState.TransientFailure)
            {
                var task = channel.ConnectAsync();
                task.Wait();
            }
        }

    }
}
