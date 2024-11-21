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
        public async Task<IActionResult> GetProductTypes(CancellationToken cancellationToken)
        {
            var types = await repository.GetAllAsync(cancellationToken);
            if (types.IsNullOrEmpty())
                return NotFound();

            var orderByProductType = types.OrderBy(x => x.Name);
            var result = mapper.Map<List<ProductTypeResponseDto>>(orderByProductType);
            return Ok(result);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductType(int id, CancellationToken cancellationToken)
        {
            var productType = await repository.GetByProductTypeIdAsync(id, cancellationToken);
            if (productType.IsNullOrEmpty())
                return NotFound();

            var result = mapper.Map<List<ProductTypeResponseDto>>(productType);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetProductTypesByName([FromQuery] string name, CancellationToken cancellationToken)
        {
            var productType = await repository.GetByNameAsync(name, cancellationToken);
            if (productType.IsNullOrEmpty())
                return NotFound("Bu isme sahip Product Type yok.");

            var result = mapper.Map<List<ProductTypeResponseDto>>(productType);
            return Ok(result);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllProductTypes(CancellationToken cancellationToken)
        {
            var types = await repository.GetAllIncludingDeletedAsync(cancellationToken);
            if (types.IsNullOrEmpty())
                return NotFound();

            var orderByProductType = types.OrderBy(x => x.Name);
            var result = mapper.Map<List<ProductTypeResponseDto>>(orderByProductType);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateProductType([FromBody] ProductTypeRequestDto productTypeRequestDto, CancellationToken cancellationToken)
        {
            var product = mapper.Map<ProductType>(productTypeRequestDto);
            await repository.CreateAsync(product, cancellationToken);

            var result = mapper.Map<ProductTypeResponseDto>(product);
            return Created("", result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProductType(int id, [FromBody] ProductTypeRequestDto productTypeRequestDto, CancellationToken cancellationToken)
        {
            var existingProductType = await repository.GetByIdAsync(id, cancellationToken);
            if (existingProductType == null)
                return NotFound();

            mapper.Map(productTypeRequestDto, existingProductType);

            await repository.UpdateAsync(existingProductType, cancellationToken);

            var result = mapper.Map<ProductTypeResponseDto>(existingProductType);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductType(int id, CancellationToken cancellationToken)
        {
            var productType = await repository.GetByIdAsync(id, cancellationToken);
            if (productType == null)
                return NotFound($"Id = {id} bulunamadı.");

            await repository.SoftDeleteAsync(productType, cancellationToken);

            return Ok($"{id} numaralı ProductType başarıyla kaldırıldı.");
        }
    }
}
