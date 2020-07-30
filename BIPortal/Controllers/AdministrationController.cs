using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BIPortal.Models;
using BIPortal.DTO;
using System.Net.Http;
using System.Net.Http.Headers;
using AutoMapper;

namespace BIPortal.Controllers
{
    public class AdministrationController : Controller
    {
        // GET: Administration
        public ActionResult Index()
        {
            ViewBag.Message = "Administration Page";

            List<UsersModel> UsersList = null;

            if (Session["UserName"] == null || Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var emailID = Session["UserName"].ToString();

                string Baseurl = ConfigurationManager.AppSettings["baseURL"];

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));                    
                    var responseTask = client.GetAsync(string.Format("api/GetCurrentUser/?emailID={0}", emailID));
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
                            cfg.CreateMap<UserRoleMappingDTO, UserRoleMappingModel>();
                        });
                        IMapper mapper = config.CreateMapper();

                        UsersList = mapper.Map<List<UsersDTO>, List<UsersModel>>(readTask.Result);                        
                    }
                }
                if (UsersList.Count() == 0)
                {
                    ViewBag.InfoMessage = "You don't have access to the portal, kindly contact Admin to enable";
                }
                else
                {
                    Session["UserID"] = UsersList[0].UserID;
                }

                return View(UsersList);
            }

            //return View();
        }
    }
}