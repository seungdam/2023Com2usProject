using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using CloudStructures;
using CloudStructures.Structures;
using CSCommon;
using ZLogger;
using Com2usProject.Service;

namespace Com2usProject.Repository;

public class RedisDb : IRedisDb
{
    readonly ILogger<RedisDb> _logger;
    readonly IOptions<DbConnectionStrings> _dbConfig;
    RedisConnection _redisConn;
    RedisConnection _redisGameConn;
    RedisConfig _redisConfig;
    RedisConfig _redisGameConfig;

    public RedisDb(ILogger<RedisDb> logger, IOptions<DbConnectionStrings> dbconfig)
    {
        _logger = logger;
        _dbConfig = dbconfig;

        try
        {
            _redisConfig = new RedisConfig("AuthTokenRedisDb", dbconfig.Value.RedisTokenDb);
            _redisConn = new RedisConnection(_redisConfig);

            _redisGameConfig = new RedisConfig("DbCommandRedisDb", dbconfig.Value.RedisRequestDb);
            _redisGameConn = new RedisConnection(_redisGameConfig);

        }
        catch
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
            if (!result) return false;
            else return true;
            
        }
        catch
        {

            _logger.ZLogError("Something Error Occur At CheckAuthToken. Plz Check This Code");
            return false;
        }
        
    }

    public async Task<CSCommon.ErrorCode> RegisterAuthToken(string Email, string Token)
    {
        try
        {
           
            var redisQuery = new RedisString<string>(_redisConn, Token, null);
            var result = await redisQuery.SetAsync(Email); // 

            if (!result) return CSCommon.ErrorCode.RedisErrorFailToAddToken;
        }
        catch
        { 
            _logger.ZLogError("Something Error Occur At AddAuthToken. Plz Check This Code");
            return ErrorCode.RedisErrorException;
            
        }

        return CSCommon.ErrorCode.ErrorNone;
    }


    public async Task<bool> StartPlayerRequest(int PlayerId, int Type)
    {
        try
        {
            var redisQuery = new RedisString<int>(_redisGameConn, PlayerId.ToString(),TimeSpan.FromSeconds(2));
            var result = await redisQuery.SetAsync(Type, when:When.NotExists);
            if (!result) throw new Exception();
        }
        catch
        {
            _logger.ZLogError($"[RedisDb.RegisterPlayerRequest] ErrorCode : {CSCommon.ErrorCode.RedisErrorException}");
            return false;
        } 

        return true;
    }

    public async void FinishPlayerRequest(int PlayerId)
    {
        try
        {
            var redisQuery = new RedisString<CSCommon.RequestType>(_redisGameConn, PlayerId.ToString(), null);
            var result = await redisQuery.DeleteAsync();
            if (!result) throw new Exception();
        }
        catch
        {
            _logger.ZLogError($"[RedisDb.FinishPlayerRequest] ErrorCode : {CSCommon.ErrorCode.RedisErrorException}");
        }

    }

    public RedisConnection GetConnection() { return _redisGameConn; }
    public void Dispose()
    {
        _redisConn.GetConnection().Close();
    }
}
