using Com2usWebProject.ModelResReq;
using Com2usWebProject.Services;
using Microsoft.AspNetCore.Mvc;
using ZLogger;

namespace Com2usWebProject.Controllers;
    [ApiController]
    [Route("Controller")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountDB m_accountDB;
        private readonly ILogger<AccountController> m_logger;


        public AccountController(ILogger<AccountController> logger, IAccountDB accountDB)
        {
            m_logger = logger;
            m_accountDB = accountDB;
        }

        // Post 요청 보내기
        [HttpPost]
        public async Task<PkCreateAccountRes> Post(PkCreateAccountReq request)
        {
            var response = new PkCreateAccountRes(); 

            var errorCode = await m_accountDB.CreateAccountAsync(request.Email, request.Password);
            if (errorCode != CSCommon.ErrorCode.None)
            {
            response.Result = errorCode;
            return response;
            }
        return response;
        }
    }

