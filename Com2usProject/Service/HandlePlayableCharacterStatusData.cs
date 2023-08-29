
using Com2usProject.DataModel;
using Com2usProject.Repository;
using Com2usProject.ServiceInterface;
using SqlKata.Execution;
using ZLogger;

namespace Com2usProject.Service;



public class HandlePlayableCharacterStatusData : IPlayableCharacterStatusData 
{

    readonly IInGameDb _gameDb;
    readonly ILogger<HandlePlayableCharacterStatusData> _logger;
    HandlePlayableCharacterStatusData(ILogger<HandlePlayableCharacterStatusData> logger, IInGameDb gameDb)
    {
        _gameDb = gameDb;
        _logger = logger;
    }

    public async Task<(CSCommon.ErrorCode ErrorCode, PlayableCharacterStatusInfo[]? playerInfos)> LoadPlayerCharacterStatusData(String email)
    {

        try
        {
            var playerableCharacterDatas = await _gameDb.GetQueryFactory().Query("playerinfo")
                                                .Select("PlayerId", "Class", "Level")
                                                .Where("PlayerEmail", email)
                                                .Limit(3)
                                                .GetAsync<PlayableCharacterStatusInfo>();

            if (playerableCharacterDatas is not null)
            {
                return (ErrorCode: CSCommon.ErrorCode.ErrorNone, playerInfos: playerableCharacterDatas.ToArray());
            }
            else
            {
                return (ErrorCode: CSCommon.ErrorCode.ErrorNone, playerInfos: new[] { new PlayableCharacterStatusInfo() { PlayerId = -1, Level = 0, Class = "None" } });
            }
        }
        catch
        {
            _logger.ZLogError($"[InGameRepository.LoadPlayerInfoData] ErrorCode: {CSCommon.ErrorCode.LoadPlayerDataErrorException}");
            return (ErrorCode: CSCommon.ErrorCode.LoadPlayerDataErrorException, playerInfos: null);
        }
    }
}
