using AutoMapper;
using BIPortal.DTO;
using BIPortal.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;

namespace BIPortal.Controllers
{
    public class UsersController : Controller
    {
        // GET: Users
        
        public ActionResult AddUser()
        {
            ViewBag.Message = "Add Users Page";

           ViewBag.type = new SelectList("","AccountId", "AccountName");
           ViewBag.Salutation = new SelectList("", "AccountId", "AccountName");



            IEnumerable<PermissionMasterModel> PermissionTypeList = null;

            string Baseurl = "https://localhost:44383/";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var responseTask = client.GetAsync("api/GetPermissionTypes");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<List<PermissionMasterDTO>>();
                    readTask.Wait();

                    var config = new MapperConfiguration(cfg =>
                    {
                        //cfg.CreateMap<UsersDTO, UsersModel>();
                        cfg.CreateMap<PermissionMasterDTO, PermissionMasterModel>();

                    });
                    IMapper mapper = config.CreateMapper();

                    PermissionTypeList = mapper.Map<List<PermissionMasterDTO>, List<PermissionMasterModel>>(readTask.Result);
                }
            }

            //ViewBag.type = new SelectList(PermissionTypeList);
            //ViewBag.type = PermissionTypeList;

            return View();
        }

        // GET: MyProfile
        public ActionResult MyProfile()
        {
            ViewBag.Message = "My Profile Page";
            IEnumerable<UsersModel> UsersList = null;

            string Baseurl = ConfigurationManager.AppSettings["baseURL"];

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var responseTask = client.GetAsync("api/GetCurrentUser");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<List<UsersDTO>>();
                    readTask.Wait();

                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<UsersDTO, UsersModel>();
                        cfg.CreateMap<PermissionMasterDTO, PermissionMasterModel>();
                    });
                    IMapper mapper = config.CreateMapper();

                    UsersList = mapper.Map<List<UsersDTO>, List<UsersModel>>(readTask.Result);
                }
            }

            //ViewBag.Name = "Selva";
            //ViewBag.EmailID = UsersList.Select(l => l.EmailID).ToString();

            foreach (var item in UsersList)
            {
                ViewBag.EmailID = item.EmailID;
                ViewBag.FirstName = item.FirstName;
                ViewBag.LastName = item.LastName;
                //ViewBag.Creationdate = item.CreatedDate;
                ViewBag.Creationdate = item.CreatedDate.ToString("dd/MM/yyyy");
                ViewBag.PermissionType = item.PermissionMaster.PermissionName;
                ViewBag.Saluation = item.Salutation;
            }

            return View(UsersList);
        }


        public ActionResult ViewUser()
        {
            ViewBag.Message = "View Users Page";
            IEnumerable<UsersModel> UsersList = null;

            //string Baseurl = "https://localhost:44383/";
            string Baseurl = ConfigurationManager.AppSettings["baseURL"];

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var responseTask = client.GetAsync("api/GetUsers");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<List<UsersDTO>>();
                    readTask.Wait();

                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<UsersDTO, UsersModel>();
                        cfg.CreateMap<PermissionMasterDTO, PermissionMasterModel>();
                    });
                    IMapper mapper = config.CreateMapper();

                    UsersList = mapper.Map<List<UsersDTO>, List<UsersModel>>(readTask.Result);
                }
            }

            return View(UsersList);
        }

        
        public ActionResult LoadEditUser(int UserId)
        {
            ViewBag.Message = "Edit User Page";
            IEnumerable<UsersModel> UsersList = null;
            var LoadEditUserModel = new UsersModel();

            string Baseurl = ConfigurationManager.AppSettings["baseURL"];

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //var responseTask = client.GetAsync("api/GetSelectedUser");
                var responseTask = client.GetAsync(string.Format("api/GetSelectedUser/?iUSERID={0}", UserId));
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<List<UsersDTO>>();
                    readTask.Wait();

                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<UsersDTO, UsersModel>();
                        cfg.CreateMap<PermissionMasterDTO, PermissionMasterModel>();
                    });
                    IMapper mapper = config.CreateMapper();

                    UsersList = mapper.Map<List<UsersDTO>, List<UsersModel>>(readTask.Result);
                }
            }


            foreach (var item in UsersList)
            {
                //ViewBag.EmailID = item.EmailID;                
                //ViewBag.FirstName = item.FirstName;
                //ViewBag.LastName = item.LastName;
                //ViewBag.Creationdate = item.CreatedDate.ToString("dd/MM/yyyy");
                //ViewBag.PermissionType = item.PermissionMaster.PermissionName;
                //ViewBag.Saluation = item.Salutation;

                LoadEditUserModel.EmailID = item.EmailID;
                LoadEditUserModel.FirstName = item.FirstName;
                LoadEditUserModel.LastName = item.LastName;
                LoadEditUserModel.CreatedDate = item.CreatedDate;
                LoadEditUserModel.PermissionID = item.PermissionMaster.PermissionID;
                // AddUserModel.PermissionMaster.PermissionID = item.PermissionMaster.PermissionID;
                LoadEditUserModel.Salutation = item.Salutation;
                LoadEditUserModel.UserID = item.UserID;
                LoadEditUserModel.CreatedBy = item.CreatedBy;
                LoadEditUserModel.Active = item.Active;
            }

            return View(LoadEditUserModel);
            
        }

        [HttpPost]
        public ActionResult AddUser(UsersModel AddUserModel)
        {
            string Baseurl = ConfigurationManager.AppSettings["baseURL"] + "/" + "api/AddNewUser";
            //string Baseurl = "https://localhost:44383/api/AddNewUser";

            using (var client = new HttpClient())
            {
                //IEnumerable<UsersModel> AddUserList = null;

                //HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //HTTP POST
                var postTask = client.PostAsJsonAsync<UsersModel>(Baseurl,AddUserModel);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    //return RedirectToAction("AddUser");
                    return RedirectToAction("ViewUser");
                }

            }

            return View(AddUserModel);
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult EditUser(UsersModel EditUserModel)
        {
            string Baseurl = ConfigurationManager.AppSettings["baseURL"] + "api/UpdateUser";

            using (var client = new HttpClient())
            {
                //IEnumerable<UsersModel> AddUserList = null;

                //HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //HTTP POST
                var postTask = client.PostAsJsonAsync<UsersModel>(Baseurl, EditUserModel);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    //return RedirectToAction("AddUser");
                    return RedirectToAction("ViewUser");
                }

            }

            return View(EditUserModel);
        }

    }
}








