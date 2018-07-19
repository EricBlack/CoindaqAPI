/**************************************************************
 *  Filename:      JsonResultModel
 *
 *  Description:  JsonResultModel ClassFile.
 *
 *  Company:     Coindaq 
 *
 *  @Author:      ChaoShu
 *
 *  @Created:    2018/5/11/周五 12:32:35 
 **************************************************************/

using CoindaqAPI.Utils.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoindaqAPI.Utils
{
    public class JsonResultModel
    {
        [JsonProperty("code")]
        public ReturnCode Code { get; set; }

        [JsonProperty("message")]
        public string Msg { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        public JsonResultModel(ReturnCode code, string message)
        {
            this.Code = code;
            this.Msg = message;
            this.Data = null;
        }

        public JsonResultModel(ReturnCode code, string message, object data)
        {
            this.Code = code;
            this.Msg = message;
            this.Data = data;
        }

    }

    public class JsonResultModel<T>
    {
        [JsonProperty("code")]
        public ReturnCode Code { get; set; }

        [JsonProperty("message")]
        public string Msg { get; set; }

        [JsonProperty("data")]
        public T Data { get; set; }

        public JsonResultModel(ReturnCode code, string message)
        {
            this.Code = code;
            this.Msg = message;
        }

        public JsonResultModel(ReturnCode code, string message, T data)
        {
            this.Code = code;
            this.Msg = message;
            this.Data = data;
        }
    }

    public enum ReturnCode
    {
        Success = 9000,
        LoginTimeOut = 9001,
        ParameterError = 9010,
        AccountExist = 9011,
        AddressExist = 9012,
        AuthorizationError = 9020,
        EndPointError = 9021,
        TokenError = 9022,
        QueryError = 9030,
        SubmitError = 9040,
        DBError = 9050,
        RedisError = 9055,
        PasswordError = 9060,
        ConnectionError = 9070,
        MessageError = 9080,
        UnknownError = 9100
    }
}
