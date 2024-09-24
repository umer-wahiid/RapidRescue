namespace RapidResuce.Interfaces
{
    public interface IEmailService
    {
        void SendOtpEmail(string toEmail, string username, string otp);
        void SendConfirmationEmail(string toEmail, string username);
        void SendResetPasswordEmail(string toEmail, string username, string callbackUrl);
    }
}
