namespace WebApplication1.Abstractions.Services
{
    public interface IEmailService
    {
        void Send(string mailTo, string subject, string body, bool isBodyHtml = false);
    }
}
