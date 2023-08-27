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
    RedisConnection _redisConn;
    RedisConfig _redisConfig;

    public RedisDb(ILogger<RedisDb> logger, IOptions<DbConnectionStrings> dbconfig)
    {
        _logger = logger;
        _dbConfig = dbconfig;

        try
        {
            _redisConfig = new RedisConfig("AuthTokenRedisDb", dbconfig.Value.RedisDb);
            _redisConn = new RedisConnection(_redisConfig);
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
            var redisQuery = new RedisString<string>(_redisConn, token, null);
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
           
            var redisQuery = new RedisString<string>(_redisConn, token, null);
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

    public RedisConnection GetConnection() {  return _redisConn; }
    public void Dispose()
    {
        _redisConn.GetConnection().Close();
    }
}
