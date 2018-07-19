/**************************************************************
 *  Filename:      TwoFactorController
 *
 *  Description:  TwoFactorController ClassFile.
 *
 *  Company:     Coindaq 
 *
 *  @Author:      ChaoShu
 *
 *  @Created:    2018/5/11/周五 19:39:19 
 **************************************************************/

using CoindaqAPI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Proto;
using Microsoft.AspNetCore.Mvc;
using CoindaqAPI.Utils.Filter;
using Grpc.Core;
using CoindaqAPI.Utils.MsgServices;

namespace CoindaqAPI.Controllers
{
    //[Produces("application/json")]
    [Route("/api/v1/[controller]")]
    [Exception]
    public class TwoFactorController :BaseController, IDisposable
    {
        public TwoFactorService.TwoFactorServiceClient Client { get; set; }
        public TiantianMsg MsgService { get; set; }
        public TwoFactorController() :
            base("ServicesUrl\\UserService")
        {
            Client = new TwoFactorService.TwoFactorServiceClient(Channel);
            var userClient = new UserService.UserServiceClient(Channel);
            MsgService = new TiantianMsg(userClient);
        }

        /// <summary>
        /// Generate factor info
        /// </summary>
        /// <param name="factor"></param>
        /// <returns></returns>
        [HttpPost("GenerateFactor")]
        public IActionResult GenerateFactor([FromBody] FactorReq factor)
        {
            try
            {
                var reply = Client.GenerateFactor(factor);

                return Json(new JsonResultModel(ReturnCode.Success, "Generate two factor information successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Query user factor info
        /// </summary>
        /// <param name="factor"></param>
        /// <returns></returns>
        [HttpGet("QueryFactor")]
        [TokenCheck]
        public IActionResult QueryFactor([FromQuery] FactorReq factor)
        {
            try
            {
                var reply = Client.QueryFactor(factor);
                if (reply.Id == 0)
                    reply = null;

                return Json(new JsonResultModel(ReturnCode.Success, "Query successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.QueryError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Refresh factor info
        /// </summary>
        /// <param name="factor"></param>
        /// <returns></returns>
        [HttpPost("RefreshFactor")]
        [TokenCheck]
        public IActionResult RefreshFactor([FromBody] FactorReq factor)
        {
            try
            {
                var reply = Client.RefreshFactor(factor);
                return Json(new JsonResultModel(ReturnCode.Success, "Two factor information refreshed successful."));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Verify factor info
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost("VerifyFactorCode")]
        public IActionResult VerifyFactorCode([FromBody] InfoReq info)
        {
            try
            {
                var reply = Client.VerifyFactorCode(info);
                return Json(new JsonResultModel(ReturnCode.Success, "Two factor information verfied successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.QueryError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Verify user factors
        /// </summary>
        /// <param name="factorList"></param>
        /// <returns></returns>
        [HttpPost("VerifyUserFactors")]
        public IActionResult VerifyUserFactors( [FromBody] FactorListReq factorList)
        {
            try
            {
                if (!ValidateHelper.IsEmailFormat(factorList.Email))
                    return Json(new JsonResultModel(ReturnCode.ParameterError, "Email format is not correct."));

                var reply = Client.VerifyUserFactors(factorList);
                return Json(new JsonResultModel(ReturnCode.Success, "User factor verification complete.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.QueryError, ex.Status.Detail));
            }
        }
    }
}
