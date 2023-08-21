using Com2usProject.DataModel;

namespace Com2usProject.Service;

public interface ICharacterDb
{

    public Task<Tuple<CSCommon.ErrorCode,String>> GetCharacterData(String id);
   // public Task<CSCommon.ErrorCode> UpdateCharacterData(String id);

}
