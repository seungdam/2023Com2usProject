# 컴투스 한국공대 산학협력 프로젝트 서버 분야 공부 문서 (오승담)

약 4주간 프로젝트를 진행하면서, 제가 직접 공부한 흐름 및 공부하면서 중점적으로 보아야하는 핵심 내용 및 간단한 설명을 담고 있는 문서입니다.

#### C# 에서 주의 깊게 봐야하는 내용
-----------------------------------------------------

+ Task<T>와 스레드에 관한 이해
+ async / await 문법에 관한 이해 
  * 비동기 흐름을 제어합니다.
  * DB와 연동하여 작업을 수행하는 동안 걸리는 작업들에게 사용하면, 블록킹 없이 메시지 루프를 유지할 수 있습니다.
    * Async : 주로 함수에 붙이는 키워드 입니다. 일반적으로 Task<T> / Task 타입과 함께 사용 수 있습니다. 해당 함수가 await 키워드를 품고 있음을 암시합니다.
    * Await : 수행시켜 둔 비동기 작업이 완료될 경우 해당 키워드 다음 작업을 수행하도록 하여 흐름을 제어할 수 있습니다.
  해당 프로젝트에서 전반적인 처리가 모두 async / await으로 수행될 만큼 매우 큰 비중을 차지하고 있는 문법으로 반드시 제대로 숙지되어야 합니다.

```
해당 문법을 사용하는 이유는, 서버의 비동기 처리의 필요 이유와 같습니다. I/O 작업과 같은 오랜 작업을 수행하는 경우, 각 스레드가 이를 작업하기 위해 할당 받아집니다.
스레드가 많이 남아 있을 경우에는 큰 문제가 없겠지만, 스레드 자원이 남아있지 않은 경우 추후에 요청을 수행하는 클라이언트는 서버에서 스레드 자원이 생성되길 기다려야 합니다. (지연 발생)
뿐만 아니라, 우리가 앞으로 구현할 서버는 1000명 ~ n * 10000명의 사람들이 사용할 것이기 때문에 스레드 자원을 아무리 많이 준다고 한들, 해당 현상이 발생하게 될 것입니다.
이런 상황을 방지하기 위해서 비동기적인 처리가 필요합니다. 
```

+ Interface 와 추상 클래스와의 차이점 및 의미하는 바
+ 의존성 주입 --> interface 내용을 공부하면서 함께 봐야하는 내용입니다. 


>> 의존한다의 사전적 의미부터 살펴보는 과정이 필요합니다.
>> 의존하다: B는 A에 의존한다. 라는 관계가 설정되었을 때, 이는 A가 변한다면 B도 변한다. 라는 것을 의미합니다.  C#에서는 이를 Interface를 활용해서 표현합니다.
>> 이런 내부적인 의존성을 외부에서 런타임 시, 결정하도록 하기 위해 생긴 방법이 DI(의존성 주입) 빌드 패턴 입니다.
>> 의존성이 강할 수록 해당 객체는 확장성이 좋지 않게 됩니다. 모든 객체를 무조건 느슨한 관계로 표현하는 것이 좋은 것은 아니지만, 이를 잘 설계하면 유지보수성이 높게 프로젝트를 작업할 수 있습니다.



```
예를 들어, 물건을 판매하기 위한 Store 클래스가 있다고 가정합니다. 해당 Store에는 펜을 판매합니다.

그렇다면 이 Store 클래스는 생성자 호출 시, _pen = new Pen() 형태로 이루어질 것입니다.
현재 Store는 Pen이라는 클래스에 강하게 의존하고 있습니다. 이런 경우 발생할 수 있는 문제점은 다음과 같습니다.

1.	두 클래스 간의 관계가 강하게 결합되어 있어 만약 Store에서 다른 물건을 팔려고 시도한다면? 해당 Store 클래스의 전반적인 내용이 변경하거나 불필요한 상속 선언을 수행해야 한다.
2.	객체와 객체 간의 관계가 아닌 클래스와 클래스 간의 관계가 맺어짐으로써 객체 지향적이지 못하다.

해당 문제를 해결하기 위해 Pen, 혹은 다른 상품들을 추상적으로 묶어 처리할 수 있는 인터페이스가 필요합니다. 
이때 우리가 공부한 Interface 키워드를 활용해 Product (상품이라는 인터페이스)라는 새로운 인터페이스를 정의해 이런 현상을 방지할 수 있습니다.
```



의존성 주입에도 여러가지 방법이 존재합니다.
  1.	생성자 주입
  2.	수정자 주입
  3.	필드 주입
