using AutoMapper;
using InventoryApp.Application.Dto;
using InventoryApp.Application.Interfaces;
using InventoryApp.Models.Context;
using InventoryApp.Models.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

            if (employees == null)
            {
                return NotFound();
            }
            List<EmployeeResponseDto> employeeResponseDto = mapper.Map<List<EmployeeResponseDto>>(employees.OrderBy(x => x.Name));

            return Ok(employeeResponseDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var employee = await repository.GetByEmployeeIdAsync(id);
            if (employee == null)
            {
                return NotFound($"{id} numaralı id ile bir employee bulunamadı.");
            }

            var employeeResponseDto = mapper.Map<List<EmployeeResponseDto>>(employee);
            return Ok(employeeResponseDto);
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetEmployeesByName([FromQuery] string name)
        {
            var employees = await repository.GetByNameAsync(name);

            if (employees.Count == 0)
            {
                return NotFound("Bu isme sahip Employee yok.");
            }

            List<EmployeeResponseDto> employeeResponseDto = mapper.Map<List<EmployeeResponseDto>>(employees);

            return Ok(employeeResponseDto);
        }


        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeRequestDto employeeRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var employee = mapper.Map<Employee>(employeeRequestDto);
            await repository.CreateAsync(employee);

            var createdEmployeeRequestDto = mapper.Map<EmployeeRequestDto>(employee);
            return Created("", createdEmployeeRequestDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeRequestDto employeeRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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

            await repository.RemoveAsync(employee);

            return Ok($"{id} numaralı Employee başarıyla silindi.");
        }
    }
}
