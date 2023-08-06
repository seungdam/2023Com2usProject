using CSCommon;

namespace Com2usWebProject.Services
{
    public interface IAccountDB : IDisposable
    {
        public Task<ErrorCode> CreateAccountAsync(String id, String pw);

        //public Task<Tuple<ErrorCode, Int64>> VerifyAccount(String email, String pw);
    }
}