각 주입법을 통해 다양하게 생성이 가능한데, asp .net core에서는 보통은 생성자 주입법을 보편적으로 사용하는 것 같습니다.


#### Redis
------------------------------------------------------

##### 기본 개념
 
 + Redis는 메모리 DB로써, 일반적인 DB와 다르게 캐싱을 목적으로 사용한다. 이로써 DB에 직접적으로 접근하여 데이터를 가져오는데 발생하는 시간을 줄일 수 있습니다.
 + Redis는 기본적으로 싱글 스레드로 동작하기 때문에 스레드 세이프하다고 볼 수 있습니다.
 + C#에선 이를 활용하기 위해 StackExchange.Redis 와 CloudStructure 라이브러리를 활용합니다. (매우 중요)
 + REST API 요청에 관해, 순차적인 처리를 하기 위해서 해당 라이브러리의 메소드를 활용하기도 합니다.
 + 주로 사용하는 자료형은 String이며 해당 문서는 String과 관련된 명령어만 정리했습니다.
 + 좀 더 자세한 내용은 RedisGate 사이트를 통해 자세한 정보를 볼 수 있습니다.

###### 주요 Data Structure
 1. String
 + key - value가 1:1 관계인 자료 구조
 + 알파벳,숫자,한글 이미지 등이 사용 가능한 안전 바이너리 문자열.
 + 최대길이 512mb
 + 키 구분 시, _를 사용하는 것이 효율적(사용자 관점)

###### Command
 - SET : 데이터를 저장하는 명령어. 중복된 키 값 선언 시, 마지막으로 명령한 key value 값으로 대체. 
   (NX 데이터베이스에 키값이 존재하지 않을 때만 수행)
   (XX 데이터베이스에 키값이 존재할 때만 수행)
   (EX 데이터의 만료기한을 초 단위로 설정한다. == SETEX, PX 밀리세컨드 단위)
 - GET : key 값에 해당하는 Value를 가져온다.
 - LIST : 여러 개의 데이터를 컨테이너 형식으로 관리하여 저장. (자료구조의 list와 유사하다) Key 와 Value의 관계가 1 : N 
   + TRIM : 지정된 범위를 제외한 모든 값들을 삭제  (key start stop)
   + LPUSH, RPUSH /POP : 왼, 오른쪽으로 데이터 삽입/삭제  + X 시, 기존에 리스트(Key)가 있을 경우에만 수행한다.
   + B(L/R)POP: 지정된 값이 없을 경우, 들어올 때까지 기다렸다가 삭제 (중요) + LPUSH 명령어를 합처서 쓸 수 있다.
   + LINSERT: 지정한 값 앞 뒤 사이에 추가 (linsert key before after pivot value  )
   + BLMOVE : 리스트 <-> 로 값을 옮기기 위함 
  - SET: List와 마찬가지로 1: N 관계, 중복 원소를 넣을 수 없다.
   + SADD: 원소를 추가한다.
   + SMEMEBER: 원소 조회 (순서 보장 X) + SORT ASC DESC 로 정렬해서 볼 수 있음
   + SREM: 원소 삭제 --> SRM: 패턴으로 삭제 가능. (자세한 건 Redis Gate 참고)
   + SCARD: 원소 갯수 조회
   + SUNION, SINTER : 합집합, 교집합
 
 ##### + CloudStructure
 
 + StackExchange.Redis를 기반으로 사용자가 좀 더 편하게 사용할 수 도록 제공하는 라이브러리 입니다.
 + 다양한 비동기 메소드를 지원합니다.
 + 깃허브 링크: https://github.com/xin9le/CloudStructures


 #### HTTP 와 REST API(POST)

 + HTTP : 이번 프로젝트에서 주로 사용할 통신 프로토콜 입니다. 웹에서 데이터를 통신하기 위한 주요 프로토콜로 다양한 요청 메소드(POST,GET,DEL) 등을 활용해
        요청을 보낼 수 있습니다. 하지만 이것은 기본적인 규칙에 해당하는 것으로 반드시 모든 요청 메소드를 의미에 맞게 사용할 필요는 없습니다. 게임 서버와 같이
        보안이 중요하게 요구되는 경우 모든 요청을 Post로 요청합니다.

+ POST : POST 메소드는 서버로 데이터를 전송하기 위한 메소드로 Body에 데이터를 담아 전송합니다. 이 점이 Get 보다 보안성이 좋은 점으로 앞서 언급했듯 게임 서버에서는 해당 명령을 주로 사용합니다.


#### 보안

---------------------------------------------------

