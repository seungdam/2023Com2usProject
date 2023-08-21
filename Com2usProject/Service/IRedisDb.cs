namespace Com2usProject.Service;

public interface IRedisDb : IDisposable
{
  
    public Task<CSCommon.ErrorCode> AddAuthToken(string email, string token); // 토큰 추가하기

    public Task<bool> CheckAuthTokenExist(string token);
}
