using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BIPortal.Models;
using BIPortal.DTO;

namespace BIPortal.Mapping
{
    public class MapperProfile: Profile
    {
        public MapperProfile()
        {
            //CreateMap<WorkspaceModel, WorkspaceDTO>();
            CreateMap<WorkspaceDTO, WorkspaceModel>();
            CreateMap<List<WorkspaceDTO>, List<WorkspaceModel>>();
        }
    }
}
