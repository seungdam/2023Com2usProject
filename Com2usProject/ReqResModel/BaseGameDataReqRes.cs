namespace Com2usProject.ReqResModel;

public class BaseGameDataReq
{
    public String AuthToken { get; set; }
    public int PlayerId { get; set; }
    public CSCommon.RequestType RequestType { get; set; }
    public CSCommon.ErrorCode ErrorCode { get; set; }
}

public class BaseGameDataRes
{
    public CSCommon.ErrorCode ErrorCode { get; set; }
}