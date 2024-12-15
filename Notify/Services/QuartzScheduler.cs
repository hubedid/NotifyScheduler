using Notify.Endpoints;
using Notify.Repository.Tugas;
using Quartz;
using Quartz.Impl.Matchers;

namespace Notify.Services;

public class QuartzScheduler
{
    private readonly ILogger<QuartzScheduler> _logger;
    private readonly ITugasRepository _tugasRepository;
    private readonly IScheduler _scheduler;

    public QuartzScheduler(ILogger<QuartzScheduler> logger, IScheduler scheduler, ITugasRepository tugasRepository)
    {
        _logger = logger;
        _tugasRepository = tugasRepository;
        _scheduler = scheduler;
    }

    public async Task StartAsync()
    {
        await _scheduler.Start();

        IEnumerable<TugasModel> schedules = _tugasRepository.GetAllTugas();

        foreach (TugasModel schedule in schedules)
        {
            string jobKey = $"job-{schedule.Email}-{Guid.NewGuid()}";
            IJobDetail job = JobBuilder.Create<ReminderJob>()
                .WithIdentity(jobKey)
                .StoreDurably()
                .UsingJobData("Username", schedule.Username) 
                .UsingJobData("Email", schedule.Email)
                .UsingJobData("Judul", schedule.Judul) 
                .UsingJobData("Deskripsi", schedule.Deskripsi) 
                .UsingJobData("Deadline", schedule.Deadline.ToString("f"))
                .Build();

            var triggers = new List<ITrigger>
            {
                TriggerBuilder.Create()
                    .WithIdentity($"trigger-1day-{Guid.NewGuid()}")
                    .ForJob(job)
                    .StartAt(schedule.Deadline.AddDays(-1))
                    .WithSimpleSchedule(x => x.WithMisfireHandlingInstructionNextWithExistingCount())
                    .Build(),
                TriggerBuilder.Create()
                    .WithIdentity($"trigger-6hours-{Guid.NewGuid()}")
                    .ForJob(job)
                    .StartAt(schedule.Deadline.AddHours(-6))
                    .WithSimpleSchedule(x => x.WithMisfireHandlingInstructionNextWithExistingCount())
                    .Build(),
                TriggerBuilder.Create()
                    .WithIdentity($"trigger-1hour-{Guid.NewGuid()}")
                    .ForJob(job)
                    .StartAt(schedule.Deadline.AddHours(-1))
                    .WithSimpleSchedule(x => x.WithMisfireHandlingInstructionNextWithExistingCount())
                    .Build()
            };

            await _scheduler.AddJob(job, replace: true);

            foreach (ITrigger trigger in triggers)
            {
                await _scheduler.ScheduleJob(trigger);
            }

            _tugasRepository.SetJobKeyByTugasId(schedule.TugasId, jobKey);
        }
    }


    public async Task<string> AddNewScheduleByTugasId(string tugasId)
    {
        await _scheduler.Start();

        TugasModel schedule = _tugasRepository.GetTugasById(tugasId);

        string jobKey = $"job-{schedule.Email}-{Guid.NewGuid()}";
        IJobDetail job = JobBuilder.Create<ReminderJob>()
            .WithIdentity(jobKey)
            .StoreDurably()
            .UsingJobData("Username", schedule.Username)
            .UsingJobData("Email", schedule.Email)
            .UsingJobData("Judul", schedule.Judul)
            .UsingJobData("Deskripsi", schedule.Deskripsi)
            .UsingJobData("Deadline", schedule.Deadline.ToString("f"))
            .Build();

        var triggers = new List<ITrigger>
        {
            TriggerBuilder.Create()
                .WithIdentity($"trigger-1day-{Guid.NewGuid()}")
                .ForJob(job)
                .StartAt(schedule.Deadline.AddDays(-1))
                .WithSimpleSchedule(x => x.WithMisfireHandlingInstructionNextWithExistingCount())
                .Build(),
            TriggerBuilder.Create()
                .WithIdentity($"trigger-6hours-{Guid.NewGuid()}")
                .ForJob(job)
                .StartAt(schedule.Deadline.AddHours(-6))
                .WithSimpleSchedule(x => x.WithMisfireHandlingInstructionNextWithExistingCount())
                .Build(),
            TriggerBuilder.Create()
                .WithIdentity($"trigger-1hour-{Guid.NewGuid()}")
                .ForJob(job)
                .StartAt(schedule.Deadline.AddHours(-1))
                .WithSimpleSchedule(x => x.WithMisfireHandlingInstructionNextWithExistingCount())
                .Build()
        };

        await _scheduler.AddJob(job, replace: true);

        foreach (ITrigger trigger in triggers)
        {
            await _scheduler.ScheduleJob(trigger);
        }

        _tugasRepository.SetJobKeyByTugasId(tugasId, jobKey);

        return jobKey;
    }

    public async Task ListRegisteredJobs()
    {

        IReadOnlyCollection<JobKey> jobKeys = await _scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());

        _logger.LogInformation("Daftar semua job terdaftar:");

        foreach (JobKey jobKey in jobKeys)
        {
            _logger.LogInformation($"Job Terdaftar: {jobKey}");
        }

    }

    public async Task ListJobAndTriggers()
    {
        IReadOnlyCollection<JobKey> jobKeys = await _scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());

        foreach (JobKey jobKey in jobKeys)
        {
            IReadOnlyCollection<ITrigger> triggers = await _scheduler.GetTriggersOfJob(jobKey);

            _logger.LogInformation($"Job: {jobKey}");
            foreach (ITrigger trigger in triggers)
            {
                _logger.LogInformation($"Trigger: {trigger.Key}, NextFireTime: {trigger.GetNextFireTimeUtc()}");
            }
        }
    }

    public async Task<dynamic> ListTriggersByJobKey(string jobId)
    {
        var jobKey = new JobKey(jobId);
        IReadOnlyCollection<ITrigger> triggers = await _scheduler.GetTriggersOfJob(jobKey);
        return new
        {
            Job = jobKey,
            Trigers = triggers.Select( t => new
            {
                Triger = t.Key,
                NextFireTime = t.GetNextFireTimeUtc()
            })
        };
    }

}
