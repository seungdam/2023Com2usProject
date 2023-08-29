using Com2usProject.MiddleWare;
using Com2usProject.Repository;
using Com2usProject.Service;
using Com2usProject.ServiceInterface;
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

builder.Services.AddTransient<IAccountDb, HandleAccount>();
builder.Services.AddTransient<IInGameDb, InGameDb>();
builder.Services.AddTransient<IPlayableCharacterStatusData, HandleCharacterStatusData>();
builder.Services.AddTransient<IPlayerInventoryData, HandleInventoryData>();
builder.Services.AddTransient<IPlayerMailBoxData, HandleMailBoxData>();
builder.Services.AddSingleton<IRedisDb, RedisDb>();
builder.Services.AddControllers();

var app = builder.Build();
app.UseMiddleware<MiddleWareTokenVerifier>();
app.MapControllers();

app.Run(appConfig["ServerAddress"]);
