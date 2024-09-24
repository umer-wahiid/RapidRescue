namespace RapidResuce.Interfaces
{
    public interface IEmailHelper
    {
        void Send(string toEmail, string subject, string body, bool isHtmlBody);
    }
}
