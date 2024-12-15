namespace Notify.Repository.Email;

public interface IEmailSenderRepository
{
    Task<bool> SendEmail(string email, string body);
}
