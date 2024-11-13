using AutoMapper;
using InventoryApp.Application.Dto;
using InventoryApp.Application.Interfaces;
using InventoryApp.Models.Entity;
using InventoryApp.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductTypeController : ControllerBase
    {
        private readonly IRepository<ProductType> repository;
        private readonly IMapper mapper;
        public ProductTypeController(IRepository<ProductType> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductTypes()
        {
            var types = await repository.GetAllAsync();
            List<ProductTypeDto> productTypeDto = mapper.Map<List<ProductTypeDto>>(types.OrderBy(x => x.Name));
            return Ok(productTypeDto);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductType(int id)
        {
            var productType = await repository.GetByIdAsync(id);

            if (productType == null)
            {
                return NotFound();
            }

            var productTypeDto = mapper.Map<ProductTypeDto>(productType);
            return Ok(productTypeDto);
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetProductTypesByName([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("İsim kısmı boş geçilemez.");
            }

            var productType = await repository.GetByNameAsync(name);

            if (productType.Count == 0)
            {
                return NotFound("Bu isme sahip Product Type yok.");
            }

            List<ProductTypeDto> productTypeDto = mapper.Map<List<ProductTypeDto>>(productType);

            return Ok(productType);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductType([FromBody] ProductTypeDto productTypeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var productType = new ProductType
            {
                Name = productTypeDto.Name
            };

            await repository.CreateAsync(productType);

            var createdProductTypeDto = mapper.Map<ProductTypeDto>(productType);

            return Created("", productTypeDto); 
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductType(int id, [FromBody] ProductTypeDto productTypeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != productTypeDto.Id)
            {
                return BadRequest("ID uyuşmazlığı: URL'deki ID ile gönderilen ID aynı olmalıdır.");
            }

            var existingProductType = await repository.GetByIdAsync(id);
            if (existingProductType == null)
                return NotFound();

            existingProductType.Name = productTypeDto.Name;

            await repository.UpdateAsync(existingProductType);

            var updatedProductTypeDto = mapper.Map<ProductTypeDto>(existingProductType);
            return Ok(updatedProductTypeDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductType(int id)
        {
            var productType = await repository.GetByIdAsync(id);
            if (productType == null)
                return NotFound();

            await repository.RemoveAsync(productType);

            return NoContent();
        }
    }
}
