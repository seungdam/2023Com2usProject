using Com2usProject.DataModel;

namespace Com2usProject.ServiceInterface;


public interface IPlayableCharacterStatusData
{

   
    public Task<(CSCommon.ErrorCode ErrorCode, PlayableCharacterStatusInfo[]? playerInfos)> LoadPlayerCharacterStatusData(string Email);
}