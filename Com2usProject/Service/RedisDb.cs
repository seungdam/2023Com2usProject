using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using CloudStructures;
using CloudStructures.Structures;
using CSCommon;
using System.ComponentModel.DataAnnotations;
using ZLogger;

namespace Com2usProject.Service;

public class RedisDb : IRedisDb
{
    readonly ILogger<RedisDb> _logger;
    readonly IOptions<DbConnectionStrings> _dbConfig;
    RedisConnection _rConn;
    RedisConfig _rConfig;
    public RedisDb(ILogger<RedisDb> logger, IOptions<DbConnectionStrings> dbconfig)
    {
        _logger = logger;
        _dbConfig = dbconfig;
        _rConfig = new RedisConfig("AuthToken", dbconfig.Value.RedisTockenDB);
        _rConn = new RedisConnection(_rConfig);

    }
   public async Task<bool> CheckAuthTokenExist(string token)
    {

        try
        {
            var redisQuery = new RedisList<string>(_rConn, token, null);
            var result = await redisQuery.ExistsAsync(); // 토큰이 존재하는가?
            if (!result) return true;
        }
        catch(Exception e)
        {

            _logger.ZLogError("Something Error Occur. Plz Check This Code");
            return false;
        }
        
        return true;
    }

    

    public async Task<CSCommon.ErrorCode> AddAuthToken(string email, string token)
    {
        try
        {
            // 발급받은 토큰은 Token이라는 키를 가지는 String 자료형 통해 관리하도록 한다.
            var redisQuery = new RedisString<string>(_rConn, token, null);
            var result = await redisQuery.SetAsync(email); // LPush Method를 사용해서 등록

            if (!result) return CSCommon.ErrorCode.RedisErrorFailToAddToken;
        }
        catch (Exception e)
        {

            return ErrorCode.RedisErrorException;
            
        }

        return CSCommon.ErrorCode.ErrorNone;
    }


    
    public void Dispose()
    {
        _rConn.GetConnection().Close();
    }
}
