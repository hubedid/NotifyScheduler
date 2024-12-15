using Notify.Repository.Email;
using Quartz;

namespace Notify.Services;

public class ReminderJob : IJob
{
    private readonly ILogger<ReminderJob> _logger;
    private readonly IEmailSenderRepository _emailSenderRepository;

    public ReminderJob(ILogger<ReminderJob> logger, IEmailSenderRepository emailSenderRepository)
    {
        _logger = logger;
        _emailSenderRepository = emailSenderRepository;
    }
    public async Task Execute(IJobExecutionContext context)
    {

        JobDataMap jobData = context.JobDetail.JobDataMap;
        string? username = jobData.GetString("Username");
        _logger.LogInformation($"exec job: {username}");
        string? email = jobData.GetString("Email");
        string? judul = jobData.GetString("Judul");
        string? deskripsi = jobData.GetString("Deskripsi");
        string? deadline = jobData.GetString("Deadline");

        TimeSpan remainingTime = DateTime.Parse(deadline) - DateTime.Now;

        string body = $"""
                      <h1>Halo, {username}</h1>
                      <p>Kamu ada deadline {deadline}</p>
                      <p>Sisa waktu : {remainingTime.Hours} jam</p>
                      <p>Judul : {judul}</p>
                      <p>Deskripsi : {deskripsi}</p>
                      """;

        bool sender = await _emailSenderRepository.SendEmail(email, body);

        _logger.LogInformation($"sender status: {sender}");
    }
}
