using Com2usProject.Service;
using Microsoft.AspNetCore.Builder;
using ZLogger;

var builder = WebApplication.CreateBuilder(args);

IConfiguration appConfig = builder.Configuration;

builder.Services.Configure<DbConnectionStrings>(appConfig.GetSection(nameof(DbConnectionStrings)));

// 로그 설정
builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddZLoggerConsole();
});

// Add services to the container.

builder.Services.AddTransient<IAccountDb, MySqlAccountDb>();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();


var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

app.MapControllers();

app.Run();
