using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAppOrazioAlessandro.DTOs.Auth;
using WebAppOrazioAlessandro.Entities;
using WebAppOrazioAlessandro.Services.Interfaces;

namespace WebAppOrazioAlessandro.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<bool> RegisterAsync(RegisterDto dto)
        {
            // Normalizzo email
            var emailNormalized = dto.Email.Trim().ToLower();

            var existingUser = await _userManager.FindByEmailAsync(emailNormalized);
            if (existingUser != null)
                throw new InvalidOperationException("Esiste già un utente con questa email.");

            var user = new ApplicationUser
            {
                UserName = dto.Username.Trim(),
                Email = emailNormalized
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new ArgumentException(string.Join("; ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, "Admin");
            return true;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            var emailNormalized = dto.Email.Trim().ToLower();
            var user = await _userManager.FindByEmailAsync(emailNormalized);
            if (user == null)
                return null;

            var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!passwordValid)
                return null;

            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, roles);

            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email!,
                Role = roles.FirstOrDefault() ?? ""
            };
        }

        private string GenerateJwtToken(ApplicationUser user, IList<string> roles)
        {
            var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Email, user.Email!)
                };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(jwtKey))
                throw new InvalidOperationException("JWT Key non configurata!");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            int hours = int.TryParse(_configuration["Jwt:ExpirationHours"], out var h) ? h : 3;

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(hours),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public async Task<string?> GeneratePasswordResetTokenAsync(string email)
        {
            var emailNormalized = email.Trim().ToLower();
            var user = await _userManager.FindByEmailAsync(emailNormalized);
            if (user == null)
                return null; 

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return token;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto model)
        {
            var emailNormalized = model.Email.Trim().ToLower();
            var user = await _userManager.FindByEmailAsync(emailNormalized);
            if (user == null)
                return false;

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            return result.Succeeded;
        }

        public Task<bool> LogoutAsync() => Task.FromResult(true); //  client gestisce il logout
        public async Task<bool> AssignRoleAsync(string userEmail, string role)
        {
            if (string.IsNullOrWhiteSpace(userEmail) || string.IsNullOrWhiteSpace(role))
                throw new ArgumentException("Email e ruolo obbligatori");

            // Lista  ruoli validi
            var validRoles = new[] { "Admin", "Supervisor", "User" };
            if (!validRoles.Contains(role))
                throw new InvalidOperationException($"Il ruolo '{role}' non è valido. Valori consentiti: {string.Join(", ", validRoles)}");

            // Trovo l'utente
            var user = await _userManager.FindByEmailAsync(userEmail.Trim().ToLower());
            if (user == null)
                throw new InvalidOperationException("Utente non trovato");

            // Rimuovo eventuali ruoli precedenti 
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            // Assegno il nuovo ruolo
            var result = await _userManager.AddToRoleAsync(user, role);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));

            return true;
        }
    }
}
