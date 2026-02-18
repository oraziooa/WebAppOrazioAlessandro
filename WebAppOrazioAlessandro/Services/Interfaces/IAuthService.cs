using WebAppOrazioAlessandro.DTOs.Auth;

namespace WebAppOrazioAlessandro.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);
        Task<bool> RegisterAsync(RegisterDto dto);
        Task<string?> GeneratePasswordResetTokenAsync(string email);
        Task<bool> ResetPasswordAsync(ResetPasswordDto model);
        Task<bool> AssignRoleAsync(string userEmail, string role);
    }
}
