using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BIPortal.DTO;
using BIPortal.Data.NewRequest;
using AutoMapper;

namespace BIPortal.Data.NewRequest
{
    public class NewRequestData
    {
        public void AddNewRequest(List<WorkFlowMasterDTO> workFlowMasterDTO)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<WorkFlowMasterDTO, WorkFlowMaster>();
            });
            IMapper mapper = config.CreateMapper();

            var workFlowMasterDetails = mapper.Map<List<WorkFlowMasterDTO>, List<WorkFlowMaster>>(workFlowMasterDTO);

            using (var context = new BIPortalEntities())
            {
                foreach (var f in workFlowMasterDTO)
                {
                    // Insert WorkFlowMaster
                    var ownerIDResult = (from u in context.WorkSpaceOwnerMasters
                                         where u.WorkspaceID == f.WorkspaceID
                                         select u).ToList();

                    var userWorkFlowMasterMapping = new WorkFlowMaster
                    {
                        WorkspaceID = f.WorkspaceID,
                        WorkspaceName = f.WorkspaceName,
                        ReportID = f.ReportID,
                        ReportName = f.ReportName,
                        OwnerID = ownerIDResult[0].OwnerID,
                        RequestedBy = "Venkat", // user(logged In) email address should come here
                        RequestedDate = DateTime.Now,
                        Status = "PENDING"
                    };
                    context.WorkFlowMasters.Add(userWorkFlowMasterMapping);                    
                }
                context.SaveChanges();
            }

        }
    }
}
