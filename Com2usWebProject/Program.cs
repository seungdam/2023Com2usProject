using ZLogger;
using Com2usWebProject.Services;
using StackExchange.Redis;

using Com2usWebProject.ModelRedis;


// Program.cs 가장 최상위 클래스 미들웨어는 이 파일에 포함된다고 보면 된다.
// Res , Req(요청 응답들)을 처리하는 것을 담당
var builder = WebApplication.CreateBuilder(args);



// Logging 설정하기
Host.CreateDefaultBuilder()
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();

        logging.AddZLoggerConsole(options =>
        {
            options.EnableStructuredLogging = true;
        });
  });

// 이미 정의된 서비스들이다.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 사용자가(내가) 정의한 서비스들의 생명주기 설정하기  셋팅할 때는 항상 <인터페이스, 실제 사용 클래스(?)>
// Transient : 하나당 여러번 생성도 가능. 상태 저장 X
// Scoped : 하나의 요청당 인스턴스 한번. 상태 유지하려는 경우
// SigleTone : 앱 종료될 때 까지 유지, 인스턴스를 재사용. 서비스를 재사용하는 경우 사용
builder.Services.AddTransient<InterfaceAccountDB, AccountDB>();
// 싱글톤 빌드

// Redis같은 캐시 데이터베이스는 지속적으로 재사용하므로 싱글톤으로 구현하는 것이 적절하겠다.
builder.Services.AddSingleton<InterfaceRedisDB, RedisDB>();

builder.Logging.ClearProviders();
builder.Logging.AddZLoggerConsole();


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// 빌드 후 수행

app.UseRouting();

app.Run();