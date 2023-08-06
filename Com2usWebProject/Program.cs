using ZLogger;
using Com2usWebProject.Services;
using Com2usWebProject.ModelRedis;


// Program.cs ���� �ֻ��� Ŭ���� �̵����� �� ���Ͽ� ���Եȴٰ� ���� �ȴ�.
// Res , Req(��û �����)�� ó���ϴ� ���� ���
var builder = WebApplication.CreateBuilder(args);


IConfiguration appConfig = builder.Configuration; // appsettings�� ���� config������ �����´�

builder.Services.Configure<DBInfo>(appConfig.GetSection(nameof(DBInfo)));

// Logging �����ϱ� ZLogger ���̺귯���� ����
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


// ����ڰ�(����) ������ ���񽺵��� �����ֱ� �����ϱ�  ������ ���� �׻� <�������̽�, ���� ��� Ŭ����(?)>
// Transient : �ϳ��� ������ ������ ����. ���� ���� X
// Scoped : �ϳ��� Ŭ���̾�Ʈ ��û�� �ν��Ͻ� �ѹ�. ���� �����Ϸ��� ���
// SigleTone : �� ����� �� ���� ����, �ν��Ͻ��� ����. ���񽺸� �����ϴ� ��� ���

// �̱��� ����
// Redis���� ĳ�� �����ͺ��̽��� ���������� �����ϹǷ� �̱������� �����ϴ� ���� �����ϰڴ�.
//builder.Services.AddSingleton<InterfaceRedisDB, RedisDB>();

// �α��� ���� ���, ��û�� ���� ������ �ƴϰ� Ŭ���̾�Ʈ�� ��û�� ���� �ʿ��ϱ� ������ Scoped�� ����
builder.Services.AddTransient<IAccountDB, AccountDB>();

builder.Services.AddControllers();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Logging.ClearProviders();
builder.Logging.AddZLoggerConsole();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ���� �� ����
//
app.UseHttpsRedirection();
//app.MapControllers();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

app.Run();