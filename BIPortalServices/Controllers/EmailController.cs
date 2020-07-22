using AutoMapper;
using BIPortal.Data.Email;
using BIPortal.DTO;
using BIPortal.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BIPortalServices.Controllers
{
    public class EmailController : ApiController
    {
        [HttpPost]
        [Route("api/SendEmail")]
        public IHttpActionResult SendEmail(EmailModel emailModel)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest("Not a valid model");
                }
                else
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<EmailModel, EmailDTO>();                        
                    });
                    IMapper mapper = config.CreateMapper();

                    var emailResultData = mapper.Map<EmailModel, EmailDTO>(emailModel);

                    string powerBIUserName = ConfigurationManager.AppSettings["powerBIUserName"];
                    string powerBIPWD = ConfigurationManager.AppSettings["powerBIPWD"];
                    string smtpHost = ConfigurationManager.AppSettings["smtpHost"];
                    int smtpPort = Convert.ToInt32(ConfigurationManager.AppSettings["smtpPort"]);

                    EmailData emailData = new EmailData();

                    emailData.SendEmail(powerBIUserName, powerBIPWD, smtpHost, smtpPort, emailResultData.ToEmail, emailResultData.Subject, emailResultData.Body);

                    return Created("api/SendEmail", true);
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Send email failed");
            }
        }
    }
}
