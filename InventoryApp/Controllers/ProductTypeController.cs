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
            if (types.IsNullOrEmpty())
                return NotFound();

            var orderByProductType = types.OrderBy(x => x.Name);
            var result = mapper.Map<List<ProductTypeResponseDto>>(orderByProductType);
            return Ok(result);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductType(int id)
        {
            var productType = await repository.GetByProductTypeIdAsync(id);
            if (productType.IsNullOrEmpty())
                return NotFound();

            var result = mapper.Map<List<ProductTypeResponseDto>>(productType);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetProductTypesByName([FromQuery] string name)
        {
            var productType = await repository.GetByNameAsync(name);
            if (productType.IsNullOrEmpty())
                return NotFound("Bu isme sahip Product Type yok.");

            var result = mapper.Map<List<ProductTypeResponseDto>>(productType);
            return Ok(result);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllProductTypes()
        {
            var types = await repository.GetAllIncludingDeletedAsync();
            if (types.IsNullOrEmpty())
                return NotFound();

            var orderByProductType = types.OrderBy(x => x.Name);
            var result = mapper.Map<List<ProductTypeResponseDto>>(orderByProductType);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateProductType([FromBody] ProductTypeRequestDto productTypeRequestDto)
        {
            var product = mapper.Map<ProductType>(productTypeRequestDto);
            await repository.CreateAsync(product);

            var result = mapper.Map<ProductTypeResponseDto>(product);
            return Created("", result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProductType(int id, [FromBody] ProductTypeRequestDto productTypeRequestDto)
        {
            var existingProductType = await repository.GetByIdAsync(id);
            if (existingProductType == null)
                return NotFound();

            mapper.Map(productTypeRequestDto, existingProductType);

            await repository.UpdateAsync(existingProductType);

            var result = mapper.Map<ProductTypeRequestDto>(existingProductType);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductType(int id)
        {
            var productType = await repository.GetByIdAsync(id);
            if (productType == null)
                return NotFound($"Id = {id} bulunamadı.");

            await repository.SoftDeleteAsync(productType);

            return Ok($"{id} numaralı ProductType başarıyla kaldırıldı.");
        }
    }
}
