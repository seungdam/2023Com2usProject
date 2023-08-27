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
public class LoadPlayerDataController : ControllerBase
{
    readonly IInGameDb _gameDb;
    readonly ILogger<LoadPlayerDataController> _logger;

    public LoadPlayerDataController(ILogger<LoadPlayerDataController> logger, IInGameDb characterDb)
    {
        _logger = logger;
        _gameDb = characterDb;
    }


    [HttpPost("/LoadPlayerInventory")]
    public async Task<bool> LoadInventoryData(LoadPlayerDataReq request)
    {
        return true;
    }

    [HttpPost("/LoadPlayerMail")]
    public async Task<MailInfo>

}
