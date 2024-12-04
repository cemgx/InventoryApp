using AutoMapper;
using InventoryApp.Application.Dto;
using InventoryApp.Application.Interfaces;
using InventoryApp.Application.Utility;
using InventoryApp.Models.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

namespace InventoryApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        private const string CacheKey = "Inventories_List";

        public InventoryController(IInventoryRepository inventoryRepository, IEmployeeRepository employeeRepository, IProductRepository productRepository, IMapper mapper, IMemoryCache cache)
        {
            _inventoryRepository = inventoryRepository;
            _employeeRepository = employeeRepository;
            _productRepository = productRepository;
            _mapper = mapper;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> GetInventories(CancellationToken cancellationToken)
        {
            if (!_cache.TryGetValue(CacheKey, out List<InventoryResponseDto> cachedInventories))
            {
                var inventories = await _inventoryRepository.GetAllAsync(cancellationToken);
                if (inventories.IsNullOrEmpty())
                {
                    return NotFound("Hiç inventory yok");
                }

                cachedInventories = _mapper.Map<List<InventoryResponseDto>>(inventories);

                _cache.Set(CacheKey, cachedInventories, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
            }

            return Ok(cachedInventories);
        }

        [HttpGet("product/{productId:int}")]
        public async Task<IActionResult> GetInventoryByProductId(int productId, CancellationToken cancellationToken)
        {
            var inventories = await _inventoryRepository.GetByProductIdAsync(productId, cancellationToken);

            if (inventories.IsNullOrEmpty())
            {
                return NotFound($"{productId} numaralı ProductId ile ilişkili envanter kaydı bulunamadı.");
            }

            var result = _mapper.Map<List<InventoryResponseDto>>(inventories);

            return Ok(result);
        }

        [HttpGet("deliveredDateGet")]
        public async Task<IActionResult> GetInventoryByDeliveredDate([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, CancellationToken cancellationToken)
        {
            if (!_cache.TryGetValue(CacheKey, out List<InventoryResponseDto> cachedInventoriesByDate))
            {
                if (startDate > endDate)
                {
                    return BadRequest("Başlangıç tarihi bitiş tarihinden büyük olamaz.");
                }

                var inventories = await _inventoryRepository.GetByDeliveredDateAsync(startDate, endDate, cancellationToken);
                if (inventories.IsNullOrEmpty())
                {
                    return NotFound($"Girdiğiniz {startDate:yyyy-MM-dd} ve {endDate:yyyy-MM-dd} tarihleri arasında ürün bulunamadı.");
                }

                cachedInventoriesByDate = _mapper.Map<List<InventoryResponseDto>>(inventories);

                _cache.Set(CacheKey, cachedInventoriesByDate, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
            }

            return Ok(cachedInventoriesByDate);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllInventories(CancellationToken cancellationToken)
        {
            if (!_cache.TryGetValue(CacheKey, out List<InventoryResponseDto> cachedAllEmployees))
            {
                var inventories = await _inventoryRepository.GetAllIncludingDeletedAsync(cancellationToken);

                cachedAllEmployees = _mapper.Map<List<InventoryResponseDto>>(inventories);

                _cache.Set(CacheKey, cachedAllEmployees, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
            }

            return Ok(cachedAllEmployees);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateInventory([FromBody] InventoryRequestDto inventoryRequestDto, CancellationToken cancellationToken)
        {
            var givenByEmployee = await _employeeRepository.GetByIdAsync(inventoryRequestDto.GivenByEmployeeId, cancellationToken);
            var receivedByEmployee = await _employeeRepository.GetByIdAsync(inventoryRequestDto.ReceivedByEmployeeId, cancellationToken);
            var product = await _productRepository.GetByIdAsync(inventoryRequestDto.ProductId, cancellationToken);
            
            if (givenByEmployee == null)
                return NotFound("GivenByEmployeeId için geçerli bir Employee bulunamadı.");
            if (receivedByEmployee == null)
                return NotFound("ReceivedByEmployeeId için geçerli bir Employee bulunamadı.");
            if (product == null)
                return NotFound("ProductId için geçerli bir Product bulunamadı.");
            
            var existingInventory = await _inventoryRepository.GetByProductIdWithIsTakenAsync(inventoryRequestDto.ProductId, cancellationToken);
            if (existingInventory != null && existingInventory.IsTaken)
            {
                return BadRequest("Bu ürün şu anda başka bir kişi tarafından alındı ve henüz iade edilmedi.");
            }

            var inventory = _mapper.Map<Inventory>(inventoryRequestDto);

            inventory.IsTaken = inventory.DeliveredDate.HasValue && !inventory.ReturnDate.HasValue;

            await _inventoryRepository.CreateAsync(inventory, cancellationToken);

            _cache.Remove(CacheKey);

            var result = _mapper.Map<InventoryResponseDto>(inventory);
            return Created("", result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateInventory(int id, [FromBody] InventoryRequestDto inventoryRequestDto, CancellationToken cancellationToken)
        {
            var inventory = await _inventoryRepository.GetByIdAsync(id, cancellationToken);
            if (inventory == null)
                return NotFound($"{id} numaralı Id ile eşleşen bir Inventory bulunamadı.");

            var givenByEmployee = await _employeeRepository.GetByIdAsync(inventoryRequestDto.GivenByEmployeeId, cancellationToken);
            var receivedByEmployee = await _employeeRepository.GetByIdAsync(inventoryRequestDto.ReceivedByEmployeeId, cancellationToken);
            var product = await _productRepository.GetByIdAsync(inventoryRequestDto.ProductId, cancellationToken);

            if (givenByEmployee == null)
                return NotFound("GivenByEmployeeId için geçerli bir Employee bulunamadı.");
            if (receivedByEmployee == null)
                return NotFound("ReceivedByEmployeeId için geçerli bir Employee bulunamadı.");
            if (product == null)
                return NotFound("ProductId için geçerli bir Product bulunamadı.");

            _mapper.Map(inventoryRequestDto, inventory);

            inventory.IsTaken = inventory.DeliveredDate.HasValue && !inventory.ReturnDate.HasValue;

            await _inventoryRepository.UpdateAsync(inventory, cancellationToken);

            _cache.Remove(CacheKey);

            return Ok($"{id} numaralı Id ile eşleşen Inventory güncellendi.");
        }

        [HttpPut("{id}/updateReturnDate")]
        public async Task<IActionResult> UpdateReturnDate(int id, [FromBody] DateTime? returnDate, CancellationToken cancellationToken)
        {
            var existingInventory = await _inventoryRepository.GetByIdAsync(id, cancellationToken);
            if (existingInventory == null)
            {
                return NotFound($"{id} numaralı Id ile eşleşen bir envanter kaydı bulunamadı.");
            }

            await _inventoryRepository.UpdateReturnDateAsync(id, returnDate, cancellationToken);

            _cache.Remove(CacheKey);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventory(int id, CancellationToken cancellationToken)
        {
            var inventory = await _inventoryRepository.GetByIdAsync(id, cancellationToken);
            if (inventory == null)
                return NotFound($"Id = {id} bulunamadı.");

            await _inventoryRepository.SoftDeleteAsync(inventory, cancellationToken);

            _cache.Remove(CacheKey);

            return Ok($"{id} numaralı Inventory başarıyla kaldırıldı.");
        }
    }
}