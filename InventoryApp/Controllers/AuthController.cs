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
            var employee = await repository.GetByMailAsync(loginRequestDto.Email, cancellationToken);
            if (employee == null)
            {
                return Unauthorized("Geçersiz kullanıcı adı veya şifre.");
            }

            //var passwordHasher = new Application.Hash.PasswordHasher();
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

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("EmployeeCookie");
            return Ok("Başarıyla çıkış yaptınız.");
        }
    }
}
