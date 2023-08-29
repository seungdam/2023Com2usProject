using Com2usProject.DataModel;
using Com2usProject.ReqResModel;
using Com2usProject.Repository;
using Microsoft.AspNetCore.Mvc;

using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text.Json.Nodes;
using ZLogger;
using System;
using Newtonsoft.Json;
using Com2usProject.ServiceInterface;

namespace Com2usProject.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LoadPlayerDataController : ControllerBase
{
    readonly IPlayableCharacterStatusData _characterStatusHandler;
    readonly IPlayerMailBoxData _mailBoxHandler;
    readonly IPlayerInventoryData _inventoryHandler;


    readonly IRedisDb  _redisDb;
    readonly ILogger<LoadPlayerDataController> _logger;

    public LoadPlayerDataController(ILogger<LoadPlayerDataController> logger, IPlayableCharacterStatusData csHandler,IPlayerInventoryData piHandler,IPlayerMailBoxData pmHandler,
        IRedisDb redisDb)
    {
        _logger = logger;
        _characterStatusHandler = csHandler;
        _inventoryHandler = piHandler;
        _mailBoxHandler = pmHandler;
        _redisDb = redisDb;
    }



    [HttpPost("/LoadPlayerInventoryData")]
    public async Task<LoadPlayerInventoryDataRes> LoadSelectedPlayerInventoryData(LoadPlayerInventoryDataReq request)
    {

        var resultInventoryData = await _inventoryHandler.LoadInventory(request.PlayerId, request.InventoryPage);
        _redisDb.FinishPlayerRequest(request.PlayerId);
        LoadPlayerInventoryDataRes response = new LoadPlayerInventoryDataRes();

        if (resultInventoryData.ErrorCode == CSCommon.ErrorCode.ErrorNone)
        {

            if (resultInventoryData.InventoryInfos.Length > 0)
            {
                response.InventoryInfos = "";
                List<InventoryInfo> il = new List<InventoryInfo>();
                foreach (var i in resultInventoryData.InventoryInfos) il.Add(i);
                response.InventoryInfos = JsonConvert.SerializeObject(il, Formatting.Indented);
                response.ErrorCode = CSCommon.ErrorCode.ErrorNone;
            }
        }
        else
        {
            response.ErrorCode = CSCommon.ErrorCode.LoadPlayerDataErrorException;
            _logger.ZLogError($"[LoadPlayerController.LoadSelectedPlayerData] ErrorCode  {resultInventoryData.ErrorCode}");
        }
        
        return response;
    }

    [HttpPost("/LoadPlayerMailData")]
    public async Task<LoadPlayerMailBoxDataRes> LoadSelectedPlayerMailData(LoadPlayerMailBoxDataReq request)
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
            _logger.ZLogError($"[LoadPlayerController.LoadSelectedPlayerData] ErrorCode {resultMailData.ErrorCode} ");
        }
        
        return response;
    }

}
