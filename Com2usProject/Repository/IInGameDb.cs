using Com2usProject.DataModel;

namespace Com2usProject.Repository;

public interface IInGameDb : IDisposable
{


    //public Task<(CSCommon.ErrorCode errorCode , PlayerInfo playerData)> LoadPlayerMailData(String PlayerId);
    public Task<(CSCommon.ErrorCode errorCode, InventoryInfo[] InventoryDatas)> LoadPlayerInventoryData(String playerId);

    public Task<(CSCommon.ErrorCode errorCode, PlayerInfo[] playerDatas)> LoadPlayerInfoData(String email);
}
