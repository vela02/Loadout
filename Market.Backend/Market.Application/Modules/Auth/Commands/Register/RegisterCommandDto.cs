namespace Market.Application.Modules.Auth.Commands.Register;

public class RegisterCommandDto
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string Token { get; set; } = null!;
}