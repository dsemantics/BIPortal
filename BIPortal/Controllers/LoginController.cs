using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Identity.Client;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using BIPortal.Models;
using System.Net.Http.Headers;
using System.Configuration;
using System.Net.Http;
using System.Net;

namespace BIPortal.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        //[Authorize]
        public ActionResult Index()
        {
            ViewBag.Message = "Your Login page.";
            return View();
        }

        // Sends an OpenIDConnect Sign-In Request.  
        //public async Task<string> SignIn()
        ////public void SignIn()
        //{
        //    //if (!Request.IsAuthenticated)
        //    //{

        //    //    HttpContext.GetOwinContext()
        //    //        .Authentication.Challenge(new AuthenticationProperties { RedirectUri = "/" },
        //    //            OpenIdConnectAuthenticationDefaults.AuthenticationType);
        //    //}

        //    //if (!Request.IsAuthenticated)
        //    //{
        //    //    HttpContext.GetOwinContext().Authentication.Challenge(
        //    //        new AuthenticationProperties { RedirectUri = "/Administration" },
        //    //        OpenIdConnectAuthenticationDefaults.AuthenticationType);
        //    //}

        //    string username = "venkata.murakunda@datasemantics.in";
        //    string password = "Khammam2";
        //    string clientId = System.Configuration.ConfigurationManager.AppSettings["ClientId"];
        //    string tenant = System.Configuration.ConfigurationManager.AppSettings["Tenant"];

        //    string authority = String.Format(System.Globalization.CultureInfo.InvariantCulture, System.Configuration.ConfigurationManager.AppSettings["Authority"], tenant);
        //    string[] scopes = new string[] { "user.read"};
        //    IPublicClientApplication app;
        //    app = PublicClientApplicationBuilder.Create(clientId).WithAuthority(authority).Build();
        //    var securePassword = new SecureString();
        //    foreach (char c in password.ToCharArray())
        //        securePassword.AppendChar(c);
        //    var result = await app.AcquireTokenByUsernamePassword(scopes, username, securePassword).ExecuteAsync();
        //    return result.IdToken;            
        //}
        [HttpPost]
        public ActionResult Index(LoginModel loginModel)
        {
            string Baseurl = ConfigurationManager.AppSettings["baseURL"] + "api/AuthenticateUser";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //HTTP POST
                var postTask = client.PostAsJsonAsync<LoginModel>(Baseurl, loginModel);               
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<string>();
                    readTask.Wait();
                    if (readTask.Result == "Authentication failed")
                    {
                        ViewBag.ErrorMessage = "The Email and/or Password are incorrect";
                    }
                    else
                    {
                        Session["UserName"] = readTask.Result;
                        Session["CurrentDateTime"] = DateTime.Now.ToString();
                        return RedirectToAction("Index", "Administration");
                    }
                }
            }


            return View();
        }


        // Signs the user out and clears the cache of access tokens.  
        public void SignOut()
        {

            //HttpContext.GetOwinContext().Authentication.SignOut(
            //    OpenIdConnectAuthenticationDefaults.AuthenticationType, CookieAuthenticationDefaults.AuthenticationType);

            HttpContext.GetOwinContext().Authentication.SignOut(
                    OpenIdConnectAuthenticationDefaults.AuthenticationType,
                    CookieAuthenticationDefaults.AuthenticationType);

        }
    }
}