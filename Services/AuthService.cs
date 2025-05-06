// Services/AuthService.cs
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; // Per accedere a appsettings.json
using Microsoft.IdentityModel.Tokens;
using WeatherApp.Data;
using WeatherApp.Dtos.Auth;
using WeatherApp.Models;

namespace WeatherApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<AuthResponseDto>> Login(string username, string password)
        {
            var response = new ServiceResponse<AuthResponseDto>();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found.";
                return response;
            }
            // Verifica la password (BCrypt.Verify)
            else if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                response.Success = false;
                response.Message = "Incorrect password.";
                return response;
            }
            else
            {
                // Password corretta, crea il token
                response.Data = new AuthResponseDto
                {
                    Username = user.Username,
                    Token = CreateToken(user)
                };
                response.Message = "Login successful!";
            }
            return response;
        }

        public async Task<ServiceResponse<int>> Register(User user, string password)
        {
            var response = new ServiceResponse<int>();
            if (await UserExists(user.Username))
            {
                response.Success = false;
                response.Message = "Username already exists.";
                return response;
            }

            // Crea l'hash della password
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            user.PasswordHash = passwordHash;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            response.Data = user.Id; // Restituisce l'ID del nuovo utente
            response.Message = "User registered successfully!";
            return response;
        }

        private async Task<bool> UserExists(string username)
        {
            if (await _context.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower()))
            {
                return true;
            }
            return false;
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
                // Puoi aggiungere altri claim se necessario (es. ruoli)
            };

            // Prendi la chiave segreta da appsettings.json
            // Assicurati che sia abbastanza lunga e complessa!
            var appSettingsToken = _configuration.GetSection("AppSettings:Token").Value;
            if (string.IsNullOrEmpty(appSettingsToken))
                throw new Exception("AppSettings:Token is not configured in appsettings.json");

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettingsToken));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1), // Token valido per 1 giorno
                SigningCredentials = creds
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token); // Il token JWT come stringa
        }
    }
}
