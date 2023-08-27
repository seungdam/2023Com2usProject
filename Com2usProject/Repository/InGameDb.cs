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

namespace Com2usProject.Repository;

public class InGameDb : IInGameDb
{
    readonly ILogger<InGameDb> _logger;
    readonly IRedisDb _redisDb;
    readonly IOptions<DbConnectionStrings> _dbConfig;

    Compiler _mySqlComplier;
    QueryFactory _mySqlQueryFactory;
    IDbConnection _mySqlDbConnection;


    public InGameDb(ILogger<InGameDb> logger, IOptions<DbConnectionStrings> dbconfig, IRedisDb redisDb)
    {
        _logger = logger;
        _dbConfig = dbconfig;
        _redisDb = redisDb;
        _mySqlDbConnection = new MySqlConnection(dbconfig.Value.MySqlGameDb);
        _mySqlDbConnection.Open();


        _mySqlComplier = new MySqlCompiler();
        _mySqlQueryFactory = new QueryFactory(_mySqlDbConnection, _mySqlComplier);
    }

    public async Task<(CSCommon.ErrorCode ErrorCode, InventoryInfo[]? InventoryInfos)> LoadPlayerInventoryData(int ReqPlayerId)
    {

        try
        {
           
            // 최대 9칸 까지만 가져온다.
            var InventoryDatas = await _mySqlQueryFactory.Query("inventory").Select("InventoryIndex", "ItemCode", "Count").Where("PlayerId", ReqPlayerId).Limit(9).GetAsync<InventoryInfo>();
            LoadAllInventoryInfoData(ReqPlayerId);
            if (InventoryDatas is not null)
            {
                return (ErrorCode: CSCommon.ErrorCode.ErrorNone, InventoryDatas.ToArray());
            }
            else
            {
                return (ErrorCode: CSCommon.ErrorCode.ErrorNone, InventoryInfos: null);
            }
        }
        catch
        {
            _logger.ZLogError("Error Occur At LoadPlayerInventoryData");

            return (ErrorCode: CSCommon.ErrorCode.LoadPlayerDataErrorException, MailInfos: null);
        }
    }
  
    public async Task<(CSCommon.ErrorCode ErrorCode, MailInfo[]? MailInfos)> LoadPlayerMailData(int ReqPlayerId)
    {

        try
        {
           

            var mailInfos = await _mySqlQueryFactory.Query("mailbox").Select("MailIndex", "ItemCode", "ItemCount", "RecievedTick", "ExpirationTick").Where("PlayerId", ReqPlayerId).Limit(5).GetAsync<MailInfo>();


            LoadAllMailInfoData(ReqPlayerId);
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

    public async Task<(CSCommon.ErrorCode ErrorCode, PlayerInfo[]? playerInfos)> LoadPlayerInfoData(String email)
    {

        try
        {
            var bIsPlayerInfoExist = await _mySqlQueryFactory.Query("playerinfo").Where("PlayerEmail", email).ExistsAsync();

            if (bIsPlayerInfoExist)
            {
                var LoadedplayerDatas = await _mySqlQueryFactory.Query("playerinfo").Select("PlayerId", "Class", "Level").Where("PlayerEmail", email).GetAsync<PlayerInfo>();
                return (ErrorCode: CSCommon.ErrorCode.ErrorNone, playerInfos: LoadedplayerDatas.ToArray());
            }
            else
            {
                return (ErrorCode: CSCommon.ErrorCode.ErrorNone, playerInfos: new[] { new PlayerInfo() { PlayerId = -1, Level = 0, Class = "None" } });
            }
        }
        catch
        {
            _logger.ZLogError($"[InGameRepository.LoadPlayerInfoData] ErrorCode: {CSCommon.ErrorCode.LoadPlayerDataErrorException}");
            return (ErrorCode: CSCommon.ErrorCode.ErrorNone, playerInfos: new[] { new PlayerInfo() { PlayerId = -1, Level = 0, Class = "None" } });
        }
    }

    public async void  LoadAllInventoryInfoData(int ReqPlayerId)
    {
       await Task.Run(
            async () =>
            {
                var inventoryInfos = await _mySqlQueryFactory.Query("inventory").Select("InventoryIndex", "ItemCode", "Count").Where("PlayerId", ReqPlayerId).GetAsync<InventoryInfo>();
                if(inventoryInfos is not null)
                {
                    var redisQuery = new RedisSortedSet<string>(_redisDb.GetConnection(), "PlayerInventory" + ReqPlayerId.ToString(),null);

                    foreach (var i in inventoryInfos) if (i is not null)
                        {
                            string jsonString = JsonConvert.SerializeObject(i);
                            await redisQuery.AddAsync(jsonString, i.InventoryIndex, null, StackExchange.Redis.When.NotExists);

                        }
                }
                _logger.ZLogInformation("[InGameDb.LoadAllInventoryInfoData]Load All Inventory Information");
            }
        );

    }

    public async void LoadAllMailInfoData(int ReqPlayerId)
    {
        await Task.Run(
            async () =>
            {
                var mailInfos = await _mySqlQueryFactory.Query("mailbox").Select("MailIndex", "ItemCode", "ItemCount").Where("PlayerId", ReqPlayerId).GetAsync<MailInfo>();
                if (mailInfos is not null)
                {
                    var redisQuery = new RedisSortedSet<string>(_redisDb.GetConnection(), "PlayerMail" + ReqPlayerId.ToString(), null);

                    foreach (var i in mailInfos) if (i is not null) await redisQuery.AddAsync(i.ToString(),i.index,null,StackExchange.Redis.When.NotExists);

                }
                _logger.ZLogInformation("[InGameDb.LoadAllMailInfoData]Load All Inventory Information");
            }
        );

    }


 

    public void Dispose()
    {
        _mySqlDbConnection.Close();
    }
}
