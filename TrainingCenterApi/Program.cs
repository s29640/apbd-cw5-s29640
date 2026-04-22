using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TrainingCenterApi.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

AppData.Seed();

app.MapControllers();

app.Run();