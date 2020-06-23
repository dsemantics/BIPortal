using AutoMapper;
using BIPortal.Data;
using BIPortal.Data.Users;
using BIPortal.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BIPortalServices.Controllers
{
    public class UsersController : ApiController
    {
        [Route("api/GetUsers")]
        public IHttpActionResult GetUsers()
        {
            try
            {
                //List<UsersDTO> usersDTO = new List<UsersDTO>();
                //using (var context = new BIPortalEntities())
                //{

                //    var usersResult = context.UserMasters.Include("GroupMaster").ToList();

                //    var config = new MapperConfiguration(cfg =>
                //    {
                //        cfg.CreateMap<UserMaster, UsersDTO>();
                //        cfg.CreateMap<GroupMaster, GroupMasterDTO>();
                //    });
                //    IMapper mapper = config.CreateMapper();


                //    usersDTO = mapper.Map<List<UserMaster>, List<UsersDTO>>(usersResult);
                //}
                //return Ok(usersDTO);

                UsersData usersData = new UsersData();
                var users = usersData.GetUsers();

                return Ok(users);


            }
            catch (Exception ex)
            {
                return BadRequest("Could not fetch roles");
            }
        }

        [Route("api/AddUser")]
        public IHttpActionResult AddUser( )
        {
            try
            {
                List<UsersDTO> usersDTO = new List<UsersDTO>();
                using (var context = new BIPortalEntities())
                {
                    //var usersResult = context.UserMasters.ToList();

                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<UserMaster, UsersDTO>();
                    });
                    IMapper mapper = config.CreateMapper();

                   // usersDTO = mapper.Map<List<UserMaster>, List<UsersDTO>>(usersResult);
                }
                return Ok(usersDTO);
            }
            catch (Exception ex)
            {
                return BadRequest("Could not fetch roles");
            }
        }
    }
}
