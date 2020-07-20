using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using BIPortal.DTO;
using BIPortal.Models;

namespace BIPortal.Controllers
{
    public class PendingApprovalsController : Controller
    {
        // GET: ViewPendingApprovals
        public ActionResult ViewPendingApprovals()
        {
            List<WorkFlowMasterModel> pendingApprovals = new List<WorkFlowMasterModel>();

            string Baseurl = ConfigurationManager.AppSettings["baseURL"];
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var responseTask = client.GetAsync("api/GetPendingApprovals");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<List<WorkFlowMasterDTO>>();
                    readTask.Wait();

                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<WorkFlowMasterDTO, WorkFlowMasterModel>();
                        cfg.CreateMap<UsersDTO, UsersModel>();
                        cfg.CreateMap<PermissionMasterDTO, PermissionMasterModel>();
                        cfg.CreateMap<UserRoleMappingDTO, UserRoleMappingModel>();
                    });
                    IMapper mapper = config.CreateMapper();

                    pendingApprovals = mapper.Map<List<WorkFlowMasterDTO>, List<WorkFlowMasterModel>>(readTask.Result);
                }
            }

            return View(pendingApprovals);
        }
    }
}