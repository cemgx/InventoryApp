using AutoMapper;
using InventoryApp.Application.Dto;
using InventoryApp.Application.Interfaces;
using InventoryApp.Models.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace InventoryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryRepository inventoryRepository;
        private readonly IEmployeeRepository employeeRepository;
        private readonly IProductRepository productRepository;
        private readonly IMapper mapper;

        public InventoryController(IInventoryRepository inventoryRepository, IEmployeeRepository employeeRepository, IProductRepository productRepository, IMapper mapper)
        {
            this.inventoryRepository = inventoryRepository;
            this.employeeRepository = employeeRepository;
            this.productRepository = productRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetInventories()
        {
            var types = await inventoryRepository.GetAllAsync();
            List<InventoryResponseDto> inventoryResponseDto = mapper.Map<List<InventoryResponseDto>>(types);
            return Ok(inventoryResponseDto);
        }

        [HttpGet("product/{productId:int}")]
        public async Task<IActionResult> GetInventoryByProductId(int productId)
        {
            var inventories = await inventoryRepository.GetByProductIdAsync(productId);

            if (inventories.IsNullOrEmpty())
            {
                return NotFound($"{productId} numaralı ProductId ile ilişkili envanter kaydı bulunamadı.");
            }

            var inventoriesDto = mapper.Map<List<InventoryResponseDto>>(inventories);

            return Ok(inventoriesDto);
        }

        [HttpGet("deliveredDateGet")]
        public async Task<IActionResult> GetInventoryByDeliveredDate([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
            {
                return BadRequest("Başlangıç tarihi bitiş tarihinden büyük olamaz.");
            }

            var inventories = await inventoryRepository.GetByDeliveredDateAsync(startDate, endDate);

            if (inventories.Count != 0)
            {
                var inventoriesDto = mapper.Map<List<InventoryResponseDto>>(inventories);

                return Ok(inventoriesDto);
            }

            return NotFound($"Girdiğiniz {startDate:yyyy-MM-dd} ve {endDate:yyyy-MM-dd} tarihleri arasında ürün bulunamadı.");
        }

        [HttpPost]
        public async Task<IActionResult> CreateInventory([FromBody] InventoryRequestDto inventoryRequestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var givenByEmployee = await employeeRepository.GetByIdAsync(inventoryRequestDto.GivenByEmployeeId);
            var receivedByEmployee = await employeeRepository.GetByIdAsync(inventoryRequestDto.ReceivedByEmployeeId);
            var product = await productRepository.GetByIdAsync(inventoryRequestDto.ProductId);

            if (givenByEmployee == null)
                return NotFound("GivenByEmployeeId için geçerli bir Employee bulunamadı.");
            if (receivedByEmployee == null)
                return NotFound("ReceivedByEmployeeId için geçerli bir Employee bulunamadı.");
            if (product == null)
                return NotFound("ProductId için geçerli bir Product bulunamadı.");

            var existingInventory = await inventoryRepository.GetByProductIdWithIsTakenAsync(inventoryRequestDto.ProductId);
            if (existingInventory != null && existingInventory.IsTaken)
            {
                return BadRequest("Bu ürün şu anda başka bir kişi tarafından alındı ve henüz iade edilmedi.");
            }

            var inventory = mapper.Map<Inventory>(inventoryRequestDto);

            inventory.IsTaken = inventory.DeliveredDate.HasValue && !inventory.ReturnDate.HasValue;

            await inventoryRepository.CreateAsync(inventory);
            var createdInventoryDto = mapper.Map<InventoryRequestDto>(inventory);

            return Created("", createdInventoryDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInventory(int id, [FromBody] InventoryRequestDto inventoryRequestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var inventory = await inventoryRepository.GetByIdAsync(id);
            if (inventory == null)
                return NotFound($"{id} numaralı Id ile eşleşen bir Inventory bulunamadı.");

            var givenByEmployee = await employeeRepository.GetByIdAsync(inventoryRequestDto.GivenByEmployeeId);
            var receivedByEmployee = await employeeRepository.GetByIdAsync(inventoryRequestDto.ReceivedByEmployeeId);
            var product = await productRepository.GetByIdAsync(inventoryRequestDto.ProductId);

            if (givenByEmployee == null)
                return NotFound("GivenByEmployeeId için geçerli bir Employee bulunamadı.");
            if (receivedByEmployee == null)
                return NotFound("ReceivedByEmployeeId için geçerli bir Employee bulunamadı.");
            if (product == null)
                return NotFound("ProductId için geçerli bir Product bulunamadı.");

            mapper.Map(inventoryRequestDto, inventory);

            inventory.IsTaken = inventory.DeliveredDate.HasValue && !inventory.ReturnDate.HasValue;

            await inventoryRepository.UpdateAsync(inventory);

            return NoContent();
        }

        [HttpPut("{id}/updateReturnDate")]
        public async Task<IActionResult> UpdateReturnDate(int id, [FromBody] DateTime? returnDate)
        {
            var existingInventory = await inventoryRepository.GetByIdAsync(id);
            if (existingInventory == null)
            {
                return NotFound($"{id} numaralı Id ile eşleşen bir envanter kaydı bulunamadı.");
            }

            await inventoryRepository.UpdateReturnDateAsync(id, returnDate);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventory(int id)
        {
            var inventory = await inventoryRepository.GetByIdAsync(id);
            if (inventory == null)
                return NotFound($"Id = {id} bulunamadı.");

            await inventoryRepository.RemoveAsync(inventory);

            return NoContent();
        }
    }
}