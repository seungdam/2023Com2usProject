using Com2usProject.ReqResModel;
using Com2usProject.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Cryptography;
using ZLogger;

namespace Com2usProject.Controllers;


[ApiController]
[Route("login/[controller]")]
public class LoginAccountController : ControllerBase
{
    readonly IAccountDb _accountDb;
    readonly IRedisDb _redisTokenDb;
    readonly ILogger<LoginAccountController> _logger;
    

    public LoginAccountController(ILogger<LoginAccountController> logger, IAccountDb accountDb, IRedisDb redisDb)
    {
        _logger = logger;
        _accountDb = accountDb;
        _redisTokenDb = redisDb;
    }

    [HttpPost]
    public async Task<LoginAccountRes> Login(AccountReq request)
    {
        LoginAccountRes response = new LoginAccountRes();
        try
        {
            var verifyResult = await _accountDb.VerifyAccount(request.Email, request.Password);
            
            if (verifyResult.Item1 == CSCommon.ErrorCode.ErrorNone)
            {
                var rngCsp = new RNGCryptoServiceProvider();
                byte[] Token = new byte[20];

                rngCsp.GetNonZeroBytes(Token);
                response.AuthToken = request.Password + Convert.ToBase64String(Token);
                response.Id = verifyResult.Item2;

                rngCsp.Dispose();

                var addTokenReult = await _redisTokenDb.AddAuthToken(response.AuthToken, request.Email);
                
                if(addTokenReult != CSCommon.ErrorCode.ErrorNone)
                {
                    response.AuthToken = "None";
                    response.ErrorCode = addTokenReult;
                   
                }
            }
            else
            {
                response.AuthToken = "None";
                response.ErrorCode = verifyResult.Item1;
            }
        }
        catch (Exception ex)
        {
            _logger.ZLogError("Something Error Occur. Plz Check This Code.");   
        }
        return response;
    }
}
