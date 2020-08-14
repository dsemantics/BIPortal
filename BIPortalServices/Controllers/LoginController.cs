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
using Microsoft.IdentityModel.Clients.ActiveDirectory;

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

        [Route("api/AuthenticatePowerBIUser")]
        [HttpPost]
        public async Task<string> AuthenticatePowerBIUser(LoginModel loginModel)
        {
            try
            {
                //The client id that Azure AD created when you registered your client app.
                string clientID = System.Configuration.ConfigurationManager.AppSettings["powerBIClientId"];

                //RedirectUri you used when you register your app.
                //For a client app, a redirect uri gives Azure AD more details on the application that it will authenticate.
                // You can use this redirect uri for your client app
                //string redirectUri = "urn:ietf:wg:oauth:2.0:oob";

                //Resource Uri for Power BI API
                string resourceUri = System.Configuration.ConfigurationManager.AppSettings["powerBIresourceUri"];

                //OAuth2 authority Uri
                string authorityUri = System.Configuration.ConfigurationManager.AppSettings["powerBIauthorityUri"];

                string username = loginModel.UserName;
                string password = loginModel.Password;
                //Get access token:
                // To call a Power BI REST operation, create an instance of AuthenticationContext and call AcquireToken             
                // AcquireToken will acquire an Azure access token
                // Call AcquireToken to get an Azure token from Azure Active Directory token issuance endpoint
                AuthenticationContext authContext = new AuthenticationContext(authorityUri);                
                var credential = new UserPasswordCredential(username, password);
                Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationResult res = authContext.AcquireTokenAsync(resourceUri, clientID, credential).Result;
                                
                return res.AccessToken;
            }
            catch (Exception ex)
            {
                return "PowerBI Authentication failed";
            }
        }
    }
}
