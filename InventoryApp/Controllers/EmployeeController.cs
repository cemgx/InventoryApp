using AutoMapper;
using InventoryApp.Application.Dto;
using InventoryApp.Application.Hash;
using InventoryApp.Application.Interfaces;
using InventoryApp.Application.LogEntities;
using InventoryApp.Models.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Compliance.Redaction;
using Microsoft.IdentityModel.Tokens;

namespace InventoryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeController> _logger;
        private readonly Redactor _redactor;
        private readonly IMemoryCache _cache;

        private const string CacheKey = "Employees_List";
        private const string AllEmployeesCacheKey = "Employees_List_All";

        public EmployeeController(IEmployeeRepository repository, IMapper mapper, ILogger<EmployeeController> logger, Redactor redactor, IMemoryCache cache)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _redactor = redactor;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees(CancellationToken cancellationToken)
        {
            if (!_cache.TryGetValue(CacheKey, out List<EmployeeResponseDto> cachedEmployees))
            {
                var employees = await _repository.GetAllAsync(cancellationToken);
                if (employees.IsNullOrEmpty())
                {
                    return NotFound("Hiç employee yok");
                }

                var orderedEmployees = employees.OrderBy(x => x.Name);
                cachedEmployees = _mapper.Map<List<EmployeeResponseDto>>(orderedEmployees);

                _cache.Set(CacheKey, cachedEmployees, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
            }

            return Ok(cachedEmployees);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(int id, CancellationToken cancellationToken)
        {
            var employee = await _repository.GetByEmployeeIdAsync(id, cancellationToken);
            if (employee.IsNullOrEmpty())
            {
                return NotFound($"{id} numaralı id ile bir employee bulunamadı.");
            }

            var result = _mapper.Map<List<EmployeeResponseDto>>(employee);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetEmployeesByName([FromQuery] string name, CancellationToken cancellationToken)
        {
            var employees = await _repository.GetByNameAsync(name, cancellationToken);
            if (employees.IsNullOrEmpty())
            {
                return NotFound("Bu isme sahip Employee yok.");
            }

            var result = _mapper.Map<List<EmployeeResponseDto>>(employees);
            return Ok(result);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllEmployees(CancellationToken cancellationToken)
        {
            if (!_cache.TryGetValue(AllEmployeesCacheKey, out List<EmployeeResponseDto> cachedAllEmployees))
            {
                var employees = await _repository.GetAllIncludingDeletedAsync(cancellationToken);
                if (employees.IsNullOrEmpty())
                {
                    return NotFound();
                }

                var orderByEmployees = employees.OrderBy(x => x.Name);
                cachedAllEmployees = _mapper.Map<List<EmployeeResponseDto>>(orderByEmployees);
                _cache.Set(AllEmployeesCacheKey, cachedAllEmployees, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
            }

            return Ok(cachedAllEmployees);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeRequestDto employeeRequestDto, CancellationToken cancellationToken)
        {
            var existingEmployee = await _repository.GetByMailAsync(employeeRequestDto.Email, cancellationToken);
            if (existingEmployee != null)
                return NotFound("Bu maille zaten bir kullanıcı mevcut.");

            var passwordHasher = new PasswordHasher();
            var (hashedPassword, salt) = passwordHasher.HashPassword(employeeRequestDto.Password);

            var employee = _mapper.Map<Employee>(employeeRequestDto);
            employee.Password = hashedPassword;
            employee.Salt = salt;

            var randomCode = _repository.GenerateRandomString(6);
            employee.MailVerificationCode = randomCode;

            await _repository.CreateAsync(employee, cancellationToken);

            _cache.Remove(CacheKey);
            _cache.Remove(AllEmployeesCacheKey);

            var employeeLog = new EmployeeLog
            {
                Name = _redactor.Redact(employeeRequestDto.Name),
                Email = _redactor.Redact(employeeRequestDto.Email),
                Password = _redactor.Redact(employeeRequestDto.Password)
            };

            await _repository.LogEmployeeData(employeeLog, cancellationToken);

            //_logger.LogError(employeeLog.Name);

            var result = _mapper.Map<EmployeeResponseDto>(employee);
            return Created("", $"Hesabınız başarıyla oluşturuldu. Giriş yapabilmek için mailinizi {randomCode} ile onaylamanız gerekmektedir.");
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeRequestDto employeeRequestDto, CancellationToken cancellationToken)
        {
            var existingEmployee = await _repository.GetByIdAsync(id, cancellationToken);
            if (existingEmployee == null)
            {
                return NotFound();
            }

            var passwordHasher = new PasswordHasher();
            var (hashedPassword, salt) = passwordHasher.HashPassword(employeeRequestDto.Password);

            _mapper.Map(employeeRequestDto, existingEmployee);
            existingEmployee.Password = hashedPassword;
            existingEmployee.Salt = salt;

            await _repository.UpdateAsync(existingEmployee, cancellationToken);

            _cache.Remove(CacheKey);
            _cache.Remove(AllEmployeesCacheKey);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id, CancellationToken cancellationToken)
        {
            var employee = await _repository.GetByIdAsync(id, cancellationToken);
            if (employee == null)
            {
                return NotFound($"{id} numaralı id ile bir employee bulunamadı.");
            }

            await _repository.SoftDeleteAsync(employee, cancellationToken);

            _cache.Remove(CacheKey);
            _cache.Remove(AllEmployeesCacheKey);

            return Ok($"{id} numaralı Employee başarıyla kaldırıldı.");
        }
    }
}
