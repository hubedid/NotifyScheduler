using Notify.Repository.Email;
using Notify.Repository.Tugas;
using Notify.Services;

namespace Notify.Endpoints;

public class Tugas
{
    private readonly ILogger<Tugas> _logger;
    private readonly QuartzScheduler _scheduler;
    private readonly ITugasRepository _tugasRepository;
    private readonly IEmailSenderRepository _emailSenderRepository;

    public Tugas(
        ILogger<Tugas> logger,
        QuartzScheduler scheduler,
        ITugasRepository tugasRepository, 
        IEmailSenderRepository emailSenderRepository)
    {
        _logger = logger;
        _scheduler = scheduler;
        _tugasRepository = tugasRepository;
        _emailSenderRepository = emailSenderRepository;
    }

    public async Task<IEnumerable<TugasModel>> GetAllTugas()
    {
        //bool tes = await _emailSenderRepository.SendEmail("mohamad11ipa520@gmail.com", "ajarins yahaha hayukkk");
        //_logger.LogInformation($"sender status : {tes}");

        //await _scheduler.ListRegisteredJobs();
        await _scheduler.ListJobAndTriggers();


        return _tugasRepository.GetAllTugas();
    }

    public TugasModel GetTugasById(string tugasId)
    {
        //bool tes = await _emailSenderRepository.SendEmail("mohamad11ipa520@gmail.com", "ajarins yahaha hayukkk");
        //_logger.LogInformation($"sender status : {tes}");

        //await _scheduler.ListRegisteredJobs();
        //await _scheduler.ListJobAndTriggers();


        return _tugasRepository.GetTugasById(tugasId);
    }

    public async Task<dynamic> TugasCallback(string tugasId)
    {
       string jobKey = await _scheduler.AddNewScheduleByTugasId(tugasId);

       dynamic schedule = await _scheduler.ListTriggersByJobKey(jobKey);

       return schedule;
    }

}


