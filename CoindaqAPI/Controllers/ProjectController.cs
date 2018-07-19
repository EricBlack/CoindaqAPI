/**************************************************************
*  Filename:      ProjectController
*
*  Description:  ProjectController ClassFile.
*
*  Company:     Coindaq 
*
*  @Author:      ChaoShu
*
*  @Created:    2018/5/14/周一 14:41:02 
**************************************************************/

using CoindaqAPI.Utils;
using CoindaqAPI.Utils.Filter;
using CoindaqAPI.Utils.Redis;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
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
    public class ProjectController : BaseController
    {
        public ProjectService.ProjectServiceClient Client { get; set; }
        public ProjectController() :
            base("ServicesUrl\\ProjectService")
        {
            //BaseService.CheckChannelStatus(Channel);
            Client = new ProjectService.ProjectServiceClient(Channel);
        }

        /// <summary>
        /// Query project by id
        /// </summary>
        /// <param name="idReq"></param>
        /// <returns></returns>
        [HttpGet("QueryProjectById")]
        public IActionResult QueryProjectById([FromQuery] IdReq idReq)
        {
            try
            {
                var reply = Client.QueryProjectById(idReq);
                if (reply.Id == 0)
                    reply = null;

                return Json(new JsonResultModel(ReturnCode.Success, "Query successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Query user participation project
        /// </summary>
        /// <param name="idReq"></param>
        /// <returns></returns>
        [HttpGet("QueryUserParticipationProject")]
        public IActionResult QueryUserParticipationProject([FromQuery] IdReq idReq)
        {
            try
            {
                var reply = Client.QueryUserParticipationProject(idReq);
                return Json(new JsonResultModel(ReturnCode.Success, "Query user participation projects successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Query project photos information
        /// </summary>
        /// <param name="idReq"></param>
        /// <returns></returns>
        [HttpGet("QueryProjectPhotosInfo")]
        public IActionResult QueryProjectPhotosInfo([FromQuery] IdReq idReq)
        {
            try
            {
                var reply = Client.QueryProjectPhotosInfo(idReq);
                return Json(new JsonResultModel(ReturnCode.Success, "Query project photos information successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Query project videos information
        /// </summary>
        /// <param name="idReq"></param>
        /// <returns></returns>
        [HttpGet("QueryProjectVideosInfo")]
        public IActionResult QueryProjectVideosInfo([FromQuery] IdReq idReq)
        {
            try
            {
                var reply = Client.QueryProjectVideosInfo(idReq);
                return Json(new JsonResultModel(ReturnCode.Success, "Query project videos information successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Query project description information
        /// </summary>
        /// <param name="idReq"></param>
        /// <returns></returns>
        [HttpGet("QueryProjectDescriptionInfo")]
        public IActionResult QueryProjectDescriptionInfo([FromQuery] IdReq idReq)
        {
            try
            {
                var reply = Client.QueryProjectDescriptionInfo(idReq);

                return Json(new JsonResultModel(ReturnCode.Success, "Query project description information successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Query project certification information
        /// </summary>
        /// <param name="idReq"></param>
        /// <returns></returns>
        [HttpGet("QueryProjectCertificationInfo")]
        public IActionResult QueryProjectCertificationInfo([FromQuery] IdReq idReq)
        {
            try
            {
                var reply = Client.QueryProjectCertificationInfo(idReq);

                return Json(new JsonResultModel(ReturnCode.Success, "Query project certification information successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Query project members information
        /// </summary>
        /// <param name="idReq"></param>
        /// <returns></returns>
        [HttpGet("QueryProjectMembersInfo")]
        public IActionResult QueryProjectMembersInfo([FromQuery] IdReq idReq)
        {
            try
            {
                var reply = Client.QueryProjectMembersInfo(idReq);

                return Json(new JsonResultModel(ReturnCode.Success, "Query project members information successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Query project stages information
        /// </summary>
        /// <param name="idReq"></param>
        /// <returns></returns>
        [HttpGet("QueryProjectStagesInfo")]
        public IActionResult QueryProjectStagesInfo([FromQuery] IdReq idReq)
        {
            try
            {
                var reply = Client.QueryProjectStagesInfo(idReq);
                return Json(new JsonResultModel(ReturnCode.Success, "Query project stage information successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Filter project stage information
        /// </summary>
        /// <param name="stageFilter"></param>
        /// <returns></returns>
        [HttpGet("FilterProjectStageInfo")]
        public IActionResult FilterProjectStageInfo([FromQuery] StageFilterReq stageFilter)
        {
            try
            {
                var reply = Client.FilterProjectStageInfo(stageFilter);
                return Json(new JsonResultModel(ReturnCode.Success, "Query project stage information successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Query stage coin information
        /// </summary>
        /// <param name="idReq"></param>
        /// <returns></returns>
        [HttpGet("QueryStageCoinInfo")]
        public IActionResult QueryStageCoinInfo([FromQuery] IdReq idReq)
        {
            try
            {
                var reply = Client.QueryStageCoinInfo(idReq);
                return Json(new JsonResultModel(ReturnCode.Success, "Query stage coin information successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Filter stage coin information
        /// </summary>
        /// <param name="stageCoinFilter"></param>
        /// <returns></returns>
        [HttpGet("FilterStageCoinInfo")]
        public IActionResult FilterStageCoinInfo([FromQuery] StageCoinFilterReq stageCoinFilter)
        {
            try
            {
                var reply = Client.FilterStageCoinInfo(stageCoinFilter);

                return Json(new JsonResultModel(ReturnCode.Success, "Query stage coin information successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Query project notice information by filter
        /// </summary>
        /// <param name="noticeFilter"></param>
        /// <returns></returns>
        [HttpGet("QueryProjectNoticesByFilter")]
        public IActionResult QueryProjectNoticesByFilter([FromQuery] NoticeFilterReq noticeFilter)
        {
            try
            {
                var reply = Client.QueryProjectNoticesByFilter(noticeFilter);
                return Json(new JsonResultModel(ReturnCode.Success, "Query project notice information successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Query project notice vote information
        /// </summary>
        /// <param name="idReq"></param>
        /// <returns></returns>
        [HttpGet("QueryProjectNoticeVoteInfo")]
        public IActionResult QueryProjectNoticeVoteInfo([FromQuery] IdReq idReq)
        {
            try
            {
                var reply = Client.QueryProjectNoticeVoteInfo(idReq);
                if(reply.NoticeId == 0)
                {
                    reply = null;
                }
                return Json(new JsonResultModel(ReturnCode.Success, "Query project notice vote information successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Check user whehter have right to vote notice
        /// </summary>
        /// <param name="noticeRight"></param>
        /// <returns></returns>
        [HttpGet("CheckUserCanVoteNotice")]
        public IActionResult CheckUserCanVoteNotice([FromQuery] NoticeRightReq noticeRight)
        {
            try
            {
                var reply = Client.CheckUserCanVoteNotice(noticeRight);
                return Json(new JsonResultModel(ReturnCode.Success, "Query user vote right successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Query user notice vote information
        /// </summary>
        /// <param name="userVote"></param>
        /// <returns></returns>
        [HttpPost("UserNoticeVote")]
        public IActionResult UserNoticeVote([FromBody] UserVoteReq userVote)
        {
            try
            {
                var reply = Client.UserNoticeVote(userVote);
                return Json(new JsonResultModel(ReturnCode.Success, "User vote  notice successful."));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// User join project ico
        /// </summary>
        /// <param name="icoOrder"></param>
        /// <returns></returns>
        [HttpPost("JoinProjectIco")]
        public IActionResult JoinProjectIco([FromBody] IcoOrderReq icoOrder)
        {
            try
            {
                var reply = Client.JoinProjectIco(icoOrder);
                JObject data = new JObject();
                data["user_id"] = reply.UserId.ToString();
                data["project_id"] = reply.ProjectId;
                data["stage_id"] = reply.StageId;
                data["price"] = reply.Price;
                data["base_coin"] = reply.BaseCoin;
                data["target_coin"] = reply.TargetCoin;
                data["pay_count"] = reply.PayAmount;
                data["locktype"] = reply.LockType;

                string jsonString = JsonHelper.ToJson(data);
                //调用Redis服务
                string message = "";
                var result = RedisScript.GetInstance().ExecuteScript(jsonString, out message);
                if (result)
                {
                    return Json(new JsonResultModel(ReturnCode.Success, "User join project successful."));
                }
                else
                {
                    return Json(new JsonResultModel(ReturnCode.SubmitError, message));
                }
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        /// <summary>
        /// Check user kyc status
        /// </summary>
        /// <param name="idReq"></param>
        /// <returns></returns>
        [HttpGet("CheckUserKycStatusInfo")]
        public IActionResult CheckUserKycStatusInfo([FromQuery] IdReq idReq)
        {
            try
            {
                var reply = Client.CheckUserKycStatusInfo(idReq);
                return Json(new JsonResultModel(ReturnCode.Success, "Check user kyc status information successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.SubmitError, ex.Status.Detail));
            }
        }

        [HttpGet("QueryRecommendProjectsInfo")]
        public IActionResult QueryRecommendProjectsInfo()
        {
            try
            {
                var reply = Client.QueryRecommendProjectsInfo(new Proto.Empty{});

                return Json(new JsonResultModel(ReturnCode.Success, "Query successful.", reply)); 
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.QueryError, ex.Status.Detail));
            }
        }

        [HttpGet("QueryUserJoinProjectsInfo")]
        public IActionResult QueryUserJoinProjectsInfo([FromQuery] IdReq idReq)
        {
            try
            {
                var reply = Client.QueryUserJoinProjectsInfo(idReq);

                return Json(new JsonResultModel(ReturnCode.Success, "Query successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.QueryError, ex.Status.Detail));
            }
        }

        [HttpGet("QueryProjectDetailsInfo")]
        public IActionResult QueryProjectDetailsInfo([FromQuery] IdReq idReq)
        {
            try
            {
                var reply = Client.QueryProjectDetailsInfo(idReq);

                return Json(new JsonResultModel(ReturnCode.Success, "Query successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.QueryError, ex.Status.Detail));
            }
        }

        [HttpGet("QueryRaiseInvestmentDetailsInfo")]
        public IActionResult QueryRaiseInvestmentDetailsInfo([FromQuery] IdReq idReq)
        {
            try
            {
                var reply = Client.QueryRaiseInvestmentDetailsInfo(idReq);

                return Json(new JsonResultModel(ReturnCode.Success, "Query successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.QueryError, ex.Status.Detail));
            }
        }

        [HttpGet("QueryProjectNoticesInfo")]
        public IActionResult QueryProjectNoticesInfo([FromQuery] IdReq idReq)
        {
            try
            {
                var reply = Client.QueryProjectNoticesInfo(idReq);

                return Json(new JsonResultModel(ReturnCode.Success, "Query successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.QueryError, ex.Status.Detail));
            }
        }

        [HttpGet("QueryProjectNoticeNewsDetailsInfo")]
        public IActionResult QueryProjectNoticeNewsDetailsInfo([FromQuery] IdReq idReq)
        {
            try
            {
                var reply = Client.QueryProjectNoticeNewsDetailsInfo(idReq);

                return Json(new JsonResultModel(ReturnCode.Success, "Query successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.QueryError, ex.Status.Detail));
            }
        }

        [HttpGet("QueryNoticeVoteDetailsInfo")]
        public IActionResult QueryNoticeVoteDetailsInfo([FromQuery] NoticeRightReq noticeRight)
        {
            try
            {
                var reply = Client.QueryNoticeVoteDetailsInfo(noticeRight);

                return Json(new JsonResultModel(ReturnCode.Success, "Query successful.", reply));
            }
            catch (RpcException ex)
            {
                return Json(new JsonResultModel(ReturnCode.QueryError, ex.Status.Detail));
            }
        }
    }
}