using Com2usProject.DataModel;
using Com2usProject.Repository;
using Com2usProject.ReqResModel;
using Com2usProject.ServiceInterface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZLogger;
using Newtonsoft.Json;

namespace Com2usProject.Controllers;

[ApiController]

[Route("api/[controller]")]
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
        var resultVar = await _mailBoxHandler.RecvMail(request.PlayerId, request.RecvMailIndex);
        var response = new RecvMailBoxDataRes();
        
        response.ErrorCode = resultVar.ErrorCode;
        
        if (resultVar.ErrorCode != CSCommon.ErrorCode.ErrorNone)
        {
            response.DelMailIndex = -1;
            _logger.ZLogError($"[MailController.RecvMailData] ErrorCode {resultVar.ErrorCode} ");
        }
        else
        {
            response.DelMailIndex = request.RecvMailIndex;
            response.AfterInventoryInfo = JsonConvert.SerializeObject(resultVar.AfterInventoryInfo, Formatting.Indented);
            
        }  

        return response;
    }
}
