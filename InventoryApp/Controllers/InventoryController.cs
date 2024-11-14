using AutoMapper;
using InventoryApp.Application.Dto;
using InventoryApp.Application.Interfaces;
using InventoryApp.Models.Entity;
using Microsoft.AspNetCore.Http;
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

        //[HttpGet("{productId} Find")]
        //public async Task<IActionResult> GetInventoryByProductId(int productId)
        //{
        //    // ProductId'ye göre envanteri arama
        //    var inventories = await inventoryRepository.GetByProductIdAsync(productId);

        //    if (inventories == null || !inventories.Any())
        //    {
        //        return NotFound($"ProductId {productId} ile ilişkili envanter kaydı bulunamadı.");
        //    }

        //    // Envanter verisini InventoryDto'ya dönüştürme
        //    var inventoriesDto = mapper.Map<List<InventoryDto>>(inventories);

        //    return Ok(inventoriesDto);
        //}


        [HttpPost]
        public async Task<IActionResult> CreateInventory([FromBody] InventoryDto inventoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Employee ve Product ID kontrolleri
            var givenByEmployee = await employeeRepository.GetByIdAsync(inventoryDto.GivenByEmployeeId);
            var receivedByEmployee = await employeeRepository.GetByIdAsync(inventoryDto.ReceivedByEmployeeId);
            var product = await productRepository.GetByIdAsync(inventoryDto.ProductId);

            if (givenByEmployee == null)
                return NotFound("GivenByEmployeeId için geçerli bir Employee bulunamadı.");
            if (receivedByEmployee == null)
                return NotFound("ReceivedByEmployeeId için geçerli bir Employee bulunamadı.");
            if (product == null)
                return NotFound("ProductId için geçerli bir Product bulunamadı.");

            // Product isTaken kontrolü
            var existingInventory = await inventoryRepository.GetByProductIdWithIsTakenAsync(inventoryDto.ProductId);
            if (existingInventory != null && existingInventory.IsTaken)
            {
                return BadRequest("Bu ürün şu anda başka bir kişi tarafından alındı ve iade edilmedi.");
            }

            // Inventory oluşturma
            var inventory = mapper.Map<Inventory>(inventoryDto);

            // IsTaken belirleme
            inventory.IsTaken = inventory.DeliveredDate.HasValue && !inventory.ReturnDate.HasValue;

            await inventoryRepository.CreateAsync(inventory);
            var createdInventoryDto = mapper.Map<InventoryDto>(inventory);

            return Created("", createdInventoryDto);
        }

    }
}