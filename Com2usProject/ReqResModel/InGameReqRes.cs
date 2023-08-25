using Com2usProject.DataModel;
using System.Text.Json.Nodes;

namespace Com2usProject.ReqResModel;


public class LoadPlayerDataReq
{
    public String AuthToken;
    public int    PlayerId;
}

public class LoadPlayerInventoryDataRes
{
    InventoryInfo InventoryInfo;
}

public class LoadPlayerMailDataRes
{
    InventoryInfo InventoryInfo;
}