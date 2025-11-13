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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IUserService, UserClassicService>();
builder.Services.AddSingleton<IUserDatabaseRepository, UserRamRepository>();
builder.Services.AddSingleton<ITaskService, TaskClassicService>();
builder.Services.AddSingleton<ITaskDatabaseRepository, TaskRamRepository>();
builder.Services.AddSingleton<ITimeTableItemService, TimeTableItemClassicService>();
builder.Services.AddSingleton<ITimeTableItemDatabaseRepository, TimeTableItemRamRepository>();
builder.Services.AddSingleton<IPlanningTaskDatabaseRepository, PlanningTaskRamRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run("http://*:8080");
