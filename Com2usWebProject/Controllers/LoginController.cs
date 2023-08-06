using Com2usWebProject.ModelResReq;
using Com2usWebProject.Services;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace Com2usWebProject.Controllers;

[ApiController]
[Route("LoginController")]
public class LoginController : ControllerBase
 {

    private readonly IAccountDB m_accountDB;
    private readonly ILogger<LoginController> m_logger;

    public LoginController(ILogger<LoginController> logger, IAccountDB accountDB)
    {
        m_accountDB = accountDB;
        m_logger = logger;
    }

    ~LoginController() { }


    [HttpPost]
    public async Task<PkLoginRes> Post(PkLoginReq request)
    {
        var response = new PkLoginRes();

        var errorCode = await m_accountDB.VerifyAccount(request.Email, request.Password);
        if (errorCode != CSCommon.ErrorCode.None)
        {
            response.Result = errorCode;
            return response;
        }
        return response;
    }

 }

