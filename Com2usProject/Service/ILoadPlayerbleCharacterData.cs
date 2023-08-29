using Com2usProject.DataModel;
namespace Com2usProject.Service;


public interface ILoadPlayerbleCharacterData
{

    public Task<(CSCommon.ErrorCode ErrorCode, InventoryInfo[]? InventoryInfos)> LoadPlayerInventoryData(int PlayerId, int InventoryPage);
    public Task<(CSCommon.ErrorCode ErrorCode, MailInfo[]? MailInfos)> LoadPlayerMailBoxData(int PlayerId, int MailPage);
    public Task<(CSCommon.ErrorCode ErrorCode, PlayableCharacterInfo[]? playerInfos)> LoadPlayerInfoData(String Email);
}