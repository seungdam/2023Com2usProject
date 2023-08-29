using Com2usProject.DataModel;

namespace Com2usProject.ServiceInterface;

public interface IPlayerInventoryData
{
    
    public Task<(CSCommon.ErrorCode ErrorCode, InventoryInfo[]? InventoryInfos)> LoadInventory(int PlayerId,int InventoryPage);


    public Task<CSCommon.ErrorCode> AddItemToInventory(int PlayerId,int GetItemCode, int ItemCount);

    public Task<CSCommon.ErrorCode> UseOrDropItemFromInventory(int PlayerId, int InventoryIndex, int ItemCount);

}
