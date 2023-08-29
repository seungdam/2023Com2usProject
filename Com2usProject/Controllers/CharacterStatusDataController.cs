using Com2usProject.DataModel;
using Com2usProject.ReqResModel;
using Com2usProject.Repository;
using Microsoft.AspNetCore.Mvc;

using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text.Json.Nodes;
using ZLogger;
using System;
using Newtonsoft.Json;
using Com2usProject.ServiceInterface;

namespace Com2usProject.Controllers;


[ApiController]
[Route("api/[controller]")]
public class CharacterStatusDataController : ControllerBase
{
    readonly IPlayableCharacterStatusData _characterStatusHandler;


    readonly IRedisDb  _redisDb;
    readonly ILogger<CharacterStatusDataController> _logger;

    public CharacterStatusDataController(ILogger<CharacterStatusDataController> logger, IPlayableCharacterStatusData csHandler, IRedisDb redisDb)
    {
        _logger = logger;
        _characterStatusHandler = csHandler;
        _redisDb = redisDb;
    }



   

    

}
