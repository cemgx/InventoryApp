using AutoMapper;
using InventoryApp.Application.Dto;
using InventoryApp.Application.Interfaces;
using InventoryApp.Models.Context;
using InventoryApp.Models.Entity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;

namespace InventoryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository repository;
        private readonly IMapper mapper;

        public EmployeeController(IEmployeeRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetEmployees(CancellationToken cancellationToken)
        {
            var employees = await repository.GetAllAsync(cancellationToken);
            if (employees.IsNullOrEmpty())
            {
                return NotFound();
            }

            var orderByEmployees = employees.OrderBy(x => x.Name);
            var result = mapper.Map<List<EmployeeResponseDto>>(orderByEmployees);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(int id, CancellationToken cancellationToken)
        {
            var employee = await repository.GetByEmployeeIdAsync(id, cancellationToken);
            if (employee.IsNullOrEmpty())
            {
                return NotFound($"{id} numaralı id ile bir employee bulunamadı.");
            }

            var result = mapper.Map<List<EmployeeResponseDto>>(employee);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("search")]
        public async Task<IActionResult> GetEmployeesByName([FromQuery] string name, CancellationToken cancellationToken)
        {
            var employees = await repository.GetByNameAsync(name, cancellationToken);
            if (employees.IsNullOrEmpty())
            {
                return NotFound("Bu isme sahip Employee yok.");
            }

            var result = mapper.Map<List<EmployeeResponseDto>>(employees);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllEmployees(CancellationToken cancellationToken)
        {
            var employees = await repository.GetAllIncludingDeletedAsync(cancellationToken);
            if (employees.IsNullOrEmpty())
            {
                return NotFound();
            }

            var orderByEmployees = employees.OrderBy(x => x.Name);
            var result = mapper.Map<List<EmployeeResponseDto>>(orderByEmployees);
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeResponseDto employeeResponseDto, CancellationToken cancellationToken)
        {
            var existingEmployee = await repository.GetByNameAsync(employeeResponseDto.Email, cancellationToken);
            if (!existingEmployee.IsNullOrEmpty())
                return NotFound("Bu maille zaten bir kullanıcı mevcut.");

            var employee = mapper.Map<Employee>(employeeResponseDto);
            await repository.CreateAsync(employee, cancellationToken);

            var result = mapper.Map<EmployeeResponseDto>(employee);
            return Created("", result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string name, string password, CancellationToken cancellationToken)
        {
            var employee = await repository.GetByNameAsync(name, cancellationToken);
            if (employee.IsNullOrEmpty() || employee.First().Password != password)
            {
                return Unauthorized("Geçersiz kullanıcı adı veya şifre.");
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, name)
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


        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeRequestDto employeeRequestDto, CancellationToken cancellationToken)
        {
            var existingEmployee = await repository.GetByIdAsync(id, cancellationToken);
            if (existingEmployee == null)
            {
                return NotFound();
            }

            mapper.Map(employeeRequestDto, existingEmployee);

            await repository.UpdateAsync(existingEmployee, cancellationToken);

            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id, CancellationToken cancellationToken)
        {
            var employee = await repository.GetByIdAsync(id, cancellationToken);
            if (employee == null)
            {
                return NotFound($"{id} numaralı id ile bir employee bulunamadı.");
            }

            await repository.SoftDeleteAsync(employee, cancellationToken);

            return Ok($"{id} numaralı Employee başarıyla kaldırıldı.");
        }
    }
}
