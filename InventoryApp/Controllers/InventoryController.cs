using AutoMapper;
using InventoryApp.Application.Dto;
using InventoryApp.Application.Interfaces;
using InventoryApp.Models.Entity;
using Microsoft.AspNetCore.Mvc;

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
            List<InventoryDto> inventoryDto = mapper.Map<List<InventoryDto>>(types);
            return Ok(inventoryDto);
        }

        [HttpGet("product/{productId:int}")]
        public async Task<IActionResult> GetInventoryByProductId(int productId)
        {
            var inventories = await inventoryRepository.GetByProductIdAsync(productId);

            if (inventories == null || inventories.Count == 0)
            {
                return NotFound($"{productId} numaralı ProductId ile ilişkili envanter kaydı bulunamadı.");
            }

            var inventoriesDto = mapper.Map<List<InventoryDto>>(inventories);

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
                var inventoriesDto = mapper.Map<List<InventoryDto>>(inventories);

                return Ok(inventoriesDto);
            }

            return NotFound($"Girdiğiniz {startDate:yyyy-MM-dd} ve {endDate:yyyy-MM-dd} tarihleri arasında ürün bulunamadı.");
        }

        [HttpPost]
        public async Task<IActionResult> CreateInventory([FromBody] InventoryDto inventoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var givenByEmployee = await employeeRepository.GetByIdAsync(inventoryDto.GivenByEmployeeId);
            var receivedByEmployee = await employeeRepository.GetByIdAsync(inventoryDto.ReceivedByEmployeeId);
            var product = await productRepository.GetByIdAsync(inventoryDto.ProductId);

            if (givenByEmployee == null)
                return NotFound("GivenByEmployeeId için geçerli bir Employee bulunamadı.");
            if (receivedByEmployee == null)
                return NotFound("ReceivedByEmployeeId için geçerli bir Employee bulunamadı.");
            if (product == null)
                return NotFound("ProductId için geçerli bir Product bulunamadı.");

            var existingInventory = await inventoryRepository.GetByProductIdWithIsTakenAsync(inventoryDto.ProductId);
            if (existingInventory != null && existingInventory.IsTaken)
            {
                return BadRequest("Bu ürün şu anda başka bir kişi tarafından alındı ve henüz iade edilmedi.");
            }

            var inventory = mapper.Map<Inventory>(inventoryDto);

            await inventoryRepository.CreateAsync(inventory);
            var createdInventoryDto = mapper.Map<InventoryDto>(inventory);

            return Created("", createdInventoryDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInventory(int id, [FromBody] InventoryDto inventoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var inventory = await inventoryRepository.GetByIdAsync(id);
            if (inventory == null)
                return NotFound($"{id} numaralı Id ile eşleşen bir Inventory bulunamadı.");

            var givenByEmployee = await employeeRepository.GetByIdAsync(inventoryDto.GivenByEmployeeId);
            var receivedByEmployee = await employeeRepository.GetByIdAsync(inventoryDto.ReceivedByEmployeeId);
            var product = await productRepository.GetByIdAsync(inventoryDto.ProductId);

            if (givenByEmployee == null)
                return NotFound("GivenByEmployeeId için geçerli bir Employee bulunamadı.");
            if (receivedByEmployee == null)
                return NotFound("ReceivedByEmployeeId için geçerli bir Employee bulunamadı.");
            if (product == null)
                return NotFound("ProductId için geçerli bir Product bulunamadı.");

            inventory.GivenByEmployeeId = inventoryDto.GivenByEmployeeId;
            inventory.ReceivedByEmployeeId = inventoryDto.ReceivedByEmployeeId;
            inventory.ProductId = inventoryDto.ProductId;
            inventory.DeliveredDate = inventoryDto.DeliveredDate;
            inventory.ReturnDate = inventoryDto.ReturnDate;

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