using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using CloudStructures;
using CloudStructures.Structures;
using CSCommon;

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
   public async Task<bool> VerifyUserToken(string token)
    {

        return false;
    }


    public async Task<CSCommon.ErrorCode> RegisterAuthToken(string email, string token)
    {
        try
        {
            var redis = new RedisString<string>(_rConn, email, TimeSpan.FromMinutes(15));
            await redis.SetAsync(token);
        }
        catch (Exception e)
        {

            return ErrorCode.RedisErrorException;
            
        }

        return CSCommon.ErrorCode.ErrorNone;
    }


    public async Task<Tuple<CSCommon.ErrorCode, String>> GetUserToken(string email)
    {
        var query = new RedisString<String>(_rConn, email, null); // email key에 해당하는 쿼리 생성
        var token = await query.GetAsync();

        var result = new Tuple<CSCommon.ErrorCode, String>(CSCommon.ErrorCode.ErrorNone,token.Value);

        return result; 
    }
    public void Dispose()
    {
        _rConn.GetConnection().Close();
    }
}
