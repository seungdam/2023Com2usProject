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

namespace Com2usProject.Repository;


public class DbConnectionStrings
{
    public string MySqlGameDb { get; set; }
    public string RedisDb { get; set; }
    public string PlayerRequestRedisDb{ get; set; }
}

public class AccountDb : IAccountDb // 해당 Account 클래스는 MySql을 사용하므로 클래스명을 MySqlAccountDb라고 짓는다
{
    readonly ILogger<AccountDb> _logger;
    readonly IRedisDb _redisDb;
    readonly IOptions<DbConnectionStrings> _dbConfig;

    MyPasswordHasher _pwhasher;
    Compiler         _mySqlComplier;
    QueryFactory     _mySqlQueryFactory;
    IDbConnection    _mySqlDbConnection; //  IDbConnection 또한 한가지 DB에 국한된 것이 아닌 다양한 DBConnection을 사용할 수 있도록 하는 인터페이스이다.
    
    
    
    
    public AccountDb(ILogger<AccountDb> logger, IOptions<DbConnectionStrings> dbconfig)
    {
        _logger = logger;
        _dbConfig = dbconfig;
        
        _mySqlDbConnection = new MySqlConnection(dbconfig.Value.MySqlGameDb);
        _mySqlDbConnection.Open();

       
        _mySqlComplier = new MySqlCompiler();
        _mySqlQueryFactory =  new QueryFactory(_mySqlDbConnection,_mySqlComplier);
        _pwhasher = new MyPasswordHasher();
    }

    public void Dispose()
    { 
        _mySqlDbConnection.Close();
    }

    public async Task<CSCommon.ErrorCode> RegisterAccount(String email, String pw)
    {
        
        var existAccountInfo = await _mySqlQueryFactory.Query("accountinfo").Where("Email", email).ExistsAsync();
       
        if (existAccountInfo)
        {
            _logger.ZLogError($"[AccountRepository.RegisterAccount] ErrorCode: {CSCommon.ErrorCode.RegisterErrorAlreadyExist}, Email: {email} \n");
            return CSCommon.ErrorCode.RegisterErrorAlreadyExist;

        }
        _pwhasher = new MyPasswordHasher();
        string hashPassword =  _pwhasher.HashingPassword(pw);
        
        var count = await _mySqlQueryFactory.Query("accountinfo").InsertAsync(new
        {
            Email = email,
            HashPassword = hashPassword
        });
        
        
        if (count != 1)
        {
            _logger.ZLogError($"[AccountRepository.CreateAccount] ErrorCode: {CSCommon.ErrorCode.RegisterErrorFailToInsert}, Email: {email} \n");
            return CSCommon.ErrorCode.RegisterErrorFailToInsert;
        }
       
        return CSCommon.ErrorCode.ErrorNone;
    }

    public async Task<CSCommon.ErrorCode> VerifyAccount(String email, String pw)
    {
        var isAccountExist = await _mySqlQueryFactory.Query("accountinfo").Where("Email", email).ExistsAsync();
        if (!isAccountExist)
        {
            _logger.ZLogError($"[AccountRepository.VerifyAccount] ErrorCode: {CSCommon.ErrorCode.LoginErrorNoExist}\n");
            return CSCommon.ErrorCode.LoginErrorNoExist;
        }


        var LoginData = await _mySqlQueryFactory.Query("accountinfo").Where("Email", email).FirstAsync<AccountInfo>();

        if(_pwhasher.VerifyPassword(pw, LoginData.HashPassword)) // 만약 해시 검증에 성공했다면 토큰을 부여하고 
        {
            return CSCommon.ErrorCode.ErrorNone;
        }
        else
        {
            _logger.ZLogError($"[AccountRepository.VerifyAccount] ErrorCode: {CSCommon.ErrorCode.LoginErrorInvalidPassword}, Email: {email} \n");
            return CSCommon.ErrorCode.LoginErrorInvalidPassword;
        }
    }

}
