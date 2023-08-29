using Com2usProject.DataModel;
using CSCommon;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Compilers;
using SqlKata.Execution;
using StackExchange.Redis;
using System.Data;
using ZLogger;
using Com2usProject.AccountSecurity;
using Com2usProject.Repository;
using Com2usProject.ServiceInterface;

namespace Com2usProject.Service;


public class DbConnectionStrings
{
    public string MySqlGameDb { get; set; }
    public string RedisTokenDb { get; set; }
    public string RedisRequestDb { get; set; }
}

public class HandleAccount : IAccountDb // 해당 Account 클래스는 MySql을 사용하므로 클래스명을 MySqlAccountDb라고 짓는다
{
    readonly ILogger<HandleAccount> _logger;
    //readonly IRedisDb _redisDb;


    readonly IInGameDb _inGameDb;
    MyPasswordHasher _pwhasher;


    public HandleAccount(ILogger<HandleAccount> logger, IInGameDb gameDb)
    {
        _logger = logger;
        _inGameDb = gameDb;
        _pwhasher = new MyPasswordHasher();
    }

    public async Task<ErrorCode> RegisterAccount(string email, string pw)
    {

        var existAccountInfo = await _inGameDb.GetQueryFactory().Query("accountinfo").Where("Email", email).ExistsAsync();

        if (existAccountInfo)
        {
            _logger.ZLogError($"[HandleAccount.RegisterAccount] ErrorCode: {ErrorCode.RegisterErrorAlreadyExist}, Email: {email} \n");
            return ErrorCode.RegisterErrorAlreadyExist;

        }
        _pwhasher = new MyPasswordHasher();
        string hashPassword = _pwhasher.HashingPassword(pw);

        var count = await _inGameDb.GetQueryFactory().Query("accountinfo").InsertAsync(new
        {
            Email = email,
            HashPassword = hashPassword
        });


        if (count != 1)
        {
            _logger.ZLogError($"[HandleAccount.CreateAccount] ErrorCode: {ErrorCode.RegisterErrorFailToInsert}, Email: {email} \n");
            return ErrorCode.RegisterErrorFailToInsert;
        }

        return ErrorCode.ErrorNone;
    }

    public async Task<ErrorCode> VerifyAccount(string email, string pw)
    {
        var isAccountExist = await _inGameDb.GetQueryFactory().Query("accountinfo").Where("Email", email).ExistsAsync();
        if (!isAccountExist)
        {
            _logger.ZLogError($"[AccountRepository.VerifyAccount] ErrorCode: {ErrorCode.LoginErrorNoExist}\n");
            return ErrorCode.LoginErrorNoExist;
        }


        var LoginData = await _inGameDb.GetQueryFactory().Query("accountinfo").Where("Email", email).FirstAsync<AccountInfo>();

        if (_pwhasher.VerifyPassword(pw, LoginData.HashPassword)) // 만약 해시 검증에 성공했다면 토큰을 부여하고 
        {
            return ErrorCode.ErrorNone;
        }
        else
        {
            _logger.ZLogError($"[AccountRepository.VerifyAccount] ErrorCode: {ErrorCode.LoginErrorInvalidPassword}, Email: {email} \n");
            return ErrorCode.LoginErrorInvalidPassword;
        }
    }

}
