using Com2usProject.ReqResModel;
using Com2usProject.Service;
using System.Net;

namespace Com2usProject.MiddleWare;

public class TokenVerifier
{
    readonly RequestDelegate _next;
    readonly IRedisDb _redisDb;
    readonly ILogger<TokenVerifier> _logger;
    string UserTocken = "AuthToken";

    public TokenVerifier(RequestDelegate next, IRedisDb redisDb, ILogger<TokenVerifier> logger)
    {
        _next = next;
        _redisDb = redisDb;
        _logger = logger;
    }

    public async Task CheckUserTokenAsync(HttpContext context)
    {

        try
        {
            // 회원 가입을 수행하거나, 로그인 요청을 했을 당시에는 처리하지 않음
            if (context.Request.Path.StartsWithSegments("/Register") || context.Request.Path.StartsWithSegments("/Login"))
            {
                _logger.LogInformation($"None Check Request");
            }
            else
            {
                var Token = context.Request.Headers["AuthToken"];
                var result = await _redisDb.VerifyUserToken(Token);
                if (!result) context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // error code 발생
            }
        }
        catch(Exception e)
        {
            _logger.LogError("something Error detection");
        }

        //입력받은 Headers  'AuthToken' 값을 조회후 값이없다면 errcode 출력


        await _next(context);
    }
}
