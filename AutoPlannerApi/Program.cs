using AutoPlannerApi.Data.UserData.Realization;
using AutoPlannerApi.Data.UserData.Interface;
using AutoPlannerApi.Domain.UserDomain.Interface;
using AutoPlannerApi.Domain.UserDomain.Realization;
using AutoPlannerApi.Domain.TaskDomain.Interface;
using AutoPlannerApi.Domain.TaskDomain.Realization;
using AutoPlannerApi.Data.TaskData.Interface;
using AutoPlannerApi.Data.TaskData.Realization;
using AutoPlannerApi.Domain.TimeTableDomain.Interface;
using AutoPlannerApi.Domain.TimeTableDomain.Realization;
using AutoPlannerApi.Data.TimeTableData.Interface;
using AutoPlannerApi.Data.TimeTableData.Realization;
using AutoPlannerApi.Data.PlanningTaskData.Interface;
using AutoPlannerApi.Data.PlanningTaskData.Realization;
using Microsoft.Extensions.DependencyInjection;
using AutoPlannerApi.TelegramServices.Telegram.Handlers.CommandHandlers;
using AutoPlannerApi.TelegramServices.Telegram.Handlers;
using AutoPlannerApi.TelegramServices.Notifications;
using AutoPlannerApi.TelegramServices.Telegram;
using Hangfire;
using AutoPlannerApi.TelegramServices.BackgroundJobs;
using Hangfire.MemoryStorage;
using AutoPlannerApi.TelegramServices.Telegram.Models;
using AutoPlannerApi.Data.NotificationData.Interface;
using AutoPlannerApi.Data.NotificationData.Realization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("ConnectionDB");
builder.Services.Configure<TelegramBotConfig>(builder.Configuration.GetSection("TelegramBot"));

builder.Services.AddSingleton<IUserService, UserClassicService>();
//builder.Services.AddSingleton<IUserDatabaseRepository, UserRamRepository>();
builder.Services.AddSingleton<ITaskService, TaskClassicService>();
//builder.Services.AddSingleton<ITaskDatabaseRepository, TaskRamRepository>();
builder.Services.AddSingleton<ITimeTableItemService, TimeTableItemClassicService>();
//builder.Services.AddSingleton<ITimeTableItemDatabaseRepository, TimeTableItemRamRepository>();
builder.Services.AddSingleton<ITelegramLinkingService, TelegramLinkingService>();

builder.Services.AddScoped<StartCommandHandler>();
builder.Services.AddScoped<TasksCommandHandler>();
builder.Services.AddScoped<HelpCommandHandler>();
builder.Services.AddScoped<TelegramUpdateHandler>();

builder.Services.AddSingleton<ITelegramBotService, TelegramBotService>();
builder.Services.AddHostedService(provider =>
    provider.GetRequiredService<ITelegramBotService>() as TelegramBotService);

builder.Services.AddScoped<INotificationSchedulerService, NotificationSchedulerService>();

builder.Services.AddHangfire(config => config.UseMemoryStorage());
builder.Services.AddHangfireServer();

builder.Services.AddSingleton<IUserDatabaseRepository, UserPostgresRepository>(provider =>
    new UserPostgresRepository(connectionString));
builder.Services.AddSingleton<ITaskDatabaseRepository, TaskPostgresRepository>(provider =>
    new TaskPostgresRepository(connectionString, provider.GetRequiredService<ILogger<TaskPostgresRepository>>()));
builder.Services.AddSingleton<ITimeTableItemDatabaseRepository, TimeTableItemPostgresRepository>(provider =>
    new TimeTableItemPostgresRepository(connectionString, provider.GetRequiredService<ILogger<TimeTableItemPostgresRepository>>()));
builder.Services.AddSingleton<IPlanningTaskDatabaseRepository, PlanningTaskPostgresRepository>(provider =>
    new PlanningTaskPostgresRepository(connectionString, provider.GetRequiredService<ILogger<PlanningTaskPostgresRepository>>()));

builder.Services.AddScoped<ISentNotificationRepository, SentNotificationPostgresRepository>(provider =>
    new SentNotificationPostgresRepository(connectionString, provider.GetRequiredService<ILogger<SentNotificationPostgresRepository>>()));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHangfireDashboard("/hangfire");

/*RecurringJob.AddOrUpdate<TelegramNotificationJob>(
    "reschedule-notifications",
    job => job.RescheduleAllNotifications(),
    "Cron.Hourly");*/

app.UseAuthorization();

app.MapControllers();

app.Run("http://*:8080");
