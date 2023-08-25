using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using CloudStructures;
using CloudStructures.Structures;
using CSCommon;
using ZLogger;

namespace Com2usProject.Repository;

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

        try
        {
            _rConfig = new RedisConfig("AuthTokenRedisDb", dbconfig.Value.RedisTockenDb);
            _rConn = new RedisConnection(_rConfig);
        }
        catch (Exception ex)
        {
            _logger.ZLogError("Redis Conn Error");
        }
    }

   public async Task<bool> CheckAuthTokenExist(string token)
    {

        try
        {
            var redisQuery = new RedisString<string>(_rConn, token, null);
            var result = await redisQuery.ExistsAsync(); // 토큰이 존재하는가?
            if (!result) return true;
            else return false;
            
        }
        catch(Exception e)
        {

            _logger.ZLogError("Something Error Occur At CheckAuthToken. Plz Check This Code");
            return false;
        }
        
    }

    public async Task<CSCommon.ErrorCode> RegisterAuthToken(string email, string token)
    {
        try
        {
           
            var redisQuery = new RedisString<string>(_rConn, token, null);
            var result = await redisQuery.SetAsync(email); // 

            if (!result) return CSCommon.ErrorCode.RedisErrorFailToAddToken;
        }
        catch (Exception e)
        {


            _logger.ZLogError("Something Error Occur At AddAuthToken. Plz Check This Code");
            return ErrorCode.RedisErrorException;
            
        }

        return CSCommon.ErrorCode.ErrorNone;
    }


    
    public void Dispose()
    {
        _rConn.GetConnection().Close();
    }
}
