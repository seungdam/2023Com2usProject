﻿using Com2usProject.ReqResModel;
using Com2usProject.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Cryptography;
using ZLogger;
using Com2usProject.ServiceInterface;

namespace Com2usProject.Controllers;


[ApiController]
[Route("api/[controller]")]
public class LoginAccountController : ControllerBase
{
    readonly IAccountDb _accountDb;
    readonly IPlayableCharacterStatusData _gameDbLoader;
    readonly IRedisDb _redisTokenDb;
    readonly ILogger<LoginAccountController> _logger;
    

    public LoginAccountController(ILogger<LoginAccountController> logger, IAccountDb accountDb, IPlayableCharacterStatusData inGameDb,IRedisDb redisDb)
    {
        _logger = logger;
        _accountDb = accountDb;
        _redisTokenDb = redisDb;
        _gameDbLoader = inGameDb;
    }

    [HttpPost("/Login")]
    public async Task<LoginAccountRes> Login(AccountReq request)
    {
        LoginAccountRes response = new LoginAccountRes();
        try
        {
            var verifyResult = await _accountDb.VerifyAccount(request.Email, request.Password);

            if (verifyResult == CSCommon.ErrorCode.ErrorNone)
            {

                response.AuthToken = CreateAuthToken(request.Password);
                var registerTokenReult = await _redisTokenDb.RegisterAuthToken(request.Email, response.AuthToken);
                
             
                var playerListResult = await _gameDbLoader.LoadPlayerCharacterStatusData(request.Email);

                if(playerListResult.ErrorCode == CSCommon.ErrorCode.ErrorNone)
                {
                    response.PlayerInfos = playerListResult.playerInfos;
                }

                if(response.PlayerInfos is not null) foreach(var i in response.PlayerInfos) _logger.ZLogInformation($"[Login Success] Player Id : {i.PlayerId} Class : { i.Class} Level : {i.Level}" );
            }
            else
            {
                
                response.AuthToken = "None";
                response.ErrorCode = verifyResult;
            }
        }
        catch
        {
            _logger.ZLogError("Something Error Occur. Plz Check This Code.");   
        }
        return response;
    }


    public String CreateAuthToken(String Password)
    {
        var rngCsp = new RNGCryptoServiceProvider();
        byte[] Token = new byte[20];
        rngCsp.GetNonZeroBytes(Token);
        String NewAuthToken = Password + Convert.ToBase64String(Token);
        rngCsp.Dispose();

       return NewAuthToken;
    }
}
