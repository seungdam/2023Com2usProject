using ZLogger;

var builder = WebApplication.CreateBuilder(args);


// 로그 설정
builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
    logging.AddZLoggerConsole();
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


var app = builder.Build();

// Configure the HTTP request pipeline.


app.Run();
