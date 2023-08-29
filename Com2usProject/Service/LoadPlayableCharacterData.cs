
using Com2usProject.DataModel;
using Com2usProject.Repository;
using SqlKata.Execution;
using ZLogger;

namespace Com2usProject.Service;



public class LoadPlayableCharacter : ILoadPlayerbleCharacterData 
{

    readonly IInGameDb _gameDb;
    readonly ILogger<LoadPlayableCharacter> _logger;
    LoadPlayableCharacter(ILogger<LoadPlayableCharacter> logger, IInGameDb gameDb)
    {
        _gameDb = gameDb;
        _logger = logger;
    }

    public async Task<(CSCommon.ErrorCode ErrorCode, InventoryInfo[]? InventoryInfos)> LoadPlayerInventoryData(int ReqPlayerId, int InventoryPage)
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

    public async Task<(CSCommon.ErrorCode ErrorCode, MailInfo[]? MailInfos)> LoadPlayerMailBoxData(int ReqPlayerId, int MailBoxPage)
    {

        try
        {

            var mailInfos = await _gameDb.GetQueryFactory().Query("mailbox")
                                   .Select("MailIndex", "ItemCode", "ItemCount", "RecievedTick", "ExpirationTick")
                                   .Where("PlayerId", ReqPlayerId)
                                   .OrderBy("MailIndex")
                                   .ForPage(MailBoxPage, perPage: 5)
                                   .GetAsync<MailInfo>();

            if (mailInfos is not null)
            {
                return (ErrorCode: CSCommon.ErrorCode.ErrorNone, mailInfos.ToArray());
            }
            else
            {
                return (ErrorCode: CSCommon.ErrorCode.ErrorNone, MailInfos: null);
            }
        }
        catch
        {
            _logger.ZLogError("Error Occur At LoadPlayerMailData");

            return (ErrorCode: CSCommon.ErrorCode.LoadPlayerDataErrorException, MailInfos: null);
        }
    }

    public async Task<(CSCommon.ErrorCode ErrorCode, PlayableCharacterInfo[]? playerInfos)> LoadPlayerInfoData(String email)
    {

        try
        {
            var playerableCharacterDatas = await _gameDb.GetQueryFactory().Query("playerinfo")
                                                .Select("PlayerId", "Class", "Level")
                                                .Where("PlayerEmail", email)
                                                .Limit(3)
                                                .GetAsync<PlayableCharacterInfo>();

            if (playerableCharacterDatas is not null)
            {
                return (ErrorCode: CSCommon.ErrorCode.ErrorNone, playerInfos: playerableCharacterDatas.ToArray());
            }
            else
            {
                return (ErrorCode: CSCommon.ErrorCode.ErrorNone, playerInfos: new[] { new PlayableCharacterInfo() { PlayerId = -1, Level = 0, Class = "None" } });
            }
        }
        catch
        {
            _logger.ZLogError($"[InGameRepository.LoadPlayerInfoData] ErrorCode: {CSCommon.ErrorCode.LoadPlayerDataErrorException}");
            return (ErrorCode: CSCommon.ErrorCode.LoadPlayerDataErrorException, playerInfos: null);
        }
    }
}
