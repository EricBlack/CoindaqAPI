/**************************************************************
 *  Filename:      OtcController
 *
 *  Description:  OtcController ClassFile.
 *
 *  Company:     Coindaq 
 *
 *  @Author:      ChaoShu
 *
 *  @Created:    2018/6/3/周日 10:13:34 
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

namespace CoindaqAPI.Controllers
{
    [Route("/api/v1/[controller]")]
    [Exception]
    [TokenCheck]
    public class OtcController : BaseController
    {
        public OtcService.OtcServiceClient Client { get; set; }
        public OtcController() :
            base("ServicesUrl\\ProjectService")
        {
            //BaseService.CheckChannelStatus(Channel);
            Client = new OtcService.OtcServiceClient(Channel);
        }

        //支付账号添加 - POST
        [HttpPost("AddPaymentAccount")]
        public IActionResult AddPaymentAccount([FromBody]PaymentAccountReq paymentAccount)
        {
            try
            {
                var reply = Client.AddPaymentAccount(paymentAccount);

                return Json(new JsonResultModel(ReturnCode.Success, "Add payment information successful."));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        //支付账号删除 - POST
        [HttpPost("DeletePaymentAccount")]
        public IActionResult DeletePaymentAccount([FromBody] AccountFilterReq accountFilter)
        {
            try
            {
                var reply = Client.DeletePaymentAccount(accountFilter);

                return Json(new JsonResultModel(ReturnCode.Success, "Delete payment information successful."));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        //支付账号查询 - GET
        [HttpGet("QueryPaymentAccount")]
        public IActionResult QueryPaymentAccount([FromQuery] AccountFilterReq accountFilter)
        {
            try
            {
                var reply = Client.QueryPaymentAccount(accountFilter);

                return Json(new JsonResultModel(ReturnCode.Success, "Query payment information successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        //消息添加 - POST
        [HttpPost("AddShortMessage")]
        public IActionResult AddShortMessage([FromBody] MessageReq messageReq)
        {
            try
            {
                var reply = Client.AddShortMessage(messageReq);

                return Json(new JsonResultModel(ReturnCode.Success, "Send short message successful."));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        //消息列表查询 - GET
        [HttpGet("QueryShortMessages")]
        public IActionResult QueryShortMessages([FromQuery] UserPairReq userPair)
        {
            try
            {
                var reply = Client.QueryShortMessages(userPair);

                return Json(new JsonResultModel(ReturnCode.Success, "Query short message successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.QueryError, ex.Status.Detail));
            }
        }

        //交易对信息查询 - GET
        [HttpGet("QueryCurrencyPairs")]
        public IActionResult QueryCurrencyPairs([FromQuery] PairFilterReq pairFilter)
        {
            try
            {
                var reply = Client.QueryCurrencyPairs(pairFilter);

                return Json(new JsonResultModel(ReturnCode.Success, "Query currency pair information successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.QueryError, ex.Status.Detail));
            }
        }

        //创建Otc订单 - POST
        [HttpPost("CreateOtcOrder")]
        public IActionResult CreateOtcOrder([FromBody] OtcOrderReq otcOrder)
        {
            try
            {
                var reply = Client.CreateOtcOrder(otcOrder);

                return Json(new JsonResultModel(ReturnCode.Success, "Create otc order successful."));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.QueryError, ex.Status.Detail));
            }
        }

        //查询广告单列表信息 - GET
        [HttpGet("QueryOtcOrderList")]
        public IActionResult QueryOtcOrderList([FromQuery] OtcFilterReq otcFilter)
        {
            try
            {
                var reply = Client.QueryOtcOrderList(otcFilter);

                return Json(new JsonResultModel(ReturnCode.Success, "Query otc orders successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.QueryError, ex.Status.Detail));
            }
        }

        //查询广告单详细信息 - GET
        [HttpGet("QueryOtcOrderDetails")]
        public IActionResult QueryOtcOrderDetails([FromQuery] IdReq idReq)
        {
            try
            {
                var reply = Client.QueryOtcOrderDetails(idReq);
                if (reply.UserId == 0)
                    reply = null;

                return Json(new JsonResultModel(ReturnCode.Success, "Query otc order details successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.QueryError, ex.Status.Detail));
            }
        }

        //确认下单交易 - POST
        [HttpPost("ConfirmDealOrder")]
        public IActionResult ConfirmDealOrder([FromQuery] OtcDetailReq otcDetail)
        {
            try
            {
                var reply = Client.ConfirmDealOrder(otcDetail);

                return Json(new JsonResultModel(ReturnCode.Success, "User order successful.", reply));
            }
            catch(RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.QueryError, ex.Status.Detail));
            }
        }

        //取消订单 - POST
        [HttpPost("CancelDealOrder")]
        public IActionResult CancelDealOrder([FromBody] IdReq idReq)
        {
            try
            {
                var reply = Client.CancelDealOrder(idReq);

                return Json(new JsonResultModel(ReturnCode.Success, "User cancel order successful."));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.QueryError, ex.Status.Detail));
            }
        }

        //确认已付款 - POST
        [HttpPost("MarkOrderPayment")]
        public IActionResult MarkOrderPayment([FromBody] MarkUserPaiedReq userPaiedReq)
        {
            try
            {
                var reply = Client.MarkOrderPayment(userPaiedReq);

                return Json(new JsonResultModel(ReturnCode.Success, "User mark order paied successful."));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.QueryError, ex.Status.Detail));
            }
        }

        //订单申诉 - POST
        [HttpPost("ComplainOrderPayment")]
        public IActionResult ComplainOrderPayment([FromBody] ComplainReq complainReq)
        {
            try
            {
                var reply = Client.ComplainOrderPayment(complainReq);

                return Json(new JsonResultModel(ReturnCode.Success, "User complain order successful."));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.QueryError, ex.Status.Detail));
            }
        }
        
        //查询订单详细信息 - GET
        [HttpGet("QueryOtcDetailsById")]
        public IActionResult QueryOtcDetailsById([FromQuery] IdReq idReq)
        {
            try
            {
                var reply = Client.QueryOtcDetailsById(idReq);

                return Json(new JsonResultModel(ReturnCode.Success, "Query order details information successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.QueryError, ex.Status.Detail));
            }
        }

        //查询广告单关联订单信息 - GET
        [HttpGet("QueryOtcOrderDetailsListByOtcOrderId")]
        public IActionResult QueryOtcOrderDetailsListByOtcOrderId([FromQuery] IdReq idReq)
        {
            try
            {
                var reply = Client.QueryOtcOrderDetailsListByOtcOrderId(idReq);

                return Json(new JsonResultModel(ReturnCode.Success, "Query order details list information successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.QueryError, ex.Status.Detail));
            }
        }

        //查询用户参与订单信息 - GET
        [HttpGet("QueryUserOrderDetailsListByFilter")]
        public IActionResult QueryUserOrderDetailsListByFilter([FromQuery] OtcDetailsFilterReq otcDetailsFilter)
        {
            try
            {
                var reply = Client.QueryUserOrderDetailsListByFilter(otcDetailsFilter);

                return Json(new JsonResultModel(ReturnCode.Success, "Query user order list information successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.QueryError, ex.Status.Detail));
            }
        }
    }
}
