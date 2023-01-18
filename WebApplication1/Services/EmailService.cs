using System.Net;
using System.Net.Mail;
using WebApplication1.Abstractions.Services;

namespace WebApplication1.Services
{
    public class EmailService:IEmailService
    {
        IConfiguration _configuration { get; }

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Send(string mailTo, string subject, string body, bool isBodyHtml = false) 
        { 
            using SmtpClient smtp= new SmtpClient(_configuration["Email:Host"], 
                    Convert.ToInt32(_configuration["Email:Port"]));
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential(_configuration["Email:Login"], _configuration["Email:Password"]);

            MailAddress from = new MailAddress(_configuration["Email:Login"], "Kapital Bank");
            MailAddress to = new MailAddress(mailTo);   //"tu7fmvb99@code.edu.az"

            using MailMessage message = new MailMessage(from, to);
            message.Subject = subject; //"Bank Hesabiniz Tehlukededir.";
            message.Body = body; //"Size gelen 6 reqemli sifreni bize demeyiniz xaish olunur.";
            message.IsBodyHtml= isBodyHtml;
            smtp.Send(message);
        }

    }
}
