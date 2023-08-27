using Com2usProject.DataModel;
using System.Text.Json.Nodes;

namespace Com2usProject.ReqResModel;


public class LoadPlayerDataReq
{
    public String AuthToken { get; set; }
    public int    PlayerId { get; set; }
}

public class LoadPlayerInventoryDataRes
{
    InventoryInfo InventoryInfo;
}

public class LoadPlayerMailDataRes
{
    InventoryInfo InventoryInfo;
}