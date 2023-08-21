using Com2usProject.ReqResModel;
using Com2usProject.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using ZLogger;

namespace Com2usProject.Controllers;

// 2023-08-07~ 2023-08-10 
// AccountController


[ApiController]
[Route("register/[controller]")]
public  class RegisterAccountController : ControllerBase 
{
    private readonly IAccountDb _accountDb;
    private readonly ILogger<RegisterAccountController> _logger;

    public RegisterAccountController(ILogger<RegisterAccountController> logger, IAccountDb accountDb)
    {
        _logger = logger;
        _accountDb = accountDb;
    }

    [HttpPost]
    public async Task<RegisterAccountRes> Register(AccountReq request)
    {
        RegisterAccountRes response = new RegisterAccountRes();

    
        var resultValue = await _accountDb.RegisterAccount(request.Email, request.Password);
        response.ErrorCode = resultValue;
        return response;
    }
    

    

}
