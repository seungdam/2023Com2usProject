namespace Com2usProject.ReqResModel;

public class LoadPlayerMailBoxDataReq : BaseGameDataReq
{
    public int MailBoxPage { get; set; }
}
public class LoadPlayerMailBoxDataRes : BaseGameDataRes
{
    public String MailBoxInfos { get; set; }
}


public class RecvMailBoxDataReq : BaseGameDataReq
{
    public int RecvMailIndex { get; set; }
}

public class RecvMailBoxDataRes : BaseGameDataRes
{
    public int DelMailIndex { get; set; }
    public String AfterInventoryInfo { get; set; } 
    
}