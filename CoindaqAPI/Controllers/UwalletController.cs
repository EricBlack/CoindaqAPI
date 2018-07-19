/**************************************************************
 *  Filename:      UserWalletController
 *
 *  Description:  UserWalletController ClassFile.
 *
 *  Company:     Coindaq 
 *
 *  @Author:      ChaoShu
 *
 *  @Created:    2018/5/22/周二 17:29:39 
 **************************************************************/

using CoindaqAPI.Utils;
using CoindaqAPI.Utils.Filter;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Proto.UserWalletService;

namespace CoindaqAPI.Controllers
{
    [Route("/api/v1/[controller]")]
    [Exception]
    public class UwalletController : BaseController
    {
        public UserWalletServiceClient Client;

        public UwalletController() :
            base("ServicesUrl\\UserService")
        {
            Client = new UserWalletServiceClient(Channel);
        }

        /// <summary>
        /// Bind user platform coin address
        /// </summary>
        /// <param name="coinAddress"></param>
        /// <returns></returns>
        [HttpPost("BindUserCoinAddress")]
        [TokenCheck]
        public IActionResult BindUserCoinAddress([FromBody] UserCoinAddressReq coinAddress)
        {
            try
            {
                var reply = Client.BindUserCoinAddress(coinAddress);
                return Json(new JsonResultModel(ReturnCode.Success, "Coin address add successful."));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Query user platform charge coin address
        /// </summary>
        /// <param name="coinAddress"></param>
        /// <returns></returns>
        [HttpGet("QueryUserCoinAddress")]
        [TokenCheck]
        public IActionResult QueryUserCoinAddress([FromQuery] UserCoinAddressReq coinAddress)
        {
            try
            {
                var reply = Client.QueryUserCoinAddress(coinAddress);

                return Json(new JsonResultModel(ReturnCode.Success, "User coin address query successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Query user light wallet address
        /// </summary>
        /// <param name="walletAddress"></param>
        /// <returns></returns>
        [HttpPost("AddWalletAddress")]
        [TokenCheck]
        public IActionResult AddWalletAddress([FromBody] WalletAddressReq walletAddress)
        {
            try
            {
                var reply = Client.AddWalletAddress(walletAddress);
                return Json(new JsonResultModel(ReturnCode.Success, "User wallet address add successful."));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Query user light wallet address
        /// </summary>
        /// <param name="walletAddress"></param>
        /// <returns></returns>
        [HttpGet("QueryUserWalletAddress")]
        [TokenCheck]
        public IActionResult QueryUserWalletAddress([FromQuery] WalletAddressReq walletAddress)
        {
            try
            {
                var reply = Client.QueryUserWalletAddress(walletAddress);
                return Json(new JsonResultModel(ReturnCode.Success, "User wallet address query successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Delete user light wallet address
        /// </summary>
        /// <param name="idReq"></param>
        /// <returns></returns>
        [HttpPost("DeleteUserWalletAddress")]
        [TokenCheck]
        public IActionResult DeleteUserWalletAddress([FromBody] IdReq idReq)
        {
            try
            {
                var reply = Client.DeleteUserWalletAddress(idReq);
                return Json(new JsonResultModel(ReturnCode.Success, "User wallet address delete successful."));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Query user balance by filter
        /// </summary>
        /// <param name="userCoinAddress"></param>
        /// <returns></returns>
        [HttpGet("QueryUserBalanceByFilter")]
        [TokenCheck]
        public IActionResult QueryUserBalanceByFilter([FromQuery] UserCoinAddressReq userCoinAddress)
        {
            try
            {
                var reply = Client.QueryUserBalanceByFilter(userCoinAddress);

                return Json(new JsonResultModel(ReturnCode.Success, "Query user balance successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.QueryError, ex.Status.Detail));
            }
        }
    }
}
