using InventoryApp.Application.Dto;
using InventoryApp.Application.Interfaces;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using InventoryApp.Application.Hash;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AutoMapper;
using InventoryApp.Models.Entity;

namespace InventoryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IEmployeeRepository repository;
        private readonly IMapper mapper;
        private readonly Application.Hash.PasswordHasher passwordHasher;

        public AuthController(IEmployeeRepository repository, Application.Hash.PasswordHasher passwordHasher)
        {
            this.repository = repository;
            this.passwordHasher = passwordHasher;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto, CancellationToken cancellationToken)
        {
            var employee = await repository.GetByMailAsync(loginRequestDto.Email, cancellationToken);
            if (employee == null)
            {
                return Unauthorized("Geçersiz kullanıcı adı veya şifre.");
            }

            bool verified = passwordHasher.VerifyPassword(employee.Password, employee.Salt, loginRequestDto.Password);

            if (verified == false)
            {
                return Unauthorized("Geçersiz kullanıcı adı veya şifre.");
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Email, employee.Email),
                new(ClaimTypes.NameIdentifier, employee.Id.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, "EmployeeCookie");
            await HttpContext.SignInAsync("EmployeeCookie", new ClaimsPrincipal(claimsIdentity));

            return Ok("Başarıyla giriş yaptınız.");
        }

        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] PasswordRequestDto passwordRequestDto, CancellationToken cancellationToken)
        {
            var employee = await repository.GetByMailAsync(passwordRequestDto.Email, cancellationToken);
            if (employee == null)
            {
                return Unauthorized("Geçersiz kullanıcı adı veya şifre.");
            }

            var passwordHasher = new Application.Hash.PasswordHasher();
            bool verified = passwordHasher.VerifyPassword(employee.Password, employee.Salt, passwordRequestDto.Password);
            if (verified == false)
            {
                return Unauthorized("Geçersiz kullanıcı adı veya şifre.");
            }

            if (passwordRequestDto.NewPassword != passwordRequestDto.NewPassword2)
            {
                return BadRequest("New passwords don't match");
            }

            var (hashedPassword, salt) = passwordHasher.HashPassword(passwordRequestDto.NewPassword);

            employee.Password = hashedPassword;
            employee.Salt = salt;

            await repository.UpdateAsync(employee, cancellationToken);

            return Ok("Başarıyla şifrenizi değiştirdiniz.");
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPasswordCode([FromBody] ForgotPasswordRequestDto forgotPasswordRequestdto, CancellationToken cancellationToken)
        {
            var employee = await repository.GetByMailAsync(forgotPasswordRequestdto.Email, cancellationToken);
            if (employee == null)
            {
                return Unauthorized("Geçersiz kullanıcı adı veya şifre.");
            }

            var randomCode = repository.GenerateRandomString(6);
            employee.ForgetCode = randomCode;
            await repository.UpdateAsync(employee, cancellationToken);

            return Ok($"Hesabınızı kurtarma şifreniz = {randomCode}");
        }

        [HttpPut("ForgotPasswordChange")]
        public async Task<IActionResult> ForgotPasswordChange([FromBody] ForgotPasswordResponseDto forgotPasswordResponseDto, CancellationToken cancellationToken)
        {
            var employee = await repository.GetByMailAsync(forgotPasswordResponseDto.Email, cancellationToken);
            if (employee == null)
            {
                return Unauthorized("Geçersiz mail");
            }

            var code = await repository.GetByForgotCodeAsync(forgotPasswordResponseDto.ForgotCode, cancellationToken);
            if (code == null)
            {
                return Unauthorized("Kod yanlış");
            }

            if (forgotPasswordResponseDto.NewPassword != forgotPasswordResponseDto.NewPassword2)
            {
                return BadRequest("New passwords don't match");
            }

            var (hashedPassword, salt) = passwordHasher.HashPassword(forgotPasswordResponseDto.NewPassword);

            employee.Password = hashedPassword;
            employee.Salt = salt;

            await repository.UpdateAsync(employee, cancellationToken);

            return Ok("Başarıyla şifrenizi değiştirdiniz.");
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("EmployeeCookie");
            return Ok("Başarıyla çıkış yaptınız.");
        }
    }
}
