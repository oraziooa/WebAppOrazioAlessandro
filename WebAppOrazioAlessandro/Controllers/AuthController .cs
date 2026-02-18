using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAppOrazioAlessandro.DTOs.Auth;
using WebAppOrazioAlessandro.Services.Interfaces;

namespace WebAppOrazioAlessandro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Email e password obbligatorie");

            try
            {
                await _authService.RegisterAsync(dto);

                // Login automatico
                var tokenResult = await _authService.LoginAsync(new LoginDto
                {
                    Email = dto.Email,
                    Password = dto.Password
                });

                return Ok(tokenResult);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                return StatusCode(500, "Errore interno durante la registrazione");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Email e password obbligatorie");

            var result = await _authService.LoginAsync(dto);
            if (result == null)
                return Unauthorized("Email o password non corretti");

            return Ok(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            var token = await _authService.GeneratePasswordResetTokenAsync(dto.Email);
            return Ok(new { Message = "Se l'email è registrata, riceverai istruzioni", ResetToken = token });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NewPassword) || string.IsNullOrWhiteSpace(dto.Token))
                return BadRequest("Token e nuova password obbligatori");

            var success = await _authService.ResetPasswordAsync(dto);
            if (!success)
                return BadRequest("Reset fallito");

            return Ok("Password aggiornata");
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout() => Ok("Logout effettuato");

        [Authorize(Roles = "Admin")]
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.UserEmail) || string.IsNullOrWhiteSpace(dto.Role))
                return BadRequest("Email e ruolo obbligatori");

            var success = await _authService.AssignRoleAsync(dto.UserEmail, dto.Role);
            if (!success)
                return NotFound("Utente non trovato o ruolo non valido");

            return Ok($"Ruolo {dto.Role} assegnato a {dto.UserEmail}");
        }
    }
}
