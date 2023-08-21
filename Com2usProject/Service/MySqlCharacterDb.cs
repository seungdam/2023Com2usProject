using Com2usProject.DataModel;
using Com2usProject.SecurityUtil;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Data;

namespace Com2usProject.Service;

public class MySqlCharacterDb : ICharacterDb
{
    readonly ILogger<MySqlCharacterDb> _logger;
    readonly IRedisDb _redisDb;
    readonly IOptions<DbConnectionStrings> _dbConfig;

    Compiler _mySqlComplier;
    QueryFactory _mySqlQueryFactory;
    IDbConnection _mySqlDbConnection;


    public MySqlCharacterDb(ILogger<MySqlCharacterDb> logger, IOptions<DbConnectionStrings> dbconfig)
    {
        _logger = logger;
        _dbConfig = dbconfig;

        _mySqlDbConnection = new MySqlConnection(dbconfig.Value.MySqlCharacterDb);
        _mySqlDbConnection.Open();


        _mySqlComplier = new MySqlCompiler();
        _mySqlQueryFactory = new QueryFactory(_mySqlDbConnection, _mySqlComplier);
    }

    public async Task<Tuple<CSCommon.ErrorCode, String>> GetCharacterData(String id)
    {
        var bIsExist = await _mySqlQueryFactory.Query("chardb").Where("Id", id).ExistsAsync();
        string charJsonString;
        CSCommon.ErrorCode errorcode;
        if (bIsExist)
        {
           charJsonString = await _mySqlQueryFactory.Query("chardb").Select("CharInfo").Where("Id", id).FirstOrDefaultAsync<String>();

            if (charJsonString is not null) errorcode = CSCommon.ErrorCode.ErrorNone;
            else
            {
                charJsonString = "None";
                errorcode = CSCommon.ErrorCode.LoadCharacterErrorInvaildId;
            } 
        }
        else
        {
            charJsonString = "None";
            errorcode = CSCommon.ErrorCode.LoadCharacterErrorNoExist;
        }

        return new Tuple<CSCommon.ErrorCode, String>(errorcode,charJsonString);
    }
    //public async Task<CSCommon.ErrorCode> UpdateCharacterData(String id)
    //{ 
    //}
}
