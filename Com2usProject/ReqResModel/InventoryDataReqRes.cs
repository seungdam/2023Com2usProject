using Com2usProject.DataModel;
using System.Text.Json.Nodes;

namespace Com2usProject.ReqResModel;

public class LoadPlayerInventoryDataReq : BaseGameDataReq
{
    public int InventoryPage { get; set; }
}



public class LoadPlayerInventoryDataRes : BaseGameDataRes
{
    public String InventoryInfos { get; set; } 
}


public class UpdateInventoryDataRes : BaseGameDataRes
{
    public String InventoryInfo { get; set; }
}