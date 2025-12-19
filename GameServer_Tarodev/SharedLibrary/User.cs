namespace SharedLibrary;

public class User
{
    public int Id { get; set; }              // PK로 자동 인식
    public string UserName { get; set; }     // 유저 이름
    public string PasswordHash { get; set; } // 비밀번호 해시
    public string Salt { get; set; }         // 솔트 값
}
