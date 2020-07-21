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
                cfg.CreateMap<WorkFlowDetailsDTO, WorkFlowDetail>();
            });
            IMapper mapper = config.CreateMapper();

            var workFlowMasterDetails = mapper.Map<List<WorkFlowMasterDTO>, List<WorkFlowMaster>>(workFlowMasterDTO);

            using (var context = new BIPortalEntities())
            {
                context.WorkFlowMasters.AddRange(workFlowMasterDetails);
                context.SaveChanges();
            }
        }
    }
}
