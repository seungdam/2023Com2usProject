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

namespace Com2usProject.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LoadPlayerDataController : ControllerBase
{
    readonly IInGameDb _gameDb;
    readonly IRedisDb  _redisDb;
    readonly ILogger<LoadPlayerDataController> _logger;

    public LoadPlayerDataController(ILogger<LoadPlayerDataController> logger, IInGameDb inGameDb, IRedisDb redisDb)
    {
        _logger = logger;
        _gameDb = inGameDb;
        _redisDb = redisDb;
    }


   

    [HttpPost("/LoadPlayerInventoryData")]
    public async Task<LoadPlayerInventoryDataRes> LoadSelectedPlayerInventoryData(LoadPlayerDataReq request)
    {

        var resultInventoryData = await _gameDb.LoadPlayerInventoryData(request.PlayerId);
        
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
    public async Task<LoadPlayerMailDataRes> LoadSelectedPlayerMailData(LoadPlayerDataReq request)
    {

        var resultMailData = await _gameDb.LoadPlayerMailData(request.PlayerId);
        LoadPlayerMailDataRes response = new LoadPlayerMailDataRes();

        if (resultMailData.ErrorCode == CSCommon.ErrorCode.ErrorNone)
        {

            if (resultMailData.MailInfos.Length > 0)
            {
                response.MailInfos = "";
                List<MailInfo> ml = new List<MailInfo>();
                foreach (var i in resultMailData.MailInfos) ml.Add(i);
                response.MailInfos = JsonConvert.SerializeObject(ml, Formatting.Indented);
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
