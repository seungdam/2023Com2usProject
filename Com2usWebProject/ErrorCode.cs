using System;


namespace CSCommon;
public enum ErrorCode : System.UInt16
{

    None = 0,
    // Account & Login Error
    CreateAccountFailException = 2001,
    CreateAccountFailInsert = 2002,
    LoginFailUserNotExist = 2100,
}

