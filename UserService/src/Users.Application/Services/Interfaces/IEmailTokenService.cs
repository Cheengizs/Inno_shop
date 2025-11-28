namespace Users.Application.Services.Interfaces;

public interface IEmailTokenService
{
    string GenerateEmailConfirmationToken(int userId);
    bool ValidateEmailConfirmationToken(string token, out int userId);
}