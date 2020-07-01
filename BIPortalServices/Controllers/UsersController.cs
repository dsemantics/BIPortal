using AutoMapper;
using BIPortal.Data;
using BIPortal.Data.Users;
using BIPortal.DTO;
using BIPortal.Models;
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

        [AcceptVerbs("GET", "POST")]
        [HttpPost]
        [Route("api/AddNewUser")]
        public IHttpActionResult AddNewUser(UsersModel AddUserModel)

        {
            try
            {

                if (!ModelState.IsValid)
                    return BadRequest("Not a valid model");

                using (var ctx = new BIPortalEntities())
                {
                    ctx.UserMasters.Add(new UserMaster
                    {
                       
                        Salutation = "Mr.",
                        FirstName = AddUserModel.FirstName,
                        LastName = AddUserModel.LastName,
                        EmailID = AddUserModel.EmailID,
                        PermissionID = 1,
                        //CreatedDate = DateTime.Now,
                        CreatedDate = AddUserModel.CreatedDate,
                        CreatedBy = "SK",
                        ModifiedDate = DateTime.Now,
                        //ModifiedBy = "sk",
                        Active = AddUserModel.Active


                    });

                    ctx.SaveChanges();
                }

                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest("Could not fetch roles");
            }
        }


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

       
        [Route("api/GetCurrentUser")]
        public IHttpActionResult GetCurrentUser()
        {
            try
            {
                string sCurrentUserDetail = "demo@ds.com";
                UsersData usersData = new UsersData();
                var users = usersData.GetCurrentUer(sCurrentUserDetail);

                return Ok(users);


            }
            catch (Exception ex)
            {
                return BadRequest("Could not fetch roles");
            }
        }



        [Route("api/GetPermissionTypes")]
        public IHttpActionResult GetPermissionTypes()
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

                //UsersData usersData = new UsersData();
                //var users = usersData.GetUsers();


                List<PermissionMasterDTO> PermissionMasterDTO = new List<PermissionMasterDTO>();
                using (var context = new BIPortalEntities())
                {
                    
                    /*Getting data from database*/
                    var  objpermissiontypeslist = (from data in context.PermissionMasters
                                                             select data).ToList();

                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<PermissionMaster, PermissionMasterDTO>();                        
                    });
                    IMapper mapper = config.CreateMapper();


                    PermissionMasterDTO = mapper.Map<List<PermissionMaster>, List<PermissionMasterDTO>>(objpermissiontypeslist).ToList();
                }
                return Ok(PermissionMasterDTO);


                // SelectList objmodeldata = new SelectList(objcountrylist, "PermissionID", "PermissionName", 0);

                ///*Assign value to model*/
                //UsersModel objcountrymodel = new UsersModel();
                //objcountrymodel.PermissionListModel = objmodeldata;

            }

            catch (Exception ex)
            {
                return BadRequest("Could not fetch roles");
            }
        }
    }
}
