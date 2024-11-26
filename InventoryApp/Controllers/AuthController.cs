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

namespace InventoryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IEmployeeRepository repository;
        private readonly Application.Hash.PasswordHasher passwordHasher;

        public AuthController(IEmployeeRepository repository, Application.Hash.PasswordHasher passwordHasher)
        {
            this.repository = repository;
            this.passwordHasher = passwordHasher;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto, CancellationToken cancellationToken)
        {
            loginRequestDto = AntiXssUtility.EncodeDto(loginRequestDto);

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

            if (employee.IsVerified == false)
            {
                return NotFound("Mailiniz doğrulanmamış. Mailinizi doğrulayıp tekrar deneyin.");
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Email, employee.Email),
            };

            var claimsIdentity = new ClaimsIdentity(claims, "EmployeeCookie");
            await HttpContext.SignInAsync("EmployeeCookie", new ClaimsPrincipal(claimsIdentity));

            return Ok("Başarıyla giriş yaptınız.");
        }

        [HttpPut("MailVerification")]
        public async Task<IActionResult> MailVerification([FromBody] MailVerificationRequestDto mailVerificationRequestDto, CancellationToken cancellationToken)
        {
            mailVerificationRequestDto = AntiXssUtility.EncodeDto(mailVerificationRequestDto);

            var employee = await repository.GetByMailAsync(mailVerificationRequestDto.Email, cancellationToken);
            if (employee == null)
            {
                return Unauthorized("Geçersiz mail veya kod hatalı.");
            }

            if(employee.MailVerificationCode != mailVerificationRequestDto.VerificationCode)
            {
                return Unauthorized("Geçersiz mail veya kod hatalı.");
            }

            employee.IsVerified = true;
            await repository.UpdateAsync(employee, cancellationToken);

            return Ok("Başarıyla verifike oldunuz.");
        }

        [Authorize]
        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] PasswordRequestDto passwordRequestDto, CancellationToken cancellationToken)
        {
            passwordRequestDto = AntiXssUtility.EncodeDto(passwordRequestDto);

            var currentUserEmail = User.FindFirstValue(ClaimTypes.Email);
            if (currentUserEmail == null)
            {
                return Unauthorized("sistemsel hata, lütfen daha sonra tekrar deneyin.");
            }
            var employee = await repository.GetByMailAsync(currentUserEmail, cancellationToken);
            if (employee == null)
            {
                return Unauthorized("Geçersiz kullanıcı adı veya şifre.");
            }

            bool verified = passwordHasher.VerifyPassword(employee.Password, employee.Salt, passwordRequestDto.Password);
            if (verified == false)
            {
                return Unauthorized("Geçersiz kullanıcı adı veya şifre.");
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
            forgotPasswordRequestdto = AntiXssUtility.EncodeDto(forgotPasswordRequestdto);

            var employee = await repository.GetByMailAsync(forgotPasswordRequestdto.Email, cancellationToken);
            if (employee == null)
            {
                return Unauthorized("Mailiniz doğruysa şifre kurtarma kodunuz mailinize gönderilmiştir.");
            }

            var randomCode = repository.GenerateRandomString(6);
            //var code2 = repository.GenerateRandomString(6);

            //if (randomCode == code2)
            //{
            //    return BadRequest("aynı oluşturmuyor.");
            //}

            employee.ForgotCode = randomCode;
            await repository.UpdateAsync(employee, cancellationToken);

            return Ok($"Hesabınızı kurtarma şifreniz = {randomCode}");
        }

        [HttpPut("ForgotPasswordChange")]
        public async Task<IActionResult> ForgotPasswordChange([FromBody] ForgotPasswordResponseDto forgotPasswordResponseDto, CancellationToken cancellationToken)
        {
            forgotPasswordResponseDto = AntiXssUtility.EncodeDto(forgotPasswordResponseDto);

            var employee = await repository.GetByMailAsync(forgotPasswordResponseDto.Email, cancellationToken);
            if (employee.ForgotCode != forgotPasswordResponseDto.ForgotCode)
            {
                return Unauthorized("Kod yanlış veya geçersiz.");
            }

            if (employee == null)
            {
                return Unauthorized("Geçersiz mail");
            }

            var (hashedPassword, salt) = passwordHasher.HashPassword(forgotPasswordResponseDto.NewPassword);

            employee.Password = hashedPassword;
            employee.Salt = salt;

            employee.ForgotCode = null;

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
