using Com2usProject.DataModel;
using Com2usProject.ReqResModel;
using Com2usProject.Service;
using Microsoft.AspNetCore.Mvc;

using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Com2usProject.Controllers;

[Route("loadGameData/[controller]")]
[ApiController]
public class CharacterDbController : ControllerBase
{
    readonly ICharacterDb _gameDb;
    readonly ILogger<CharacterDbController> _logger;

    public CharacterDbController(ILogger<CharacterDbController> logger, ICharacterDb characterDb)
    {
        _logger = logger;
        _gameDb = characterDb;
    }


    // 임시 구현 이후 수정할 예정이다.
    [HttpPost]
    public async Task<LoadCharacterDataRes> LoadCharacterData(LoadCharacterDataReq request)
    {
        var charData = await _gameDb.GetCharacterData(request.Id);
   
        if (charData.Item1 == CSCommon.ErrorCode.ErrorNone)
        {
            CharacterModel jCharacterData = JsonSerializer.Deserialize<CharacterModel>(charData.Item2);
            LoadCharacterDataRes response = new LoadCharacterDataRes();
            response.charData = jCharacterData;
            return response;
        }
        else return null;
    }

    //[HttpPost]
    //public async Task<LoadCharacterDataRes> UpdateCharacterData(LoadCharacterDataReq request)
    //{

    //}

}
