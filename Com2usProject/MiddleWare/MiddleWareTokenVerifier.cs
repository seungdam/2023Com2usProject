using Com2usProject.ReqResModel;
using Com2usProject.Repository;
using System.Net;
using System.Text;
using System.Net.Mime;
using Newtonsoft.Json.Linq;

namespace Com2usProject.MiddleWare;

public class MiddleWareTokenVerifier
{
    readonly RequestDelegate _next;
    readonly IRedisDb _redisDb;
    readonly ILogger<MiddleWareTokenVerifier> _logger;
    string UserTocken = "AuthToken";

    public MiddleWareTokenVerifier(RequestDelegate next, IRedisDb redisDb, ILogger<MiddleWareTokenVerifier> logger)
    {
        _next = next;
        _redisDb = redisDb;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {

        try
        {
           
            _logger.LogInformation($"[Request Path] : {context.Request.Path}");
            // 회원 가입을 수행하거나, 로그인 요청을 했을 당시에는 처리하지 않음
            if (context.Request.Path.StartsWithSegments("/Register") || context.Request.Path.StartsWithSegments("/Login"))
            {
                _logger.LogInformation($"[MiddleWareTokenVerifier] Result: None Check Path");
            }
            else
            {

                var bodyContents = await GetBodyStringFromRequest(context.Request);
                var token = bodyContents["AuthToken"].ToString();
                var result = await _redisDb.CheckAuthTokenExist(token);
                if (!result) context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // error code 발생
            }
        }
        catch(Exception e)
        {
            _logger.LogError("Something Exception Occur At TokenCheckMiddleWare");
        }

        //입력받은 Headers  'AuthToken' 값을 조회후 값이없다면 errcode 출력


        await _next(context);
    }

    async Task<JObject> GetBodyStringFromRequest(HttpRequest request)
    {
       
            request.EnableBuffering();
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            //get body string here...
            var requestContents = Encoding.UTF8.GetString(buffer);

            request.Body.Position = 0;  //rewinding the stream to 0
            JObject jsonBody = JObject.Parse(requestContents);
            return jsonBody;
        

    }
  
}
