using Microsoft.AspNetCore.Mvc;
using Parsel360.API.Data;
using Parsel360.API.Helpers;
using Parsel360.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Parsel360.API.Controllers
{
	[ApiController]
	[Route("api/auth")]
	public class AuthController : ControllerBase
	{
		private readonly AppDbContext _context;
		private readonly JwtService _jwtService;

		public AuthController(AppDbContext context, JwtService jwtService)
		{
			_context = context;
			_jwtService = jwtService;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
		{
			if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
				return BadRequest("Kullanıcı adı zaten mevcut.");

			var user = new User
			{
				Username = dto.Username,
				Email = dto.Email,
				PasswordHash = HashPassword(dto.Password),
				Role = "User"
			};

			_context.Users.Add(user);
			await _context.SaveChangesAsync();

			return Ok("Kayıt başarılı.");
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
			if (user == null || !VerifyPassword(dto.Password, user.PasswordHash))
				return Unauthorized("Kullanıcı adı veya şifre yanlış.");

			var token = _jwtService.GenerateToken(user.Username, user.Role);
			return Ok(new { token });
		}

		
		private string HashPassword(string password)
		{
			using var sha256 = SHA256.Create();
			var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
			return Convert.ToBase64String(bytes);
		}

		private bool VerifyPassword(string password, string hashed)
		{
			return HashPassword(password) == hashed;
		}
	}

	public class UserRegisterDto
	{
		public string Username { get; set; } = null!;
		public string Email { get; set; } = null!;
		public string Password { get; set; } = null!;
	}

	public class UserLoginDto
	{
		public string Username { get; set; } = null!;
		public string Password { get; set; } = null!;
	}
}
