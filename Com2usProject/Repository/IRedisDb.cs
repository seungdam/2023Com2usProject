﻿using CloudStructures;
using CSCommon;

namespace Com2usProject.Repository;

public interface IRedisDb : IDisposable
{
  
    public Task<CSCommon.ErrorCode> RegisterAuthToken(string email, string token); // 토큰 추가하기

    public Task<bool> CheckAuthTokenExist(string token);
    public Task<bool> StartPlayerRequest(int PlayerId, int Type);

    public void FinishPlayerRequest(int PlayerId);
    public RedisConnection GetConnection();
}
