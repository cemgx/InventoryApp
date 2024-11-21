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
        public async Task<IActionResult> GetInventories(CancellationToken cancellationToken)
        {
            var inventories = await inventoryRepository.GetAllAsync(cancellationToken);

            var result = mapper.Map<List<InventoryResponseDto>>(inventories);
            return Ok(result);
        }

        [HttpGet("product/{productId:int}")]
        public async Task<IActionResult> GetInventoryByProductId(int productId, CancellationToken cancellationToken)
        {
            var inventories = await inventoryRepository.GetByProductIdAsync(productId, cancellationToken);

            if (inventories.IsNullOrEmpty())
            {
                return NotFound($"{productId} numaralı ProductId ile ilişkili envanter kaydı bulunamadı.");
            }

            var result = mapper.Map<List<InventoryResponseDto>>(inventories);

            return Ok(result);
        }

        [HttpGet("deliveredDateGet")]
        public async Task<IActionResult> GetInventoryByDeliveredDate([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, CancellationToken cancellationToken)
        {
            if (startDate > endDate)
            {
                return BadRequest("Başlangıç tarihi bitiş tarihinden büyük olamaz.");
            }

            var inventories = await inventoryRepository.GetByDeliveredDateAsync(startDate, endDate, cancellationToken);
            if (inventories.IsNullOrEmpty())
            {
                return NotFound($"Girdiğiniz {startDate:yyyy-MM-dd} ve {endDate:yyyy-MM-dd} tarihleri arasında ürün bulunamadı.");
            }

            var result = mapper.Map<List<InventoryResponseDto>>(inventories);
            return Ok(result);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllInventories(CancellationToken cancellationToken)
        {
            var inventories = await inventoryRepository.GetAllIncludingDeletedAsync(cancellationToken);

            var result = mapper.Map<List<InventoryResponseDto>>(inventories);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateInventory([FromBody] InventoryRequestDto inventoryRequestDto, CancellationToken cancellationToken)
        {
            var givenByEmployee = await employeeRepository.GetByIdAsync(inventoryRequestDto.GivenByEmployeeId, cancellationToken);
            var receivedByEmployee = await employeeRepository.GetByIdAsync(inventoryRequestDto.ReceivedByEmployeeId, cancellationToken);
            var product = await productRepository.GetByIdAsync(inventoryRequestDto.ProductId, cancellationToken);
            
            if (givenByEmployee == null)
                return NotFound("GivenByEmployeeId için geçerli bir Employee bulunamadı.");
            if (receivedByEmployee == null)
                return NotFound("ReceivedByEmployeeId için geçerli bir Employee bulunamadı.");
            if (product == null)
                return NotFound("ProductId için geçerli bir Product bulunamadı.");
            
            var existingInventory = await inventoryRepository.GetByProductIdWithIsTakenAsync(inventoryRequestDto.ProductId, cancellationToken);
            if (existingInventory != null && existingInventory.IsTaken)
            {
                return BadRequest("Bu ürün şu anda başka bir kişi tarafından alındı ve henüz iade edilmedi.");
            }

            var inventory = mapper.Map<Inventory>(inventoryRequestDto);

            inventory.IsTaken = inventory.DeliveredDate.HasValue && !inventory.ReturnDate.HasValue;

            await inventoryRepository.CreateAsync(inventory, cancellationToken);
            var result = mapper.Map<InventoryRequestDto>(inventory);

            return Created("", result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateInventory(int id, [FromBody] InventoryRequestDto inventoryRequestDto, CancellationToken cancellationToken)
        {
            var inventory = await inventoryRepository.GetByIdAsync(id, cancellationToken);
            if (inventory == null)
                return NotFound($"{id} numaralı Id ile eşleşen bir Inventory bulunamadı.");

            var givenByEmployee = await employeeRepository.GetByIdAsync(inventoryRequestDto.GivenByEmployeeId, cancellationToken);
            var receivedByEmployee = await employeeRepository.GetByIdAsync(inventoryRequestDto.ReceivedByEmployeeId, cancellationToken);
            var product = await productRepository.GetByIdAsync(inventoryRequestDto.ProductId, cancellationToken);

            if (givenByEmployee == null)
                return NotFound("GivenByEmployeeId için geçerli bir Employee bulunamadı.");
            if (receivedByEmployee == null)
                return NotFound("ReceivedByEmployeeId için geçerli bir Employee bulunamadı.");
            if (product == null)
                return NotFound("ProductId için geçerli bir Product bulunamadı.");

            mapper.Map(inventoryRequestDto, inventory);

            inventory.IsTaken = inventory.DeliveredDate.HasValue && !inventory.ReturnDate.HasValue;

            await inventoryRepository.UpdateAsync(inventory, cancellationToken);

            return Ok($"{id} numaralı Id ile eşleşen Inventory güncellendi.");
        }

        [HttpPut("{id}/updateReturnDate")]
        public async Task<IActionResult> UpdateReturnDate(int id, [FromBody] DateTime? returnDate, CancellationToken cancellationToken)
        {
            var existingInventory = await inventoryRepository.GetByIdAsync(id, cancellationToken);
            if (existingInventory == null)
            {
                return NotFound($"{id} numaralı Id ile eşleşen bir envanter kaydı bulunamadı.");
            }

            await inventoryRepository.UpdateReturnDateAsync(id, returnDate, cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventory(int id, CancellationToken cancellationToken)
        {
            var inventory = await inventoryRepository.GetByIdAsync(id, cancellationToken);
            if (inventory == null)
                return NotFound($"Id = {id} bulunamadı.");

            await inventoryRepository.SoftDeleteAsync(inventory, cancellationToken);

            return Ok($"{id} numaralı Inventory başarıyla kaldırıldı.");
        }
    }
}