해싱 : 해싱은 같은 값에 관해 알고리즘을 적용했을 때, 동일한 결과값이 나오기 때문에, 해커들이 주요 테이블을
만들어놨을 정도로 보안이 취약합니다. 이를 해결하기 위해 원본 데이터에 임의의 값을 추가해 원본이 아닌 다른 값으로
만든 후, 이를 여러 번 해싱하는 방법으로 진행하고 있습니다. 
이렇게 임의의 값을 추가하는 것을 솔팅(Salting)이라고 합니다.
#### PostMan

----------------------------------------------------

+ 추후 기본적으로 프로젝트를 생성하면 Swagger라는 UI를 제공합니다. 하지만 이는 정확한 요청 송수신 결과를 확인하기에 적절하지 않기 때문에 Postman이라는 대표적인 응용 프로그램을
  통해 다양한 형태의 데이터를 쉽게 송수신 할 수 있습니다.

#### ASP.NET Core

-----------------------------------------------------

+ Web서버를 구축하기 위한 프레임워크 입니다. .Netframework , 자말린 등 여러가지 프레임워크가 있으나 사실상 ASP.NET Core만 사용한다고 보면 됩니다.
+ 이런 wep api 프레임워크에 관한 역사를 알고싶으면 아래 링크의 영상을 시청하면 도움이 됩니다.
> + 영상 링크 : https://youtu.be/K6VCOFiVhmQ?si=SIwgoVGMWrSlbM4f


#### SQLKata
------------------------------------------------------

db와 연동하여 작업하는 쿼리문을 사용자가 좀 더 편하게 할 수 있도록 다양한 메소드를 지원하는 라이브러리
아래 링크를 참고하여 직접 공부하는 것이 도움이 된다.
주요 메소드 설명 링크 : https://sqlkata.com/docs/

##### + 구성 (Program.cs와 함께 참고)
------------------------------------------------------
 처음 프로젝트를 생성하면, 이를 한번도 접하지 못한 입장에서 이게 도대체 무슨 소리지 싶은 코드들이 잔뜩 존재합니다. 이때 저는 해당 영상을 시청하면서, 기초적인 내용을 습득할 수 있었습니다. 정말 좋은 영상이니
 시청해주면 좋을 것 같습니다.
 영상 링크: https://youtu.be/9zJn3a7L1uE?si=7xpqzJHpZAPrCTpU

