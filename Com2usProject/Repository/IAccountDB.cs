using CSCommon;

namespace Com2usProject.Repository;


// Interface Vs Class
// AccountDB는 기본적으로 IDisposable 인터페이스를 상속받는다. 왜? 비관리 리소스를 안전하게  삭제 처리할 수 있도록.
// 1. 의존성 주입에 관해 공부하면서 이를 interface로 선언할 지, 추상 클래스로 선언할 지 고민.
// 2. Interface는 추상 클래스보다 비교적 속도가 느리다.
// 3. 하지만 추상 클래스와 다르게 다중 상속이 가능하다. Ex) 현재는 Mysql을 통해 계정을 관리하지만 나중에는 다른 DBMS를 사용할 수 있기 때문이다.
public interface IAccountDb : IDisposable // 다른 DB를 사용한다고 하더라도 계정과 관련해서는 생성 및 로그인(검증), 데이터 해싱 작업은 공통적으로
                                          // 수행하기 때문에 해당 두 가지 기능은 필수적으로 들어간다.
{
    Task<CSCommon.ErrorCode> RegisterAccount(String email, String pw); 
    Task<CSCommon.ErrorCode> VerifyAccount(String email, String pw);

 
}
