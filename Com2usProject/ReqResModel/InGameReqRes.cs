using Com2usProject.DataModel;
using System.Text.Json.Nodes;

namespace Com2usProject.ReqResModel;

public class LoadCharacterDataReq : BaseReqRes
{
    public String Id { get; set; }
}


// 모든 데이터는 JSon형태로 저장되도록 한다.
public class LoadCharacterDataRes
{
  public CharacterModel charData { get; set; }
}
