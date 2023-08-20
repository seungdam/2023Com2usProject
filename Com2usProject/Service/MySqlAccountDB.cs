using Com2usProject.DataModel;
using Com2usProject.SecurityUtil;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Compilers;
using SqlKata.Execution;
using StackExchange.Redis;
using System.Data;
using ZLogger;




namespace Com2usProject.Service;


public class DbConnectionStrings
{
    public string MySqlAccountDB { get; set; }
    public string RedisTockenDB { get; set; }
}

public class MySqlAccountDb : IAccountDb // 해당 Account 클래스는 MySql을 사용하므로 클래스명을 MySqlAccountDb라고 짓는다
{
    readonly ILogger<MySqlAccountDb> _logger;
    readonly IRedisDb _redisDb;
    readonly IOptions<DbConnectionStrings> _dbConfig;

    MyPasswordHasher _pwhasher;
    Compiler         _mySqlComplier;
    QueryFactory     _mySqlQueryFactory;
    IDbConnection    _mySqlDbConnection; //  IDbConnection 또한 한가지 DB에 국한된 것이 아닌 다양한 DBConnection을 사용할 수 있도록 하는 인터페이스이다.
    
    
    
    
    public MySqlAccountDb(ILogger<MySqlAccountDb> logger, IOptions<DbConnectionStrings> dbconfig)
    {
        _logger = logger;
        _dbConfig = dbconfig;
        
        _mySqlDbConnection = new MySqlConnection(dbconfig.Value.MySqlAccountDB);
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
        
        var existAccountInfo = await _mySqlQueryFactory.Query("clientlogininfo").Where("Email", email).ExistsAsync();
        if (existAccountInfo)
        {
            _logger.ZLogError($"[RegisterAccount] ErrorCode: {CSCommon.ErrorCode.RegisterErrorAlreadyExist}, Email: {email} \n");
            return CSCommon.ErrorCode.RegisterErrorAlreadyExist;

        }
        _pwhasher = new MyPasswordHasher();
        string hashPassword =  _pwhasher.HashingPassword(pw);
        
        var count = await _mySqlQueryFactory.Query("clientlogininfo").InsertAsync(new
        {
            Email = email,
            HashPassword = hashPassword
        });
        
        
        if (count != 1)
        {
            _logger.ZLogError($"[AccountDb.CreateAccount] ErrorCode: {CSCommon.ErrorCode.RegisterErrorFailToInsert}, Email: {email} \n");
            return CSCommon.ErrorCode.RegisterErrorFailToInsert;
        }
       
        return CSCommon.ErrorCode.ErrorNone;
    }

    public async Task<CSCommon.ErrorCode> VerifyAccount(String email, String pw)
    {
        var isAccountExist = await _mySqlQueryFactory.Query("clientlogininfo").Where("Email", email).ExistsAsync();
        if (!isAccountExist) return CSCommon.ErrorCode.LoginErrorNoExist;

        var LoginData = await _mySqlQueryFactory.Query("clientlogininfo").Where("Email", email).FirstOrDefaultAsync<AccountModel>();

        if(_pwhasher.VerifyPassword(pw, LoginData.HashPassword)) // 만약 해시 검증에 성공했다면 토큰을 부여하고 
        {
            return CSCommon.ErrorCode.ErrorNone;
        }
        else
        {

            _logger.ZLogError($"[RegisterAccount] ErrorCode: {CSCommon.ErrorCode.LoginErrorInvalidPassword}, Email: {email} \n");

            return CSCommon.ErrorCode.LoginErrorInvalidPassword;
        }
    }

}
