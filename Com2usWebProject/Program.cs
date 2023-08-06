using ZLogger;
using Com2usWebProject.Services;
using Com2usWebProject.ModelRedis;


// Program.cs 가장 최상위 클래스 미들웨어는 이 파일에 포함된다고 보면 된다.
// Res , Req(요청 응답들)을 처리하는 것을 담당
var builder = WebApplication.CreateBuilder(args);


IConfiguration appConfig = builder.Configuration; // appsettings를 통해 config정보를 가져온다

builder.Services.Configure<DBInfo>(appConfig.GetSection(nameof(DBInfo)));

// Logging 설정하기 ZLogger 라이브러리로 설정
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


// 사용자가(내가) 정의한 서비스들의 생명주기 설정하기  셋팅할 때는 항상 <인터페이스, 실제 사용 클래스(?)>
// Transient : 하나당 여러번 생성도 가능. 상태 저장 X
// Scoped : 하나의 클라이언트 요청당 인스턴스 한번. 상태 유지하려는 경우
// SigleTone : 앱 종료될 때 까지 유지, 인스턴스를 재사용. 서비스를 재사용하는 경우 사용

// 싱글톤 빌드
// Redis같은 캐시 데이터베이스는 지속적으로 재사용하므로 싱글톤으로 구현하는 것이 적절하겠다.
//builder.Services.AddSingleton<InterfaceRedisDB, RedisDB>();

// 로그인 같은 경우, 요청이 잦은 사항이 아니고 클라이언트가 요청할 때만 필요하기 때문에 Scoped로 설정
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

// 빌드 후 수행
//
app.UseHttpsRedirection();
//app.MapControllers();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

app.Run();