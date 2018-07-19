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

namespace CoindaqAPI.Controllers
{
    //[Produces("application/json")]
    [Route("/api/v1/[controller]")]
    [Exception]
    public class KycController :BaseController, IDisposable
    {
        public KycService.KycServiceClient Client { get; set; }
        public KycController() :
            base("ServicesUrl\\UserService")
        {
            Client = new KycService.KycServiceClient(Channel);
        }

        /// <summary>
        ///  Create kyc info
        /// </summary>
        /// <param name="input">CreateKycReq</param>
        /// <returns>KycReply</returns>
        [HttpPost("CreateKyc")]
        [TokenCheck]
        public ActionResult CreateKyc([FromBody] CreateKycReq input)
        {
            try
            {
                var reply = Client.CreateKyc(input);

                return Json(new JsonResultModel(ReturnCode.Success, "Kyc created successfully."));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

         /*
        /// <summary>
        /// Reject kyc info
        /// </summary>
        /// <param name="handleKyc"></param>
        /// <returns></returns>
        [HttpPost("rejectKyc")]
        public ActionResult rejectKyc([FromBody] HandleKycReq handleKyc)
        {
            try
            {
                var reply = Client.RejectKyc(handleKyc);
                return Json(new JsonResultModel(ReturnCode.Success, "Kyc audit been rejected.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

 
        /// <summary>
        /// Pass kyc info
        /// </summary>
        /// <param name="handleKyc"></param>
        /// <returns></returns>
        [HttpPost("passKyc")]
        public ActionResult PassKyc([FromBody] HandleKycReq handleKyc)
        {
            try
            {
                var reply = Client.PassKyc(handleKyc);
                return Json(new JsonResultModel(ReturnCode.Success, "Kyc audit been passed.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        */

        /// <summary>
        /// Update Kyc Info
        /// </summary>
        /// <param name="updateKyc"></param>
        /// <returns></returns>
        [HttpPost("UpdateKycInfo")]
        [TokenCheck]
        public ActionResult UpdateKycInfo([FromBody] UpdateKycReq updateKyc)
        {
            try
            {
                var reply = Client.UpdateKycInfo(updateKyc);
                return Json(new JsonResultModel(ReturnCode.Success, "Kyc update successful."));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Query kyc info
        /// </summary>
        /// <param name="idReq"></param>
        /// <returns></returns>
        [HttpGet("QueryKycInfoById")]
        [TokenCheck]
        public ActionResult QueryKycInfoById([FromQuery] IdReq idReq)
        {
            try
            {
                var reply = Client.QueryKycInfoById(idReq);
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
        /// Get Latest Kyc Info
        /// </summary>
        /// <param name="latestKyc"></param>
        /// <returns></returns>
        [HttpGet("QueryKycLastInfo")]
        [TokenCheck]
        public ActionResult QueryKycLastInfo([FromQuery] LatestKycReq latestKyc)
        {
            try
            {
                var reply = Client.QueryKycLastInfo(latestKyc);
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
        /// Query kyc info by filter
        /// </summary>
        /// <param name="filterReq"></param>
        /// <returns></returns>
        [HttpGet("QueryKycInfos")]
        [TokenCheck]
        public ActionResult QueryKycInfos([FromQuery] FilterReq filterReq)
        {
            try
            {
                var reply = Client.QueryKycInfos(filterReq);
                return Json(new JsonResultModel(ReturnCode.Success, "Query successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.QueryError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Query country by id
        /// </summary>
        /// <param name="idReq"></param>
        /// <returns></returns>
        [HttpGet("QueryCountryById")]
        public ActionResult QueryCountryById([FromQuery] IdReq idReq)
        {
            try
            {
                var reply = Client.QueryCountryById(idReq);
                if (reply.Id == 0)
                    reply = null;
                return Json(new JsonResultModel(ReturnCode.Success, "Query country information successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.QueryError, ex.Status.Detail));
            }
        }

        [HttpGet("QueryAllCountryInfo")]
        public ActionResult QueryAllCountryInfo()
        {
            try
            {
                var reply = Client.QueryAllCountryInfo(new Empty{ });
                return Json(new JsonResultModel(ReturnCode.Success, "Query all country information successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.QueryError, ex.Status.Detail));
            }
        }
     }
}
