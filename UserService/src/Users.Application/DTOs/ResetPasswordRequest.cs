namespace Users.Application.DTOs;

public record ResetPasswordRequest(string Token, string NewPassword, string ConfirmNewPassword);