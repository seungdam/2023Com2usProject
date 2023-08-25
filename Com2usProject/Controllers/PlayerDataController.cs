using Com2usProject.DataModel;
using Com2usProject.ReqResModel;
using Com2usProject.Repository;
using Microsoft.AspNetCore.Mvc;

using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Com2usProject.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlayerDataController : ControllerBase
{
    readonly IInGameDb _gameDb;
    readonly ILogger<PlayerDataController> _logger;

    public PlayerDataController(ILogger<PlayerDataController> logger, IInGameDb characterDb)
    {
        _logger = logger;
        _gameDb = characterDb;
    }

 
}
