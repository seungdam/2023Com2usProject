using Com2usProject.DataModel;
using Com2usProject.Repository;
using Com2usProject.ReqResModel;
using Com2usProject.ServiceInterface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZLogger;
using Newtonsoft.Json;

namespace Com2usProject.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MailBoxController : ControllerBase
{
    readonly IPlayerMailBoxData _mailBoxHandler;
    readonly IRedisDb _redisDb;
    readonly ILogger<MailBoxController> _logger;
    public MailBoxController(ILogger<MailBoxController> logger, IPlayerMailBoxData mHandler, IRedisDb redisDb)
    {
        _logger = logger;
        _mailBoxHandler = mHandler;
        _redisDb = redisDb;
    }

    [HttpPost("/LoadPlayerMailData")]
    public async Task<LoadPlayerMailBoxDataRes> LoadMailData(LoadPlayerMailBoxDataReq request)
    {

        var resultMailData = await _mailBoxHandler.LoadMail(request.PlayerId, request.MailBoxPage);
        LoadPlayerMailBoxDataRes response = new LoadPlayerMailBoxDataRes();
        _redisDb.FinishPlayerRequest(request.PlayerId);
        if (resultMailData.ErrorCode == CSCommon.ErrorCode.ErrorNone)
        {

            if (resultMailData.MailInfos is not null)
            {
                response.MailBoxInfos = "";
                List<MailInfo> ml = new List<MailInfo>();
                foreach (var i in resultMailData.MailInfos) ml.Add(i);
                response.MailBoxInfos = JsonConvert.SerializeObject(ml, Formatting.Indented);
                response.ErrorCode = CSCommon.ErrorCode.ErrorNone;
            }
            response.ErrorCode = CSCommon.ErrorCode.ErrorNone;
        }
        else
        {
            response.ErrorCode = CSCommon.ErrorCode.LoadPlayerDataErrorException;
            _logger.ZLogError($"[MailController.LoadMailData] ErrorCode {resultMailData.ErrorCode} ");
        }

        return response;
    }

    [HttpPost("/RecvMailItem")]
    public async Task<RecvMailBoxDataRes> RecvMailData(RecvMailBoxDataReq request)
    {
        var resultErrorCode = await _mailBoxHandler.RecieveMail(request.PlayerId, request.RecvMailIndex);
        var response = new RecvMailBoxDataRes();
        
        response.ErrorCode = resultErrorCode;
        
        if (resultErrorCode != CSCommon.ErrorCode.ErrorNone)
        {
            response.DelMailIndex = -1;
        }
        else
        {
            response.DelMailIndex = request.RecvMailIndex;
            _logger.ZLogError($"[MailController.RecvMailData] ErrorCode {resultErrorCode} ");
        }  

        return response;
    }
}
