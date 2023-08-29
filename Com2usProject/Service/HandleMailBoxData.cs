using Com2usProject.DataModel;
using Com2usProject.Repository;
using SqlKata.Execution;
using ZLogger;


namespace Com2usProject.ServiceInterface;

public class HandleMailBoxData : IPlayerMailBoxData
{
    readonly IInGameDb _gameDb;
    readonly IPlayerInventoryData _inventoryHandler;
    readonly ILogger<HandleMailBoxData> _logger;

    public HandleMailBoxData(IInGameDb gameDb, ILogger<HandleMailBoxData> logger, IPlayerInventoryData piHandler)
    {
        _gameDb = gameDb;
        _logger = logger;
        _inventoryHandler = piHandler;
    }

  
    public async Task<CSCommon.ErrorCode> DelMail(int PlayerId,int DelMailIndex)
    {

        try
        {
            var result = await _gameDb.GetQueryFactory().Query("mailbox").Where("PlayerId", PlayerId).Where("MailIndex", DelMailIndex).DeleteAsync();

            if (result != 1) return CSCommon.ErrorCode.MailBoxDeleteError;

            else return CSCommon.ErrorCode.ErrorNone;
        }
        catch 
        {
            return CSCommon.ErrorCode.MailBoxDeleteError;
        }
    }
    public async Task<CSCommon.ErrorCode> RecieveMail(int PlayerId, int RecvMailIndex)
    {
        try
        {
            var mailInfo = await _gameDb.GetQueryFactory().Query("mailbox")
                                       .Select("MailIndex", "ItemCode", "ItemCount", "RecievedTime", "ExpirationTime")
                                       .Where("PlayerId", PlayerId)
                                       .Where("Index", RecvMailIndex)
                                       .FirstAsync<MailInfo>();

            var handleResult = await _inventoryHandler.AddItemToInventory(PlayerId, mailInfo.ItemCode, mailInfo.ItemCount);

            handleResult = await DelMail(PlayerId, RecvMailIndex);

            return handleResult;
        }
        catch
        {

            return CSCommon.ErrorCode.MailBoxReciveMailError;
        }

    }

    public async Task<(CSCommon.ErrorCode ErrorCode, MailInfo[]? MailInfos)> LoadMail(int ReqPlayerId, int MailBoxPage)
    {

        try
        {

            var mailInfos = await _gameDb.GetQueryFactory().Query("mailbox")
                                   .Select("MailIndex", "ItemCode", "ItemCount", "RecievedTime", "ExpirationTime")
                                   .Where("PlayerId", ReqPlayerId)
                                   .OrderBy("MailIndex")
                                   .ForPage(MailBoxPage, perPage: 5)
                                   .GetAsync<MailInfo>();


            if (mailInfos is not null)
            {
                return (ErrorCode: CSCommon.ErrorCode.ErrorNone, mailInfos.ToArray());
            }
            else
            {
                return (ErrorCode: CSCommon.ErrorCode.ErrorNone, MailInfos: null);
            }
        }
        catch
        {
            _logger.ZLogError("Error Occur At LoadPlayerMailData");

            return (ErrorCode: CSCommon.ErrorCode.LoadPlayerDataErrorException, MailInfos: null);
        }
    }
}