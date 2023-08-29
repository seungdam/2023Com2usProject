using Com2usProject.DataModel;
using System.Text.Json.Nodes;

namespace Com2usProject.ReqResModel;

public class LoadPlayableCharacterDataReq : BaseGameDataReq
{
    public int  PlayerId { get; set; }
}

public class LoadPlayerInventoryDataRes : BaseGameDataRes
{
    public String InventoryInfos { get; set; } 
}

public class LoadPlayerMailBoxDataRes : BaseGameDataRes
{
    public String MailBoxInfos { get; set; }
}