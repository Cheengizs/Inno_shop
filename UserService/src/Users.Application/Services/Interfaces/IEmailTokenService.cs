namespace Users.Application.Services.Interfaces;

public interface IEmailTokenService
{
    string GenerateEmailConfirmationToken(int userId);
    int? ValidateEmailConfirmationToken(string token);
}