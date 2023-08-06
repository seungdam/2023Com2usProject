using CSCommon;

namespace Com2usWebProject.Services;

public interface IAccountDB : IDisposable
{
    public Task<ErrorCode> CreateAccountAsync(String id, String pw);

    public Task<CSCommon.ErrorCode> VerifyAccount(String email, String pw);
}
