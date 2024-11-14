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
        private readonly IRepository<Employee> repository;
        private readonly IMapper mapper;

        public EmployeeController(IRepository<Employee> repository, IMapper mapper)
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

            var employeesDto = employees
                .OrderBy(x => x.Name)
                .Select(employee => new 
                {
                    employee.Id, 
                    employee.Name,
                    employee.Email
                })
                .ToList();
            return Ok(employeesDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var employee = await repository.GetByIdAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            var employeeDto = mapper.Map<EmployeeDto>(employee);
            return Ok(employeeDto);
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetEmployeesByName([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("İsim kısmı boş geçilemez.");
            }

            var employees = await repository.GetByNameAsync(name);

            if (employees.Count == 0)
            {
                return NotFound("Bu isme sahip Employee yok.");
            }

            List<EmployeeDto> employeesDto = mapper.Map<List<EmployeeDto>>(employees);

            return Ok(employeesDto);
        }


        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeDto employeeDto)
        {
            if (employeeDto == null)
            {
                return BadRequest("Employee boş bırakılamaz.");
            }

            var employee = mapper.Map<Employee>(employeeDto);

            await repository.CreateAsync(employee);

            var createdEmployeeDto = mapper.Map<EmployeeDto>(employee);
            return Created("", createdEmployeeDto);
        }

        // PUT: api/Employee/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] EmployeeDto employeeDto)
        {
            if (employeeDto == null)
            {
                return BadRequest("EmployeeId boş bırakılamaz.");
            }

            var existingEmployee = await repository.GetByIdAsync(id);
            if (existingEmployee == null)
            {
                return NotFound();
            }

            mapper.Map(employeeDto, existingEmployee);

            await repository.UpdateAsync(existingEmployee);

            return NoContent();
        }

        // DELETE: api/Employee/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await repository.GetByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            await repository.RemoveAsync(employee);

            return NoContent();
        }
    }
}
