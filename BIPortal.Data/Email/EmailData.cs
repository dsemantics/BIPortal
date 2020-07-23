using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;

namespace BIPortal.Data.Email
{
    public class EmailData
    {
        public void SendEmail(string fromEmail, string pwd, string smtpHost, int smtpPort, string toEmail, string subject, string body)
        {
            MailMessage mailmessage = new MailMessage(fromEmail, toEmail);
            mailmessage.Subject = subject;
            mailmessage.Body = body;
            mailmessage.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = smtpHost;
            smtp.Port = smtpPort;
            smtp.EnableSsl = true;
            NetworkCredential networkcredential = new NetworkCredential(fromEmail, pwd);
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = networkcredential;
            smtp.Send(mailmessage);
        }
    }
}
