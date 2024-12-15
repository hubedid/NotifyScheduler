using MySql.Data.MySqlClient;
using Notify.Endpoints;
using Notify.Repository.Email;
using Notify.Repository.Tugas;
using Notify.Services;
using Quartz;
using SqlKata.Compilers;
using SqlKata.Execution;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<QueryFactory>((e) =>
{
    var connection = new MySqlConnection(builder.Configuration.GetConnectionString("MySql"));
    var compiler = new MySqlCompiler();
    return new QueryFactory(connection, compiler);
});

builder.Services.AddQuartz();
builder.Services.AddScoped<QuartzScheduler>();
builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

builder.Services.AddScoped<ITugasRepository, TugasRepository>();
builder.Services.AddScoped<IEmailSenderRepository, EmailSenderRepository>();
builder.Services.AddScoped<Tugas>();
builder.Services.AddSingleton<IScheduler>(provider =>
{
    ISchedulerFactory schedulerFactory = provider.GetRequiredService<ISchedulerFactory>();
    return schedulerFactory.GetScheduler().Result;
});

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

using (IServiceScope scope = app.Services.CreateScope())
{
    QuartzScheduler scheduler = scope.ServiceProvider.GetRequiredService<QuartzScheduler>();
    await scheduler.StartAsync();
}

app.MapGet("/tugas", (Tugas handler) => handler.GetAllTugas())
.WithName("GetAllTugas")
.WithOpenApi();

app.MapGet("/tugas/{tugasId}", (string tugasId, Tugas handler) => handler.GetTugasById(tugasId))
    .WithName("GetTugasById")
    .WithOpenApi();

app.MapPost("/tugas/{tugasId}/callback", (string tugasId, Tugas handler) => handler.TugasCallback(tugasId))
    .WithName("TugasCallback")
    .WithOpenApi();

app.Run();
