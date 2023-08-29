
namespace CSCommon;
 public enum ErrorCode
 {
    ErrorNone = 0,
    RegisterErrorAlreadyExist,
    RegisterErrorFailToInsert,
    RegisterErrorException,

    LoginErrorNoExist,
    LoginErrorAlreadyEnter,
    LoginErrorInvalidPassword,
    LoginErrorException,

    RedisErrorFailToAddToken,
    RedisErrorTokenNoExist,
    RedisErrorException,

    LoadPlayerDataErrorNoExist,
    LoadPlayerDataErrorException,
    LoadPlayerDataErrorInvaildId,

 }

public enum RequestType
{
    LoadInventoryDataInfos,
    InsertItemToInventory,
    DeleteItemToInventory,

    LoadMailBoxDataInfos,
    InsertMailBox,
    DeleteMailBox,
}