+ 미들웨어
![MiddlewareImg](https://learn.microsoft.com/ko-kr/aspnet/core/fundamentals/middleware/index/_static/request-delegate-pipeline.png?view=aspnetcore-7.0)>

+ HTTP요청에 대해서 순차적인 검증 혹은 가공 과정을 거칠 수 있는 매개역할을 수행하는 파이프라인입니다.
+ 기본적으로 제공하는 것도 있으나, 사용자가 직접 정의해서 추가하는 것도 가능합니다.
+ Run / Map / Use를 통해 구성할 수 있습니다.
+ 게임 서버에서 요청 내용에 공통적으로 수행해야하는 기능(Ex 유저 토큰을 검증하는 기능)을 구현할 때 활용할 수 있습니다.

+ 프로젝트 생성 시, Program.cs에서 미들웨어 구성 요소를 위한 여러 가지 메소드들을 호출한다.
  이 중 주요한 것만 몇 가지 적으면
  1. UseRouting() : 라우팅은 들어오는 HTTP 요청을 일치
  2. UseEndPoint() : 요엔드포인트 실행을 미들웨어 파이프라인에 추가


+ 컨트롤러
  클라이언트로부터 전송되는 여러 가지 요청에 관한 처리 경로를 모아놓은 그룹이라고 보면 됩니다. 
  MappControllers() 메소드를 통해 우리가 작성한 컨트롤러를 등록할 수 있다.

+ 서비스 & 리포지토리 (중요) 

  * 서비스 : 들어온 요청에 대한 비즈니스 로직을 처리.
  * 리포지토리 : 직접적인 DB와의 연결 및 연결 해제 작업을 수행.
    해당 친구들은 Program.cs에서 선언된  AddTrasient, AddSingleton , AddScoped 메소드를 통해 의존성 주입이 진행됩니다.
    1. AddTransient : 서비스를 요청할 때마다 새로운 인스턴스를 생성. --> MySqlConnector와 같은 재사용이 불가능한 멤버들을 사용해야할 때 이를 사용합니다. 가장 많이 사용하는 메소드 입니다.
    2. AddScope: 하나의 스코프에서 동일한 인스턴스를 생성, 다른 요청이 들어오면 새로운 인스턴스가 생성됨
    3. AddSingleton: 하나의 인스턴스를 생성하고 공유. --> Redis와 같은 싱글스레드로 동작하는 Db와의 연결을 다루는 서비스에 관해 이 메소드를 사용하면 좋습니다.
  이 둘을 구별하는 이유는 두 요소를 구분함으로써,   Db연결에 문제가 생길 경우, 리포지토리에 해당하는 내용만  요청 처리에 문제가 있으면 서비스에 해당하는 내용만 수정하면 되기 때문에 프로젝트 유지 보수에 용이합니다. 
 + Configuration
  * 주로 Db 연결 주소나, 로그를 출력할 때 사용할 로거를 셋팅하는 등 webapi 프로젝트를 빌드하기 전 다양한 셋팅을 조정할 수 있다. appSettiong.json 파일에서 작성할 수 있다. 

#### 프로젝트 내용

---------------------------------------

  ##### + 구현 목표
  1. 로그인 / 회원 가입
  2. 게임 데이터 로드
  3. 우편함 

  
  기본적인 프로젝트 진행 시나리오는 클라이언트가 로그인 or 회원 가입 --> 캐릭터 선택 (최대 3개 까지 생성 가능)
  --> 선택한 캐릭터에 대한 우편함 및 인벤토리 정보를 로드 --> 이후 인벤토리나, 우편함의 데이터를 갱신하는 작업 수행합니다.

  Logger는 교육 당시, 추천해주신 ZLogger를 사용했습니다.
  
 ##### + 구현 시, 신경 써야할 사항
 ------------------------------------------
  1. 하나의 클라이언트 요청 당, 한번 만 처리하도록 하기.(동시에 들어오는 요청을 한 번에 처리하지 않는다.   안정성 때문)
  2. 변수명은 길어도 되니까 디테일하게 적고, 항상 부여한 명칭에 관한 합리적인 이유가 있어야한다. 
  3. 계정 정보는 날 것으로 저장하지 않기. (저의 경우 pbkdf2 알고리즘을 활용해 패스워드를 해싱하고 DB에 저장했습니다.)
  4. 각 라이브러리에 존재하는 메소드에 관한 설명을 잘 읽어보기. f12를 습관화하기.
  5. 프로젝트 시작하기 전에 데이터베이스 스키마 부터 짜고 시작하기. --> 방황하는 것을 줄일 수 있습니다.
  6. Response Model 정의할 때, 클래스 객체 대신 클래스 객체를 json String으로 파싱해서 수행하기. --> 이렇게 안하면 포스트맨에서 출력이 안됩니다.
  7. asp.net core를 구성하는 각 항목에 대한 개념적인 이해가 많이 필요합니다.

 ##### + 작성한 데이터베이스 스키마
 ------------------------------------------
  ![Db Schema](DatabasSchema.PNG)


 ##### + 각 폴더별 설명
  Service : 요청에 관한 로직을 처리하는 클래스들. (특정 데이터를 다룬다는 의미에서 HandleOOO으로 명명)
  ServiceInterface : Service에 대한 인터페이스.
  ResReqModel : 요청, 응답에 관한 데이터 모델들.
  DataModel : 데이터베이스에 있는 각 테이블들에 관한 스키마를 딴 클래스들.
  Security :   보안과 관련된 클래스. 비밀번호를 해싱하는 PasswordHasher가 있음.
  Repository : MySql 과 Redis 에 연겷하는 작업을 수행하는 클래스 및 인터페이스
  Controllers: 내가 작성한 컨트롤러들의 모음

  ##### 주요 서비스 설명
---------------------------------------
   1. HandleInventoryData : 인벤토리와 관련된 비즈니스 로직을 처리합니다. 로그인 시,  기본적으로 인벤토리 정보를 로드하며,  아이템을 얻고, 사용/버리기 요청에 관해, 개수의 증감, 삭제 등의 간단한 동작을 수행합니다.
   2. HandleMailData : 메일과 관련된 비즈니스 로직을 처리합니다. 로그인 시, 기본적으로 우편함 정보를 로드하며, 메일을 수신할 시, 인벤토리의 내용 갱신 및 메일 삭제하는 간단한 동작을 수행합니다.
   3. HandleAccount :  유저가 회원가입 시, 해당 정보를 해싱하여 데이터베이스에 갱신하거나, 로그인 시, 계정을 검증하는 간단한 동작을 수행합니다. 로그인 성공 시, 임시 토큰을 생성해 유저에게 부여하고 Redis에 등록합니다.

 ##### 핵심 기능 설명
 -------------------------------------
  1. MiddlerwareTokenVerifier : 해당 미들웨어는 이번 프로젝트에서 가장 중요한 역할을 수행하며 핵심 기능은 다음과 같습니다.
  
    *  유저의 다양한 요청에 대해서, 해당 유저의 정상적인 요청이 맞는지 검증할 필요가 있습니다. 이를 위해 모든 요청에 관해 AuthToken이라는 멤버를 부여하고, 해당 미들웨어가 Request의 Body에 있는 해당 데이터를 읽어와 검증을 수행합니다. 

     핵심 코드는 다음과 같습니다.
  
  ```C#
        // 회원 가입을 수행하거나, 로그인 요청을 했을 당시에는 처리하지 않음
            if (context.Request.Path.StartsWithSegments("/Register") || context.Request.Path.StartsWithSegments("/Login"))
            {
                _logger.LogInformation($"[MiddleWareTokenVerifier] Result: None Check Path");
            }
            else
            {
              
                var bodyContents = await GetBodyStringFromRequest(context.Request);
                var token = bodyContents["AuthToken"].ToString();
                var playerId = bodyContents["PlayerId"].Value<int>();
                var requestType = bodyContents["RequestType"].Value<int>();
                _logger.LogInformation($"[MiddleWareTokenVerifier] Token : {token} | PlayerId: {playerId} | RequestType : {requestType}");

                var checkResult = await _redisDb.CheckAuthTokenExist(token);
                var registerRequestResult = await _redisDb.StartPlayerRequest(playerId, requestType);
                if (!checkResult || !registerRequestResult)
                {
                    _redisDb.FinishPlayerRequest(playerId);
                    throw new Exception();
                }
                await _next(context);
            }

  ```

  2. PasswordHasher : 유저가 회원 가입을 진행할 때, 데이터베이스에는 Salt 처리와 반복 해싱 작업한 비밀번호를 저장합니다. Pbkdf2 알고리즘을 활용했습니다. 핵심 코드는 다음과 같습니다.

```C#
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
```
  3. RedisDb : 이번 프로젝트에서 가장 핵심이 되는 역할을 수행하며 기능은 다음과 같습니다.

  
  * 유저가 로그인 시, 생성한 임의의 토큰을 등록하고, 미들웨어에서 검증을 수행할 때, 필요한 메소드를 제공합니다. 핵심 코드는 다음과 같습니다.

```C#
 public async Task<CSCommon.ErrorCode> RegisterAuthToken(string Email, string Token)
    {
        try
        {
           
            var redisQuery = new RedisString<string>(_redisConn, Token, null);
            var result = await redisQuery.SetAsync(Email); // 

            if (!result) return CSCommon.ErrorCode.RedisErrorFailToAddToken;
        }
        catch
        { 
            _logger.ZLogError("Something Error Occur At AddAuthToken. Plz Check This Code");
            return ErrorCode.RedisErrorException;
            
        }

        return CSCommon.ErrorCode.ErrorNone;
    }
```
 * 클라이언트 요청을 한번에 한번 씩만 수행하도록 미들웨어에서 요청이 들어왔을 때, Redis에 요청 타입에 관한    key-value를 등록합니다. (PlayerId - Request Type) 그리고 요청이 끝났을 시, 해당 정보를 Redis에서 제거합니다.
   여기서 핵심은 When.No.Exist 플래그를 활용해, Redis에 해당 요청 키가 남아있을 경우(요청이 아직 안끝난 경우),
   해당 요청을 처리하지 않습니다. 핵심 코드는 다음과 같습니다

```C#
 public async Task<bool> StartPlayerRequest(int PlayerId, int Type)
    {
        try
        {
            var redisQuery = new RedisString<int>(_redisGameConn, PlayerId.ToString(),TimeSpan.FromSeconds(2));
            var result = await redisQuery.SetAsync(Type, when:When.NotExists);
            if (!result) throw new Exception();
        }
        catch
        {
            _logger.ZLogError($"[RedisDb.RegisterPlayerRequest] ErrorCode : {CSCommon.ErrorCode.RedisErrorException}");
            return false;
        } 

        return true;
    }

    public async void FinishPlayerRequest(int PlayerId)
    {
        try
        {
            var redisQuery = new RedisString<CSCommon.RequestType>(_redisGameConn, PlayerId.ToString(), null);
            var result = await redisQuery.DeleteAsync();
            if (!result) throw new Exception();
        }
        catch
        {
            _logger.ZLogError($"[RedisDb.FinishPlayerRequest] ErrorCode : {CSCommon.ErrorCode.RedisErrorException}");
        }

    }

```