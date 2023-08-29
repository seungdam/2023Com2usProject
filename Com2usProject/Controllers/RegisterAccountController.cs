using Com2usProject.ReqResModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZLogger;
using Com2usProject.ServiceInterface;

namespace Com2usProject.Controllers;

// 2023-08-07~ 2023-08-10 
// AccountController


[ApiController]
[Route("api/[controller]")]
public  class RegisterAccountController : ControllerBase 
{
   readonly IAccountDb _accountDb;
   readonly ILogger<RegisterAccountController> _logger;

    public RegisterAccountController(ILogger<RegisterAccountController> logger, IAccountDb accountDb)
    {
        _logger = logger;
        _accountDb = accountDb;
    }

    [HttpPost("/Register")]
    public async Task<RegisterAccountRes> Register(AccountReq request)
    {
        RegisterAccountRes response = new RegisterAccountRes();

    
        var resultValue = await _accountDb.RegisterAccount(request.Email, request.Password);
        response.ErrorCode = resultValue;
        return response;
    }
    

    

}
