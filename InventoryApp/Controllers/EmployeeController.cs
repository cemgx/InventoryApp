using AutoMapper;
using InventoryApp.Application.Dto;
using InventoryApp.Application.Hash;
using InventoryApp.Application.Interfaces;
using InventoryApp.Application.LogEntities;
using InventoryApp.Models.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Compliance.Redaction;
using Microsoft.IdentityModel.Tokens;

namespace InventoryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository repository;
        private readonly IMapper mapper;
        private readonly ILogger<EmployeeController> _logger;
        private readonly Redactor _redactor;

        public EmployeeController(IEmployeeRepository repository, IMapper mapper, ILogger<EmployeeController> logger, Redactor redactor)
        {
            this.repository = repository;
            this.mapper = mapper;
            _logger = logger;
            _redactor = redactor;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees(CancellationToken cancellationToken)
        {
            var employees = await repository.GetAllAsync(cancellationToken);
            if (employees.IsNullOrEmpty())
            {
                return NotFound("Hiç employee yok");
            }

            var orderByEmployees = employees.OrderBy(x => x.Name);
            var result = mapper.Map<List<EmployeeResponseDto>>(orderByEmployees);
            return Ok(result);
        }

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

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeRequestDto employeeRequestDto, CancellationToken cancellationToken)
        {
            var existingEmployee = await repository.GetByMailAsync(employeeRequestDto.Email, cancellationToken);
            if (existingEmployee != null)
                return NotFound("Bu maille zaten bir kullanıcı mevcut.");

            var passwordHasher = new PasswordHasher();
            var (hashedPassword, salt) = passwordHasher.HashPassword(employeeRequestDto.Password);

            var employee = mapper.Map<Employee>(employeeRequestDto);
            employee.Password = hashedPassword;
            employee.Salt = salt;

            var randomCode = repository.GenerateRandomString(6);
            employee.MailVerificationCode = randomCode;

            await repository.CreateAsync(employee, cancellationToken);

            var employeeLog = new EmployeeLog
            {
                Name = _redactor.Redact(employeeRequestDto.Name),
                Email = _redactor.Redact(employeeRequestDto.Email),
                Password = "***" // Şifre logda asla görünmemeli
            };

            await repository.LogEmployeeData(employeeLog, cancellationToken);

            _logger.LogError(employeeLog.Name);

            var result = mapper.Map<EmployeeResponseDto>(employee);
            return Created("", $"Hesabınız başarıyla oluşturuldu. Giriş yapabilmek için mailinizi {randomCode} ile onaylamanız gerekmektedir.");
        }

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
