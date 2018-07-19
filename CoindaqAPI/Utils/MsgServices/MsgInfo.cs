/**************************************************************
 *  Filename:      MsgInfo
 *
 *  Description:  MsgInfo ClassFile.
 *
 *  Company:     Coindaq 
 *
 *  @Author:      ChaoShu
 *
 *  @Created:    2018/5/15/周二 16:03:48 
 **************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoindaqAPI.Utils.MsgServices
{
    public class MsgInfo
    {
        public string Dest { get; set; }
        public string Msg { get; set; }
        public string Skey { get; set; }

        public MsgInfo(string dest, string msg)
        {
            Dest = dest;
            Msg = msg;
            Skey = "12345678";
        }

        public MsgInfo(string dest, string msg, string skey)
        {
            Dest = dest;
            Msg = msg;
            Skey = skey;
        }
    }
}
