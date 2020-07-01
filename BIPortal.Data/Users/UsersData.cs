using AutoMapper;
using BIPortal.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIPortal.Data.Users
{
    public class UsersData
    {
        public IEnumerable<UsersDTO> GetUsers()
        {
            
            using (var context = new BIPortalEntities())
            {
                
                var usersResult = context.UserMasters.ToList();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<UserMaster, UsersDTO>();
                    cfg.CreateMap<PermissionMaster, PermissionMasterDTO>();
                });
                IMapper mapper = config.CreateMapper();

                return mapper.Map<List<UserMaster>, List<UsersDTO>>(usersResult);

            }
        }

        public IEnumerable<UsersDTO> GetCurrentUer(string sCurrentUserDetail)
        {

            using (var context = new BIPortalEntities())
            {
                var CurusersResult = (from u in context.UserMasters
                            where u.EmailID == sCurrentUserDetail
                            select u).ToList();

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<UserMaster, UsersDTO>();
                    cfg.CreateMap<PermissionMaster, PermissionMasterDTO>();
                });
                IMapper mapper = config.CreateMapper();

                return mapper.Map<List<UserMaster>, List<UsersDTO>>(CurusersResult);

            }
        }



    }
}
