using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Threading.Tasks;
using System.Web.Http;
using BIPortal.Models;

namespace BIPortalServices.Controllers
{
    public class LoginController : ApiController
    {
        [Route("api/AuthenticateUser")]
        [HttpPost]
        public async Task<string> AuthenticateUser(LoginModel loginModel)
        {
            try
            {
                string username = loginModel.UserName; 
                string password = loginModel.Password; 
                string clientId = System.Configuration.ConfigurationManager.AppSettings["ClientId"];
                string tenant = System.Configuration.ConfigurationManager.AppSettings["Tenant"];

                string authority = String.Format(System.Globalization.CultureInfo.InvariantCulture, System.Configuration.ConfigurationManager.AppSettings["Authority"], tenant);
                string[] scopes = new string[] { "user.read" };
                IPublicClientApplication app;
                app = PublicClientApplicationBuilder.Create(clientId).WithAuthority(authority).Build();
                var securePassword = new SecureString();
                foreach (char c in password.ToCharArray())
                    securePassword.AppendChar(c);
                var result = await app.AcquireTokenByUsernamePassword(scopes, username, securePassword).ExecuteAsync();

                //return result.IdToken;
                return result.Account.Username;
            }
            catch (Exception ex)
            {
                return "Authentication failed";
            }
        }
    }
}
