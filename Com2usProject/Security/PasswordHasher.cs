using System;
using System.Reflection.Emit;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;

namespace Com2usProject.AccountSecurity;

public sealed class MyPasswordHasher
{
    int _saltBytesSize = 16;
    int _hashBytesSize = 32;

    public MyPasswordHasher()
    {

    }
    // Salt와 Digest 방법을 혼용해서 사용하자
    public string HashingPassword(string password)
    {
        byte[] saltValue = new byte[16]; // 패스워드 해싱에서 사용할 salt 값

        using (var rngCsp = new RNGCryptoServiceProvider())
        {
            rngCsp.GetNonZeroBytes(saltValue); // 0이 아닌 난수값을 할당시켜준다.
        }

        // pbkdf2 라이브러리 함수를 활용해 해싱을 진행
        byte[] hashedValue = KeyDerivation.Pbkdf2(
            password: password,
            salt: saltValue,
            prf: KeyDerivationPrf.HMACSHA256, // SHA256
            iterationCount: 10000, // 10000번 정도 재해싱 수행한다. 
            numBytesRequested: _hashBytesSize);

        byte[] finalHashBytes = new byte[_hashBytesSize + _saltBytesSize];
        Array.Copy(saltValue, 0, finalHashBytes, 0, _saltBytesSize);
        Array.Copy(hashedValue, 0, finalHashBytes, _saltBytesSize, _hashBytesSize);
        // 앞에 16바이트는 salt 값을 뒤 32바이트는 해쉬된 값을 넣어 DB에 저장. --> 나중에 검증할 때 사용할 것임
        string hashPassword = Convert.ToBase64String(finalHashBytes);
        return hashPassword;
    }

    public bool VerifyPassword(string password, string hashPassword)
    {
        // 데이터베이스에서 해시 패스워드 값을 받아와 이를 저장할 예정
        byte[] hashBytes = Convert.FromBase64String(hashPassword);

        byte[] restoreSaltValue = new byte[_saltBytesSize];
        byte[] restoreHashValue = new byte[_hashBytesSize];

        Array.Copy(hashBytes, 0, restoreSaltValue, 0, _saltBytesSize);
        Array.Copy(hashBytes, _saltBytesSize, restoreHashValue, 0, _hashBytesSize);

        byte[] compareHashedValue = KeyDerivation.Pbkdf2(
            password: password,
            salt: restoreSaltValue,
            prf: KeyDerivationPrf.HMACSHA256, // SHA256
            iterationCount: 10000, // 10000번 정도 재해싱 수행한다. 
            numBytesRequested: _hashBytesSize);

        return restoreHashValue.SequenceEqual(compareHashedValue);

    }


}
