namespace TaskManager.Application.Dtos.Auth;   

public class LoginResponseDto
{
    public string AccessToken { get; set; } = string.Empty;

    public DateTime Expiration { get; set; } 

    public string RefreshToken { get; set; } = string.Empty;

}