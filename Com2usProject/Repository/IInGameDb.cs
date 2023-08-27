using Com2usProject.DataModel;

namespace Com2usProject.Repository;

public interface IInGameDb : IDisposable
{

    public Task<(CSCommon.ErrorCode ErrorCode, InventoryInfo[]? InventoryInfos)> LoadPlayerInventoryData(int playerId);
    public Task<(CSCommon.ErrorCode ErrorCode, MailInfo[]? MailInfos)> LoadPlayerMailData(int playerId);
    public Task<(CSCommon.ErrorCode ErrorCode, PlayerInfo[]? playerInfos)> LoadPlayerInfoData(String email);
}
