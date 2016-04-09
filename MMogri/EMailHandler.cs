using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;

namespace MMogri
{
    class EMailHandler
    {
        public static void Send(string ownerEmail, string ownerPassword, string targetAdress, string subject, string body)
        {
            var fromAddress = new MailAddress(ownerEmail, "MMogri - TestServer");
            var toAddress = new MailAddress(targetAdress, "YourName!");

            SmtpClient smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(fromAddress.Address, ownerPassword),
                Timeout = 20000
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }

        }
//public static void Send(string ownerEmail, string ownerPassword, string targetAdress, string subject, string body)
//        {
//            MailMessage mail = new MailMessage();

//            mail.From = new MailAddress(ownerEmail);
//            mail.To.Add(targetAdress);
//            mail.Subject = subject;
//            mail.Body = body;

//            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
//            SmtpServer.Port = 587;
//            SmtpServer.Credentials = new System.Net.NetworkCredential(ownerEmail, ownerPassword);
//            SmtpServer.EnableSsl = true;

//            SmtpServer.Send(mail);
//        }
    }
}
