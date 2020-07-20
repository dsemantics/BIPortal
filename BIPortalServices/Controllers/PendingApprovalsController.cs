using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BIPortal.Data.PendingApprovals;

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
    }
}
