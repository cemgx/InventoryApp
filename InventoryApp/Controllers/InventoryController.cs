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
        private readonly IInventoryRepository repository;
        private readonly IMapper mapper;

        public InventoryController(IInventoryRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateInventory([FromBody] InventoryDto inventoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var inventory = mapper.Map<Inventory>(inventoryDto);

            await repository.CreateAsync(inventory);

            var createdInventoryDto = mapper.Map<InventoryDto>(inventory);
            return Created("", createdInventoryDto);
        }

    }
}