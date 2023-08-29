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
    public string RedisDb { get; set; }
    public string PlayerRequestRedisDb { get; set; }
}

public class HandleAccount : IAccountDb // 해당 Account 클래스는 MySql을 사용하므로 클래스명을 MySqlAccountDb라고 짓는다
{
    readonly ILogger<HandleAccount> _logger;
    //readonly IRedisDb _redisDb;


    readonly IInGameDb _inGameDb;
    MyPasswordHasher _pwhasher;

    //Compiler         _mySqlComplier;
    //QueryFactory     _mySqlQueryFactory;
    //IDbConnection    _mySqlDbConnection; //  IDbConnection 또한 한가지 DB에 국한된 것이 아닌 다양한 DBConnection을 사용할 수 있도록 하는 인터페이스이다.

    //public AccountDb(ILogger<AccountDb> logger, IOptions<DbConnectionStrings> dbconfig)
    //{
    //    _logger = logger;
    //    _dbConfig = dbconfig;

    //    _mySqlDbConnection = new MySqlConnection(dbconfig.Value.MySqlGameDb);
    //    _mySqlDbConnection.Open();


    //    _mySqlComplier = new MySqlCompiler();
    //    _mySqlQueryFactory =  new QueryFactory(_mySqlDbConnection,_mySqlComplier);
    //    _pwhasher = new MyPasswordHasher();
    //}


    public HandleAccount(ILogger<HandleAccount> logger, IInGameDb gameDb)
    {
        _logger = logger;
        _inGameDb = gameDb;
        _pwhasher = new MyPasswordHasher();
    }

    //public void Dispose()
    //{ 
    //    _mySqlDbConnection.Close();
    //}

    public async Task<ErrorCode> RegisterAccount(string email, string pw)
    {

        var existAccountInfo = await _inGameDb.GetQueryFactory().Query("accountinfo").Where("Email", email).ExistsAsync();

        if (existAccountInfo)
        {
            _logger.ZLogError($"[AccountRepository.RegisterAccount] ErrorCode: {ErrorCode.RegisterErrorAlreadyExist}, Email: {email} \n");
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
            _logger.ZLogError($"[AccountRepository.CreateAccount] ErrorCode: {ErrorCode.RegisterErrorFailToInsert}, Email: {email} \n");
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
