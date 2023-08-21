
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

    LoadCharacterErrorNoExist,
    LoadCharacterErrorException,
    LoadCharacterErrorInvaildId,

 }

