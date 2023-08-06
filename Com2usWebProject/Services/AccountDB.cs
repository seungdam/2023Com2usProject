using MySqlConnector;
using System.Data;
using SqlKata.Extensions;
using Microsoft.Extensions.Options;
using SqlKata.Execution;
using SqlKata.Compilers;
using System.Threading.Tasks;

using Com2usWebProject.ModelDB;
using ZLogger;
using SqlKata;
using System.Security.Principal;

public class DBInfo
{
   public string AccountDB { get; set; }
   public string GameDB { get; set; }

}

namespace Com2usWebProject.Services
{
    //private readonly App
    public class AccountDB : IAccountDB
    {
        private readonly ILogger m_logger;
        private readonly IOptions<DBInfo> m_dbInfo;

        private IDbConnection m_con;
        private QueryFactory m_qf;
        private MySqlCompiler m_sCompiler;
        public AccountDB(IOptions<DBInfo> dbinfo)
        {
            m_dbInfo = dbinfo; // 데이터베이스 정보 받아오기
            m_con = new MySqlConnection(m_dbInfo.Value.AccountDB); // 받아온 DBConfig 정보를 기반으로 커넥션 객체 생성
            m_con.Open(); // 연결 오픈

            m_sCompiler = new MySqlCompiler(); // 쿼리문 컴파일을 위한 sql 컴파일러 객체 생성
            m_qf = new QueryFactory(m_con, m_sCompiler); // sql 쿼리문 해석을 위한 쿼리 팩토리 객체 생성
        }
        ~AccountDB()
        {
            Dispose();
        }

        // IDisposable 인터페이스 기본 요소
        public void Dispose()
        {
            m_con.Dispose();
        }


        public async Task<CSCommon.ErrorCode> CreateAccountAsync(String email, String pw)
        {
            try
            {
                var existAccountInfo = await m_qf.Query("clientlogininfo").Where("Email",email).ExistsAsync();
                if (existAccountInfo)
                {
                   
                    return CSCommon.ErrorCode.CreateAccountFailAlreadyExist;
                }
                var count = await m_qf.Query("clientlogininfo").InsertAsync(new
                {
                    Email = email,
                    Pw = pw
                });

               
                if (count != 1)
                {
                    //m_logger.ZLogError($"[AccountDb.CreateAccount] ErrorCode: {CSCommon.ErrorCode.CreateAccountFailException}, Email: {email}");
                    return CSCommon.ErrorCode.CreateAccountFailInsert;
                }

                return CSCommon.ErrorCode.None;
            }
            catch (Exception e)
            {
                //m_logger.ZLogError(e,
                //    $"[AccountDb.CreateAccount] ErrorCode: {CSCommon.ErrorCode.CreateAccountFailException}, Email: {email}");
                return CSCommon.ErrorCode.CreateAccountFailException;
            }
        }

        public async Task<CSCommon.ErrorCode> VerifyAccount(String email, String pw)
        {
           
            try
            {
               
                var existAccountInfo = await m_qf.Query("clientlogininfo").Where("Email", email).FirstOrDefaultAsync<AccountModel>();

                if (existAccountInfo is null) // 계정이 존재하지 않는다면
                {
                    return CSCommon.ErrorCode.LoginFailUserNotExist;
                }
                
                if (existAccountInfo.Password != pw) // 비번 틀림
                {
                        //m_logger.ZLogError($"[AccountDb.VerifyAccount] ErrorCode: {CSCommon.ErrorCode.LoginFailPwNotMatch}");
                        return CSCommon.ErrorCode.LoginFailPwNotMatch;
                }

                    // 로그인 성공
                    return CSCommon.ErrorCode.None;
          
            }
            catch (Exception e)
            {
                //m_logger.ZLogError(e,
                   // $"[AccountDb.VerifyAccount] ErrorCode: {CSCommon.ErrorCode.LoginFailException}, Email: {email}");
                return CSCommon.ErrorCode.LoginFailException;
            }
        }
    }

   
    
}
    
