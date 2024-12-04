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
using InventoryApp.Application.Utility;
using Microsoft.AspNetCore.Antiforgery;

namespace InventoryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IEmployeeRepository _repository;
        private readonly Application.Hash.PasswordHasher _passwordHasher;
        private readonly IAntiforgery _antiforgery;

        public AuthController(IEmployeeRepository repository, Application.Hash.PasswordHasher passwordHasher, IAntiforgery antiforgery)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
            _antiforgery = antiforgery;
        }

        [HttpGet("get-antiforgery-token")]
        public IActionResult GetAntiforgeryToken()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken!,
                new CookieOptions {
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto, CancellationToken cancellationToken)
        {
            var employee = await _repository.GetByMailAsync(loginRequestDto.Email, cancellationToken);
            if (employee == null)
            {
                return Unauthorized(new { message = "Geçersiz kullanıcı adı veya şifre." });
            }

            bool verified = _passwordHasher.VerifyPassword(employee.Password, employee.Salt, loginRequestDto.Password);

            if (verified == false)
            {
                return Unauthorized(new { message = "Geçersiz kullanıcı adı veya şifre." });
            }

            if (employee.IsVerified == false)
            {
                return NotFound(new { message = "Mailiniz doğrulanmamış. Mailinizi doğrulayıp tekrar deneyin." });
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Email, employee.Email),
            };

            var claimsIdentity = new ClaimsIdentity(claims, "EmployeeCookie");
            await HttpContext.SignInAsync("EmployeeCookie", new ClaimsPrincipal(claimsIdentity));

            var antiforgery = HttpContext.RequestServices.GetRequiredService<IAntiforgery>();
            var tokens = antiforgery.GetAndStoreTokens(HttpContext);
            HttpContext.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken!,
                new CookieOptions { HttpOnly = false });

            return Ok(new { message = "Başarıyla giriş yaptınız." });
        }

        [HttpPut("MailVerification")]
        public async Task<IActionResult> MailVerification([FromBody] MailVerificationRequestDto mailVerificationRequestDto, CancellationToken cancellationToken)
        {
            var employee = await _repository.GetByMailAsync(mailVerificationRequestDto.Email, cancellationToken);
            if (employee == null)
            {
                return Unauthorized("Geçersiz mail veya kod hatalı.");
            }

            if(employee.MailVerificationCode != mailVerificationRequestDto.VerificationCode)
            {
                return Unauthorized("Geçersiz mail veya kod hatalı.");
            }

            employee.IsVerified = true;
            await _repository.UpdateAsync(employee, cancellationToken);

            return Ok("Başarıyla verifike oldunuz.");
        }

        [Authorize]
        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] PasswordRequestDto passwordRequestDto, CancellationToken cancellationToken)
        {
            var currentUserEmail = User.FindFirstValue(ClaimTypes.Email);
            if (currentUserEmail == null)
            {
                return Unauthorized("sistemsel hata, lütfen daha sonra tekrar deneyin.");
            }
            var employee = await _repository.GetByMailAsync(currentUserEmail, cancellationToken);
            if (employee == null)
            {
                return Unauthorized("Geçersiz kullanıcı adı veya şifre.");
            }

            bool verified = _passwordHasher.VerifyPassword(employee.Password, employee.Salt, passwordRequestDto.Password);
            if (verified == false)
            {
                return Unauthorized("Geçersiz kullanıcı adı veya şifre.");
            }

            var (hashedPassword, salt) = _passwordHasher.HashPassword(passwordRequestDto.NewPassword);

            employee.Password = hashedPassword;
            employee.Salt = salt;

            await _repository.UpdateAsync(employee, cancellationToken);

            return Ok("Başarıyla şifrenizi değiştirdiniz.");
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPasswordCode([FromBody] ForgotPasswordRequestDto forgotPasswordRequestdto, CancellationToken cancellationToken)
        {
            var employee = await _repository.GetByMailAsync(forgotPasswordRequestdto.Email, cancellationToken);
            if (employee == null)
            {
                return Unauthorized("Mailiniz doğruysa şifre kurtarma kodunuz mailinize gönderilmiştir.");
            }

            var randomCode = _repository.GenerateRandomString(6);

            employee.ForgotCode = randomCode;
            await _repository.UpdateAsync(employee, cancellationToken);

            return Ok($"Hesabınızı kurtarma şifreniz = {randomCode}");
        }

        [HttpPut("ForgotPasswordChange")]
        public async Task<IActionResult> ForgotPasswordChange([FromBody] ForgotPasswordResponseDto forgotPasswordResponseDto, CancellationToken cancellationToken)
        {
            var employee = await _repository.GetByMailAsync(forgotPasswordResponseDto.Email, cancellationToken);
            if (employee.ForgotCode != forgotPasswordResponseDto.ForgotCode)
            {
                return Unauthorized("Kod yanlış veya geçersiz.");
            }

            if (employee == null)
            {
                return Unauthorized("Geçersiz mail");
            }

            var (hashedPassword, salt) = _passwordHasher.HashPassword(forgotPasswordResponseDto.NewPassword);

            employee.Password = hashedPassword;
            employee.Salt = salt;

            employee.ForgotCode = null;

            await _repository.UpdateAsync(employee, cancellationToken);

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
