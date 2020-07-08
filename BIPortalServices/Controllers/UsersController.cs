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
                {
                    return BadRequest("Not a valid model");
                }
                else
                {
                    UsersData SaveUserData = new UsersData();
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<UsersModel, UsersDTO>();
                        cfg.CreateMap<UserRoleMappingModel, UserRoleMappingDTO>();
                        cfg.CreateMap<WorkFlowMasterModel, WorkFlowMasterDTO>();
                        cfg.CreateMap<WorkFlowDetailsModel, WorkFlowDetailsDTO>();
                    });
                    IMapper mapper = config.CreateMapper();

                    var userData = mapper.Map<UsersModel, UsersDTO>(AddUserModel);

                    SaveUserData.SaveUsersData(userData);

                    return Created("api/AddNewUser", true);
                }


               

                //using (var ctx = new BIPortalEntities())
                //{
                //    ctx.UserMasters.Add(new UserMaster
                //    {
                //        Salutation = "Mr.",
                //        FirstName = AddUserModel.FirstName,
                //        LastName = AddUserModel.LastName,
                //        EmailID = AddUserModel.EmailID,
                //        PermissionID = 1,
                //        //CreatedDate = DateTime.Now,
                //        CreatedDate = AddUserModel.CreatedDate,
                //        CreatedBy = "SK",
                //        ModifiedDate = DateTime.Now,
                //        Active = AddUserModel.Active
                //    });
                //    ctx.SaveChanges();
                //}

                //return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest("Can't add new User");
            }
        }


        [Route("api/GetUsers")]
        public IHttpActionResult GetUsers()
        {
            try
            {
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
                string sCurrentUserDetail = "Selva.kumar@datasematics.in";
                UsersData usersData = new UsersData();
                var users = usersData.GetCurrentUser(sCurrentUserDetail);

                return Ok(users);


            }
            catch (Exception ex)
            {
                return BadRequest("Could not fetch roles");
            }
        }

        [Route("api/GetSelectedUser")]
        public IHttpActionResult GetSelectedUser(int iUSERID)
        {
            try
            {
                UsersData usersData = new UsersData();
                var users = usersData.GetSeletedUser(iUSERID);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest("Could not fetch roles");
            }
        }

        //[Route("api/GetSelectedUserRoles")]
        //public IHttpActionResult GetSelectedUserRoles(int iUSERID)
        //{
        //    try
        //    {
        //        UsersData usersData = new UsersData();
        //        var users = usersData.GetSelectedUserRoles(iUSERID);
        //        return Ok(users);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest("Could not fetch roles");
        //    }
        //}

        [AcceptVerbs("GET", "POST")]
        [Route("api/UpdateUser")]
        public IHttpActionResult UpdateUser(UsersModel EditUserModel)
        {
            try
            {

                //if (!ModelState.IsValid)
                //    return BadRequest("Not a valid model");

                using (var ctx = new BIPortalEntities())
                {

                    var EditUserData = ctx.UserMasters.Where(x => x.UserID == EditUserModel.UserID).FirstOrDefault();
                    if (EditUserData != null)
                    {
                        EditUserData.EmailID = EditUserModel.EmailID;
                        EditUserData.FirstName = EditUserModel.FirstName;
                        EditUserData.LastName = EditUserModel.LastName;
                        //EditUserData.PermissionMaster.PermissionID = EditUserModel.PermissionMaster.PermissionID;
                        EditUserData.PermissionID = EditUserModel.PermissionID;
                        EditUserData.Salutation = EditUserModel.Salutation;
                        //EditUserData.CreatedDate = EditUserModel.CreatedDate;
                        EditUserData.ModifiedBy = "Selva";
                        EditUserData.ModifiedDate  = DateTime.Now;
                        //EditUserData.CreatedBy = EditUserModel.CreatedBy;
                        EditUserData.Active = EditUserModel.Active;


                        //EditUserData.EmailID = "demooooo@ds.com";
                        //EditUserData.FirstName = "Demooooo";
                        //EditUserData.LastName = "Demoooouserrrrr1";
                        //EditUserData.PermissionMaster.PermissionName = "1";
                        //EditUserData.Salutation = "Miss";
                        //EditUserData.Active = true;

                        ctx.SaveChanges();
                    }
                }

                return Ok();


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
            }

            catch (Exception ex)
            {
                return BadRequest("Could not fetch Permission Types");
            }
        }

    }
}
