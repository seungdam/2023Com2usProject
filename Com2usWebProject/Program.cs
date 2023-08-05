using ZLogger;
using Com2usWebProject.Services;
using StackExchange.Redis;

using Com2usWebProject.ModelRedis;


// Program.cs ���� �ֻ��� Ŭ���� �̵����� �� ���Ͽ� ���Եȴٰ� ���� �ȴ�.
// Res , Req(��û �����)�� ó���ϴ� ���� ���
var builder = WebApplication.CreateBuilder(args);



// Logging �����ϱ�
Host.CreateDefaultBuilder()
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();

        logging.AddZLoggerConsole(options =>
        {
            options.EnableStructuredLogging = true;
        });
  });

// �̹� ���ǵ� ���񽺵��̴�.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ����ڰ�(����) ������ ���񽺵��� �����ֱ� �����ϱ�  ������ ���� �׻� <�������̽�, ���� ��� Ŭ����(?)>
// Transient : �ϳ��� ������ ������ ����. ���� ���� X
// Scoped : �ϳ��� ��û�� �ν��Ͻ� �ѹ�. ���� �����Ϸ��� ���
// SigleTone : �� ����� �� ���� ����, �ν��Ͻ��� ����. ���񽺸� �����ϴ� ��� ���
builder.Services.AddTransient<InterfaceAccountDB, AccountDB>();
// �̱��� ����

// Redis���� ĳ�� �����ͺ��̽��� ���������� �����ϹǷ� �̱������� �����ϴ� ���� �����ϰڴ�.
builder.Services.AddSingleton<InterfaceRedisDB, RedisDB>();

builder.Logging.ClearProviders();
builder.Logging.AddZLoggerConsole();


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// ���� �� ����

app.UseRouting();

app.Run();