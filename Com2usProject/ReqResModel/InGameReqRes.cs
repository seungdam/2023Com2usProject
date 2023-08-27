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
    public String InventoryInfos { get; set; } 
    public CSCommon.ErrorCode ErrorCode { get; set; }
}



public class LoadPlayerMailDataRes
{
    public String MailInfos { get; set; }
    public CSCommon.ErrorCode ErrorCode { get; set; }
}