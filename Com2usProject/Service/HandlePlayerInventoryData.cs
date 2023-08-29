using Com2usProject.DataModel;
using Com2usProject.Repository;
using Microsoft.VisualBasic;
using SqlKata.Execution;
using System.Reflection;

using Com2usProject.ServiceInterface;
using ZLogger;

namespace Com2usProject.Service;

public class HandlePlayerInventoryData: IPlayerInventoryData
{
    readonly IInGameDb _gameDb;
    readonly ILogger<HandlePlayableCharacterStatusData> _logger;

    HandlePlayerInventoryData(IInGameDb gameDb, ILogger<HandlePlayableCharacterStatusData> logger)
    {
        _gameDb = gameDb;
        _logger = logger;
    }
    public async void InsertNewItem(int PlayerId,int NewItemCode, int NewItemCount)
    {

        try
        {
            var nextIndex = await _gameDb.GetQueryFactory().Query("inventory").MaxAsync<int>(column: "InventoryIndex");
            var count = await _gameDb.GetQueryFactory().Query("inventory").InsertAsync
            (
                new
                {

                    PlayerId = PlayerId,
                    InventoryIndex = nextIndex,
                    ItemCode = NewItemCode,
                    Count = NewItemCount
                }
            );

            if (count != 1) _logger.ZLogDebug("Insert Item Fail");
        }
       catch
        {
            _logger.ZLogError("Insert Exception Occur");
        }
      
    }

    public async void IncreaseItem(int PlayerId, int InventoryIndex, int IncreaseValue)
    {

        try
        {
            var count = await _gameDb.GetQueryFactory().Query("inventory")
                .Where("PlayerId", PlayerId)
                .Where("InventoryIndex", InventoryIndex)
                .IncrementAsync("Count", IncreaseValue);


            if (count != 1) _logger.ZLogDebug("Decrease Item Fail");
        }
        catch
        {
            _logger.ZLogError("DeCrease Exception Occur");
        }
        
    }

    public async void DecreaseItem(int PlayerId, int InventoryIndex, int DecreaseValue)
    {

        try
        {
            var count = await _gameDb.GetQueryFactory().Query("inventory")
               .Where("PlayerId", PlayerId)
               .Where("InventoryIndex", InventoryIndex)
               .DecrementAsync("Count", DecreaseValue);


            if (count != 1) _logger.ZLogDebug("Decrease Item Fail");
        }
        catch
        {
            _logger.ZLogError("DeCrease Exception Occur");
        }
       
    }
    
    public async void DelItem(int PlayerId, int InventoryIndex, int ItemCode)
    {

        try
        {
            var count = await _gameDb.GetQueryFactory().Query("inventory")
             .Where("PlayerId", PlayerId)
             .Where("InventoryIndex", InventoryIndex)
             .DeleteAsync();

            if (count != 1) _logger.ZLogDebug("Del Item Fail");
        }
        catch 
        {
            _logger.ZLogError("DelItem Exception Occur");

        }

    }

    // ======================= InterFace Method ===================================

    public async Task<(CSCommon.ErrorCode ErrorCode, InventoryInfo[]? InventoryInfos)> LoadInventory(int ReqPlayerId, int InventoryPage)
    {

        try
        {

            // 최대 9칸 까지만 가져온다.
            var InventoryDatas = await _gameDb.GetQueryFactory().Query("inventory")
                                .Select("InventoryIndex", "ItemCode", "Count")
                                .Where("PlayerId", ReqPlayerId)
                                .OrderBy("InventoryIndex").ForPage(InventoryPage, perPage: 9)
                                .GetAsync<InventoryInfo>();

            if (InventoryDatas is not null)
            {
                return (ErrorCode: CSCommon.ErrorCode.ErrorNone, InventoryDatas.ToArray());
            }
            else
            {
                return (ErrorCode: CSCommon.ErrorCode.ErrorNone, InventoryInfos: null);
            }
        }
        catch
        {
            _logger.ZLogError("Error Occur At LoadPlayerInventoryData");

            return (ErrorCode: CSCommon.ErrorCode.LoadPlayerDataErrorException, InventoryInfos: null);
        }

    }


    public async Task<CSCommon.ErrorCode> AddItemToInventory(int PlayerId, int GetItemCode, int ItemCount)
    {

        try
        {
            var sameItemInventoryIndex = await _gameDb.GetQueryFactory().Query("inventory")
                                        .Select("InventoryIndex")
                                        .Where("PlayerId", PlayerId)
                                        .Where("ItemCode", GetItemCode)
                                        .FirstAsync();

            if(sameItemInventoryIndex is not null)
            {
              IncreaseItem(PlayerId, sameItemInventoryIndex, ItemCount);
            }
            else
            {
              InsertNewItem(PlayerId, GetItemCode, ItemCount);
            }


        }
        catch 
        {
            _logger.ZLogError("[HandleInventoryData.AddItemToInventory] SameItemInventoryIndex Exception");
            return CSCommon.ErrorCode.InventoryErrorUseOrDropItemException;
        }

        return CSCommon.ErrorCode.ErrorNone;
    }


    public async Task<CSCommon.ErrorCode> UseOrDropItemFromInventory(int PlayerId, int InventoryIndex, int DropItemCount)
    {

        try
        {
            var curCount = await _gameDb.GetQueryFactory().Query("inventory").Select("ItemCount").Where("PlayerId", PlayerId).Where("InventoryIndex", InventoryIndex).FirstAsync();

            if (curCount - DropItemCount <= 0)
            {
                DecreaseItem(PlayerId, InventoryIndex, DropItemCount);
            }
            else
            {
                DelItem(PlayerId, InventoryIndex, DropItemCount);
            }


        }
        catch
        {
            _logger.ZLogError("[HandleInventoryData.AddItemToInventory] CurCount Exception");

            return CSCommon.ErrorCode.InventoryErrorUseOrDropItemException;

        }
        return CSCommon.ErrorCode.ErrorNone;
    }
}
