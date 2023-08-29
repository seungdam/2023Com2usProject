using Com2usProject.DataModel;
using SqlKata.Execution;

namespace Com2usProject.Repository;

public interface IInGameDb : IDisposable
{

    //public Task<(CSCommon.ErrorCode ErrorCode, InventoryInfo[]? InventoryInfos)> LoadPlayerInventoryData(int PlayerId, int InventoryPage);
    //public Task<(CSCommon.ErrorCode ErrorCode, MailInfo[]? MailInfos)> LoadPlayerMailBoxData(int PlayerId, int MailPage);
    //public Task<(CSCommon.ErrorCode ErrorCode, PlayableCharacterInfo[]? playerInfos)> LoadPlayerInfoData(String Email);

    public QueryFactory GetQueryFactory();
}
