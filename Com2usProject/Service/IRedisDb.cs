namespace Com2usProject.Service;

public interface IRedisDb : IDisposable
{
    public Task<bool> VerifyUserToken(string token); // 토큰 검증하기
    public Task<CSCommon.ErrorCode> RegisterAuthToken(string email, string token);

    public Task<Tuple<CSCommon.ErrorCode, String>> GetUserToken(string email);
}
