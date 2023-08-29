using Com2usProject.DataModel;
using Com2usProject.AccountSecurity;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Data;
using ZLogger;
using CloudStructures.Structures;
using Newtonsoft.Json;
using Com2usProject.Service;

namespace Com2usProject.Repository;

public class InGameDb : IInGameDb
{
    readonly ILogger<InGameDb> _logger;
    readonly IOptions<DbConnectionStrings> _dbConfig;

    Compiler _mySqlComplier;
    QueryFactory _mySqlQueryFactory;
    IDbConnection _mySqlDbConnection;


    public InGameDb(ILogger<InGameDb> logger, IOptions<DbConnectionStrings> dbconfig)
    {
        _logger = logger;
        _dbConfig = dbconfig;
        _mySqlDbConnection = new MySqlConnection(dbconfig.Value.MySqlGameDb);
        _mySqlDbConnection.Open();
        _mySqlComplier = new MySqlCompiler();
        _mySqlQueryFactory = new QueryFactory(_mySqlDbConnection, _mySqlComplier);
    }



    //public async Task<(CSCommon.ErrorCode ErrorCode, InventoryInfo[]? InventoryInfos)> LoadPlayerInventoryData(int ReqPlayerId,int InventoryPage)
    //{

    //    try
    //    {
           
    //        // 최대 9칸 까지만 가져온다.
    //        var InventoryDatas = await _mySqlQueryFactory.Query("inventory")
    //                            .Select("InventoryIndex", "ItemCode", "Count")
    //                            .Where("PlayerId", ReqPlayerId)
    //                            .OrderBy("InventoryIndex").ForPage(InventoryPage,perPage:9)
    //                            .GetAsync<InventoryInfo>();
    //        if (InventoryDatas is not null)
    //        {
    //            return (ErrorCode: CSCommon.ErrorCode.ErrorNone, InventoryDatas.ToArray());
    //        }
    //        else
    //        {
    //            return (ErrorCode: CSCommon.ErrorCode.ErrorNone, InventoryInfos: null);
    //        }
    //    }
    //    catch
    //    {
    //        _logger.ZLogError("Error Occur At LoadPlayerInventoryData");

    //        return (ErrorCode: CSCommon.ErrorCode.LoadPlayerDataErrorException, InventoryInfos: null);
    //    }
    //}
  
    //public async Task<(CSCommon.ErrorCode ErrorCode, MailInfo[]? MailInfos)> LoadPlayerMailBoxData(int ReqPlayerId, int MailBoxPage)
    //{

    //    try
    //    {
      
    //        var mailInfos = await _mySqlQueryFactory.Query("mailbox")
    //                               .Select("MailIndex", "ItemCode", "ItemCount", "RecievedTick", "ExpirationTick")
    //                               .Where("PlayerId", ReqPlayerId)
    //                               .OrderBy("MailIndex")
    //                               .ForPage(MailBoxPage,perPage:5)
    //                               .GetAsync<MailInfo>();

    //        if (mailInfos is not null)
    //        {
    //            return (ErrorCode: CSCommon.ErrorCode.ErrorNone, mailInfos.ToArray());
    //        }
    //        else
    //        {
    //            return (ErrorCode: CSCommon.ErrorCode.ErrorNone, MailInfos: null);
    //        }
    //    }
    //    catch
    //    {
    //        _logger.ZLogError("Error Occur At LoadPlayerMailData");

    //        return (ErrorCode: CSCommon.ErrorCode.LoadPlayerDataErrorException, MailInfos: null);
    //    }
    //}

    //public async Task<(CSCommon.ErrorCode ErrorCode, PlayableCharacterInfo[]? playerInfos)> LoadPlayerInfoData(String email)
    //{

    //    try
    //    {
    //        var playerableCharacterDatas = await _mySqlQueryFactory.Query("playerinfo")
    //                                            .Select("PlayerId", "Class", "Level")
    //                                            .Where("PlayerEmail", email)
    //                                            .Limit(3)
    //                                            .GetAsync<PlayableCharacterInfo>();

    //        if (playerableCharacterDatas is not null)
    //        {
    //            return (ErrorCode: CSCommon.ErrorCode.ErrorNone, playerInfos: playerableCharacterDatas.ToArray());
    //        }
    //        else
    //        {
    //            return (ErrorCode: CSCommon.ErrorCode.ErrorNone, playerInfos: new[] { new PlayableCharacterInfo() { PlayerId = -1, Level = 0, Class = "None" } });
    //        }
    //    }
    //    catch
    //    {
    //        _logger.ZLogError($"[InGameRepository.LoadPlayerInfoData] ErrorCode: {CSCommon.ErrorCode.LoadPlayerDataErrorException}");
    //        return (ErrorCode: CSCommon.ErrorCode.LoadPlayerDataErrorException, playerInfos: null);
    //    }
    //}


    public QueryFactory GetQueryFactory() { return _mySqlQueryFactory; }
    public void Dispose()
    {
        _mySqlDbConnection.Close();
    }
}
