using Com2usProject.DataModel;
using Com2usProject.SecurityUtil;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Data;

namespace Com2usProject.Repository;

public class InGameRepository : IInGameDb
{
    readonly ILogger<InGameRepository> _logger;
    readonly IRedisDb _redisDb;
    readonly IOptions<DbConnectionStrings> _dbConfig;

    Compiler _mySqlComplier;
    QueryFactory _mySqlQueryFactory;
    IDbConnection _mySqlDbConnection;


    public InGameRepository(ILogger<InGameRepository> logger, IOptions<DbConnectionStrings> dbconfig)
    {
        _logger = logger;
        _dbConfig = dbconfig;

        _mySqlDbConnection = new MySqlConnection(dbconfig.Value.MySqlGameDb);
        _mySqlDbConnection.Open();


        _mySqlComplier = new MySqlCompiler();
        _mySqlQueryFactory = new QueryFactory(_mySqlDbConnection, _mySqlComplier);
    }

    public async Task<(CSCommon.ErrorCode errorCode, InventoryInfo[] InventoryDatas)> LoadPlayerInventoryData(String ReqPlayerId)
    {
      
        
        // 최대 9칸 까지만 가져온다.
        var InventoryDatas = await _mySqlQueryFactory.Query("inventoryinfo").Select("Index", "ItemCode", "Count").Where("PlayerId", ReqPlayerId).Limit(9).GetAsync<InventoryInfo>();
        if (InventoryDatas is not null)
        {
            return (errorCode : CSCommon.ErrorCode.ErrorNone, InventoryDatas.ToArray());
        }
        else
        {
            return (errorCode : CSCommon.ErrorCode.LoadCharacterErrorNoExist, InventoryDatas: null);
        }
    }
  
    public async Task<(CSCommon.ErrorCode errorCode, PlayerInfo[] playerDatas)> LoadPlayerInfoData(String email)
    {
        var bIsPlayerInfoExist = await _mySqlQueryFactory.Query("playerinfo").Where("PlayerEmail", email).ExistsAsync();
        
        if(bIsPlayerInfoExist)
        {
           var LoadedplayerDatas =  await _mySqlQueryFactory.Query("playerinfo").Select("PlayerId","Class", "Level").Where("PlayerEmail",email).GetAsync<PlayerInfo>();

            return (errorCode: CSCommon.ErrorCode.ErrorNone, playerDatas: LoadedplayerDatas.ToArray());
        }
        else
        {
            return (errorCode: CSCommon.ErrorCode.ErrorNone, playerDatas: new [] { new PlayerInfo() {PlayerId = -1, Level = 0, Class= "None" } });
        }
    }
    public void Dispose()
    {
        _mySqlDbConnection.Close();
    }
}
