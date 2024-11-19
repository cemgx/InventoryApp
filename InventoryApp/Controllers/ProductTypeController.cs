using AutoMapper;
using InventoryApp.Application.Dto;
using InventoryApp.Application.Interfaces;
using InventoryApp.Models.Entity;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductTypeController : ControllerBase
    {
        private readonly IProductTypeRepository repository;
        private readonly IMapper mapper;
        public ProductTypeController(IProductTypeRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductTypes()
        {
            var types = await repository.GetAllAsync();
            if (types == null)
                return NotFound();

            List<ProductTypeResponseDto> productTypeResponseDto = mapper.Map<List<ProductTypeResponseDto>>(types.OrderBy(x => x.Name));
            return Ok(productTypeResponseDto);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductType(int id)
        {
            var productType = await repository.GetByProductTypeIdAsync(id);

            if (productType == null)
                return NotFound();

            var productTypeResponseDto = mapper.Map<List<ProductTypeResponseDto>>(productType);
            return Ok(productTypeResponseDto);
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetProductTypesByName([FromQuery] string name)
        {
            var productType = await repository.GetByNameAsync(name);
            if (productType.Count == 0)
                return NotFound("Bu isme sahip Product Type yok.");

            List<ProductTypeResponseDto> productTypeResponseDto = mapper.Map<List<ProductTypeResponseDto>>(productType);

            return Ok(productTypeResponseDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductType([FromBody] ProductTypeRequestDto productTypeRequestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = mapper.Map<ProductType>(productTypeRequestDto);
            await repository.CreateAsync(product);

            var createdProductTypeDto = mapper.Map<ProductTypeResponseDto>(product);

            return Created("", createdProductTypeDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductType(int id, [FromBody] ProductTypeRequestDto productTypeRequestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingProductType = await repository.GetByIdAsync(id);
            if (existingProductType == null)
                return NotFound();

            mapper.Map(productTypeRequestDto, existingProductType);

            await repository.UpdateAsync(existingProductType);

            var updatedProductTypeDto = mapper.Map<ProductTypeRequestDto>(existingProductType);
            return Ok(updatedProductTypeDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductType(int id)
        {
            var productType = await repository.GetByIdAsync(id);
            if (productType == null)
                return NotFound($"Id = {id} bulunamadı.");

            await repository.RemoveAsync(productType);

            return NoContent();
        }
    }
}
