﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using BIPortal.Data.PendingApprovals;
using BIPortal.Data.WorkSpaces;
using BIPortal.DTO;
using BIPortal.Models;
using BIPortal.Data.Email;
using System.Configuration;

namespace BIPortalServices.Controllers
{
    public class PendingApprovalsController : ApiController
    {
        [Route("api/GetPendingApprovals")]
        //To get roles
        public IHttpActionResult GetPendingApprovals(int? ownerID)
        {
            try
            {
                PendingApprovalsData pendingApprovalsData = new PendingApprovalsData();
                var pendingApprovals = pendingApprovalsData.GetPendingApprovals(ownerID);

                return Ok(pendingApprovals);
            }
            catch (Exception ex)
            {
                return BadRequest("Could not fetch pending approvals");
            }
        }

        [Route("api/GetUsersPendingApprovals")]
        //To get roles
        public IHttpActionResult GetUsersPendingApprovals(string emailID)
        {
            try
            {
                PendingApprovalsData pendingApprovalsData = new PendingApprovalsData();
                var pendingApprovals = pendingApprovalsData.GetPendingApprovals(emailID);

                return Ok(pendingApprovals);
            }
            catch (Exception ex)
            {
                return BadRequest("Could not fetch users pending approvals");
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

                    string powerBIUserName = ConfigurationManager.AppSettings["powerBIUserName"];
                    string powerBIPWD = ConfigurationManager.AppSettings["powerBIPWD"];
                    string smtpHost = ConfigurationManager.AppSettings["smtpHost"];
                    int smtpPort = Convert.ToInt32(ConfigurationManager.AppSettings["smtpPort"]);

                    WorkSpaceData workSpaceData = new WorkSpaceData();
                    var s = workSpaceData.AddPowerBIWorkspaceUser(workFlowMasterData, powerBIUserName, powerBIPWD, smtpHost, smtpPort);               
                    
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

                    string powerBIUserName = ConfigurationManager.AppSettings["powerBIUserName"];
                    string powerBIPWD = ConfigurationManager.AppSettings["powerBIPWD"];
                    string smtpHost = ConfigurationManager.AppSettings["smtpHost"];
                    int smtpPort = Convert.ToInt32(ConfigurationManager.AppSettings["smtpPort"]);

                    pendingApprovalData.RejectRequest(workFlowMasterData, powerBIUserName, powerBIPWD, smtpHost, smtpPort);

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
