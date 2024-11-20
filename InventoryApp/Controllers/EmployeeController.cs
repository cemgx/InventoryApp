using AutoMapper;
using InventoryApp.Application.Dto;
using InventoryApp.Application.Interfaces;
using InventoryApp.Models.Context;
using InventoryApp.Models.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Linq;

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

        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await repository.GetAllAsync();
            if (employees.IsNullOrEmpty())
            {
                return NotFound();
            }

            var orderByEmployees = employees.OrderBy(x => x.Name);
            var result = mapper.Map<List<EmployeeResponseDto>>(orderByEmployees);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var employee = await repository.GetByEmployeeIdAsync(id);
            if (employee.IsNullOrEmpty())
            {
                return NotFound($"{id} numaralı id ile bir employee bulunamadı.");
            }

            var result = mapper.Map<List<EmployeeResponseDto>>(employee);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetEmployeesByName([FromQuery] string name)
        {
            var employees = await repository.GetByNameAsync(name);
            if (employees.IsNullOrEmpty())
            {
                return NotFound("Bu isme sahip Employee yok.");
            }

            var result = mapper.Map<List<EmployeeResponseDto>>(employees);
            return Ok(result);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await repository.GetAllIncludingDeletedAsync();
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
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeRequestDto employeeRequestDto)
        {
            var employee = mapper.Map<Employee>(employeeRequestDto);
            await repository.CreateAsync(employee);

            var createdEmployeeRequestDto = mapper.Map<EmployeeRequestDto>(employee);
            return Created("", createdEmployeeRequestDto);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeRequestDto employeeRequestDto)
        {
            var existingEmployee = await repository.GetByIdAsync(id);
            if (existingEmployee == null)
            {
                return NotFound();
            }

            mapper.Map(employeeRequestDto, existingEmployee);

            await repository.UpdateAsync(existingEmployee);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await repository.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound($"{id} numaralı id ile bir employee bulunamadı.");
            }

            await repository.SoftDeleteAsync(employee);

            return Ok($"{id} numaralı Employee başarıyla kaldırıldı.");
        }
    }
}
