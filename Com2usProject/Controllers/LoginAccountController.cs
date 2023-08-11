using Com2usProject.ReqResModel;
using Com2usProject.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace Com2usProject.Controllers;

[Route("Controller/Login")]
[ApiController]
public class LoginAccountController : ControllerBase
{
    private readonly IAccountDb _accountDb;
    private readonly ILogger<LoginAccountController> _logger;

    public LoginAccountController(ILogger<LoginAccountController> logger, IAccountDb accountDb)
    {
        _logger = logger;
        _accountDb = accountDb;
    }

    [HttpPost]
    public async Task<LoginAccountRes> Login(AccountReq request)
    {
        LoginAccountRes response = new LoginAccountRes();
        var resultValue = await _accountDb.VerifyAccount(request.Email, request.Password);
        if(resultValue == CSCommon.ErrorCode.ErrorNone)
        {
            var rngCsp = new RNGCryptoServiceProvider();
            byte[] Token = new byte[20];

            rngCsp.GetNonZeroBytes(Token);
            response.UserVerifyString = request.Password + Convert.ToBase64String(Token);
            response.Result = resultValue;
            rngCsp.Dispose();
        }
        else
        {
            response.UserVerifyString = "None";
            response.Result = resultValue;
        }
        return response;
    }
}
