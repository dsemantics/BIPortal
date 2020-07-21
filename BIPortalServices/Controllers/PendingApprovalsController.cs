using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using BIPortal.Data.PendingApprovals;
using BIPortal.DTO;
using BIPortal.Models;

namespace BIPortalServices.Controllers
{
    public class PendingApprovalsController : ApiController
    {
        [Route("api/GetPendingApprovals")]
        //To get roles
        public IHttpActionResult GetPendingApprovals()
        {
            try
            {
                PendingApprovalsData pendingApprovalsData = new PendingApprovalsData();
                var pendingApprovals = pendingApprovalsData.GetPendingApprovals();

                return Ok(pendingApprovals);
            }
            catch (Exception ex)
            {
                return BadRequest("Could not fetch pending approvals");
            }
        }

        [Route("api/GetRequestDetails")]
        //To get access rights
        public IHttpActionResult GetRequestDetails(int requestID)
        {
            try
            {
                PendingApprovalsData pendingApprovalData = new PendingApprovalsData();
                var pendingApprovals = pendingApprovalData.GetRequestDetails(requestID);                          
                
                return Ok(pendingApprovals);
            }
            catch (Exception ex)
            {
                return BadRequest("Could not fetch pending approval details");
            }
        }

        [HttpPost]
        [Route("api/ApproveRequest")]
        public IHttpActionResult ApproveRequest(List<WorkFlowMasterModel> workFlowMasterModel)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest("Not a valid model");
                }
                else
                {
                    PendingApprovalsData pendingApprovalData = new PendingApprovalsData();

                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<WorkFlowMasterModel, WorkFlowMasterDTO>();
                        cfg.CreateMap<WorkFlowDetailsModel, WorkFlowDetailsDTO>();
                    });
                    IMapper mapper = config.CreateMapper();

                    var workFlowMasterData = mapper.Map<List<WorkFlowMasterModel>, List<WorkFlowMasterDTO>>(workFlowMasterModel);

                    pendingApprovalData.ApproveRequest(workFlowMasterData);

                    return Created("api/ApproveRequest", true);
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Can't approve the request");
            }
        }

        [HttpPost]
        [Route("api/RejectRequest")]
        public IHttpActionResult RejectRequest(List<WorkFlowMasterModel> workFlowMasterModel)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest("Not a valid model");
                }
                else
                {
                    PendingApprovalsData pendingApprovalData = new PendingApprovalsData();

                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<WorkFlowMasterModel, WorkFlowMasterDTO>();
                        cfg.CreateMap<WorkFlowDetailsModel, WorkFlowDetailsDTO>();
                    });
                    IMapper mapper = config.CreateMapper();

                    var workFlowMasterData = mapper.Map<List<WorkFlowMasterModel>, List<WorkFlowMasterDTO>>(workFlowMasterModel);

                    pendingApprovalData.RejectRequest(workFlowMasterData);

                    return Created("api/RejectRequest", true);
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Can't reject the request");
            }
        }
    }
}
