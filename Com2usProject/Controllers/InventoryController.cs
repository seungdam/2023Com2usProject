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
public class InventoryController : ControllerBase
{
    readonly IPlayerInventoryData _inventoryHandler;
    readonly ILogger<InventoryController> _logger;
    readonly IRedisDb _redisDb;

    public InventoryController(ILogger<InventoryController> logger, IPlayerInventoryData iHandler, IRedisDb redisDb)
    {
        _logger = logger;
        _inventoryHandler = iHandler;
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
}
