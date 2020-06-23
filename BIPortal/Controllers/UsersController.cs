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
            IEnumerable<UsersModel> UsersList = null;

            //string Baseurl = "https://localhost:44383/";

            //using (var client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri(Baseurl);
            //    client.DefaultRequestHeaders.Clear();
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    var responseTask = client.GetAsync("api/GetUsers");
            //    responseTask.Wait();
            //    var result = responseTask.Result;
            //    if (result.IsSuccessStatusCode)
            //    {
            //        var readTask = result.Content.ReadAsAsync<List<UsersDTO>>();
            //        readTask.Wait();

            //        var config = new MapperConfiguration(cfg =>
            //        {
            //            //cfg.CreateMap<UsersDTO, UsersModel>();
            //            cfg.CreateMap<UsersModel, UsersDTO>();

            //        });
            //        IMapper mapper = config.CreateMapper();

            //        UsersList = mapper.Map<List<UsersDTO>, List<UsersModel>>(readTask.Result);
            //    }
            //}

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

        public ActionResult EditUser()
        {
            ViewBag.Message = "Edit User Page";
            return View();
        }
    }
}








