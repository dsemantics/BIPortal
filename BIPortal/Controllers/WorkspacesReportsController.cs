using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BIPortal.DTO;
using BIPortal.Models;
using AutoMapper;
using System.Net.Http;
using System.Net.Http.Headers;

namespace BIPortal.Controllers
{
    public class WorkspacesReportsController : Controller
    {
        // GET: WorkspacesReports
        public ActionResult Index()
        {
            ViewBag.Message = "WorkSpaces and Reports Page";
            IEnumerable<WorkspaceModel> workspacesList = null;

            //Hosted web API REST Service base url  
            string Baseurl = "https://localhost:44383/";

            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                //HttpResponseMessage Res = await client.GetAsync("api/Employee/GetAllEmployees");

                //var responseTask = client.GetAsync("api/workspaces/GetPowerBIWorkspace");
                var responseTask = client.GetAsync("api/GetWorkSpaces");
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<List<WorkspaceDTO>>();
                    readTask.Wait();

                    var config = new MapperConfiguration(cfg => {
                        cfg.CreateMap<WorkspaceDTO, WorkspaceModel>();
                    });
                    IMapper mapper = config.CreateMapper();

                    workspacesList = mapper.Map<List<WorkspaceDTO>, List<WorkspaceModel>>(readTask.Result);
                }
            }
            return View(workspacesList);
        }
    }
}