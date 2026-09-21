using Common.Log;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ServiceLog _log;
        private readonly string _jwtKey;

        public AuthController(AppDbContext context, ServiceLog log, IConfiguration configuration)
        {
            _context = context;
            _log = log;
            _jwtKey = configuration["Jwt:Key"];
        }

        // ======================================================
        // LOGIN
        // ======================================================
        [HttpPost("login")]
        public IActionResult Login([FromBody] WebApplication1.Models.LoginRequest request)
        {
            _log.Logger.Information("Tentativa de login para usuário: {Username}", request.Username);

            var user = _context.Usuarios
                .FirstOrDefault(u => u.Username == request.Username);

            if (user == null)
                return Unauthorized(new { message = "Usuário inválido." });

            if (user.IsBlocked)
                return Unauthorized(new { message = "Sua conta foi bloqueada após 3 tentativas incorretas." });

            if (user.PasswordHash != request.Password)
            {
                user.FailedAttempts++;

                if (user.FailedAttempts >= 3)
                    user.IsBlocked = true;

                _context.SaveChanges();
                return Unauthorized(new { message = "Senha incorreta." });
            }

            user.FailedAttempts = 0;
            user.IsBlocked = false;
            _context.SaveChanges();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                message = "Login realizado com sucesso.",
                token = tokenString
            });
        }

        // ======================================================
        // CRIAR CONTA
        // ======================================================
        [HttpPost("create-account")]
        public IActionResult CreateAccount([FromBody] CreateAccountRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) ||
                string.IsNullOrEmpty(request.Password) ||
                string.IsNullOrEmpty(request.Email))
            {
                return BadRequest(new { message = "Usuário, email e senha são obrigatórios." });
            }

            var existingUser = _context.Usuarios
                .FirstOrDefault(u => u.Username == request.Username || u.Email == request.Email);

            if (existingUser != null)
                return BadRequest(new { message = "Usuário ou email já estão em uso." });

            if (!IsPasswordStrong(request.Password))
                return BadRequest(new
                {
                    message = "A senha deve ter pelo menos 5 caracteres, incluindo letra, número e caractere especial."
                });

            var newUser = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = request.Password,
                FailedAttempts = 0,
                IsBlocked = false
            };

            _context.Usuarios.Add(newUser);
            _context.SaveChanges();

            _log.Logger.Information("Conta criada com sucesso para usuário: {Username}", request.Username);

            return Ok(new { message = "Conta criada com sucesso." });
        }

        // ======================================================
        // ESQUECI MINHA SENHA
        // ======================================================
        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] WebApplication1.Models.ForgotPasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
                return BadRequest(new { message = "Email é obrigatório." });

            var user = _context.Usuarios
                .FirstOrDefault(u => u.Email == request.Email);

            if (user == null)
                return NotFound(new { message = "Email não encontrado." });

            var newPassword = GenerateRandomPassword();

            user.PasswordHash = newPassword;
            user.FailedAttempts = 0;
            user.IsBlocked = false;

            _context.SaveChanges();

            return Ok(new
            {
                message = "Nova senha gerada com sucesso.",
                newPassword = newPassword // REMOVE ISSO EM PRODUÇÃO
            });
        }

        // ======================================================
        // TROCAR SENHA (TOKEN)
        // ======================================================
        [Authorize]
        [HttpPost("change-password")]
        public IActionResult ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var username = User.Identity?.Name;

            if (string.IsNullOrEmpty(username))
                return Unauthorized(new { message = "Usuário não autenticado." });

            var user = _context.Usuarios
                .FirstOrDefault(u => u.Username == username);

            if (user == null)
                return Unauthorized(new { message = "Usuário não encontrado." });

            if (!IsPasswordStrong(request.NewPassword))
                return BadRequest(new
                {
                    message = "Por favor, crie uma senha com pelo menos uma letra, um número, um caractere especial e no mínimo 5 caracteres."
                });

            user.PasswordHash = request.NewPassword;
            user.FailedAttempts = 0;
            user.IsBlocked = false;

            _context.SaveChanges();

            return Ok(new { message = "Senha alterada com sucesso." });
        }

        // ======================================================
        // GERAR SENHA AUTOMÁTICA
        // ======================================================
        private string GenerateRandomPassword()
        {
            return "Nova@" + new Random().Next(1000, 9999);
        }

        // ======================================================
        // VALIDAÇÃO SENHA
        // ======================================================
        private bool IsPasswordStrong(string password)
        {
            if (string.IsNullOrEmpty(password))
                return false;

            if (password.Length < 5)
                return false;

            bool hasLetter = password.Any(char.IsLetter);
            bool hasNumber = password.Any(char.IsDigit);
            bool hasSpecialChar = password.Any(ch => !char.IsLetterOrDigit(ch));

            return hasLetter && hasNumber && hasSpecialChar;
        }
    }
}