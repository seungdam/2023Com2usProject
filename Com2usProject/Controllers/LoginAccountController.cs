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
    

    public LoginAccountController(ILogger<LoginAccountController> logger, IAccountDb accountDb)
    {
        _logger = logger;
        _accountDb = accountDb;
    }

    [HttpPost]
    public async Task<LoginAccountRes> Login(AccountReq request)
    {
        LoginAccountRes response = new LoginAccountRes();
        try
        {
            var resultValue = await _accountDb.VerifyAccount(request.Email, request.Password);
            if (resultValue == CSCommon.ErrorCode.ErrorNone)
            {
                var rngCsp = new RNGCryptoServiceProvider();
                byte[] Token = new byte[20];

                rngCsp.GetNonZeroBytes(Token);
                response.AuthToken = request.Password + Convert.ToBase64String(Token);
                response.Result = resultValue;
                rngCsp.Dispose();
            }
            else
            {
                response.AuthToken = "None";
                response.Result = resultValue;
            }
        }
        catch (Exception ex)
        {
            
        }
        return response;
    }
}
