using System;


namespace CSCommon;
public enum ErrorCode : System.UInt16
{

    None = 0,
    // Account & Login Error
    CreateAccountFailException = 2001,
    CreateAccountFailInsert = 2002,
    CreateAccountFailAlreadyExist = 2003,

    LoginFailUserNotExist = 2100,
    LoginFailPasswordNotExist = 2101,
    LoginFailPwNotMatch = 2102,
    LoginFailInvalidPassword  = 2103,
    LoginFailException = 2104,
}

