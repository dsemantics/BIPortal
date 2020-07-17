using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BIPortal.Models;
using BIPortal.DTO;
using AutoMapper;
using BIPortal.Data.NewRequest;

namespace BIPortalServices.Controllers
{
    public class CreateRequestController : ApiController
    {
        [HttpPost]
        [Route("api/AddNewRequest")]
        public IHttpActionResult AddNewRequest(List<WorkFlowMasterModel> workFlowMasterModel)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest("Not a valid model");
                }
                else
                {
                    NewRequestData newRequestData = new NewRequestData();

                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<WorkFlowMasterModel, WorkFlowMasterDTO>();
                    });
                    IMapper mapper = config.CreateMapper();

                    var workFlowMasterData = mapper.Map<List<WorkFlowMasterModel>, List<WorkFlowMasterDTO>>(workFlowMasterModel);

                    newRequestData.AddNewRequest(workFlowMasterData);

                    return Created("api/AddNewRequest", true);
                }             
            }
            catch (Exception ex)
            {
                return BadRequest("Can't add new request");
            }
        }
    }
}
