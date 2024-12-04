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
    public class ProductTypeController : ControllerBase
    {
        private readonly IProductTypeRepository _repository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        private const string CacheKey = "Inventories_List";

        public ProductTypeController(IProductTypeRepository repository, IMapper mapper, IMemoryCache cache)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductTypes(CancellationToken cancellationToken)
        {
            if (!_cache.TryGetValue(CacheKey, out List<ProductTypeResponseDto> cachedProductTypes))
            {
                var types = await _repository.GetAllAsync(cancellationToken);
                if (types.IsNullOrEmpty())
                    return NotFound();

                var orderByProductType = types.OrderBy(x => x.Name);
                cachedProductTypes = _mapper.Map<List<ProductTypeResponseDto>>(orderByProductType);

                _cache.Set(CacheKey, cachedProductTypes, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
            }

            return Ok(cachedProductTypes);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductType(int id, CancellationToken cancellationToken)
        {
            var productType = await _repository.GetByProductTypeIdAsync(id, cancellationToken);
            if (productType.IsNullOrEmpty())
                return NotFound();

            var result = _mapper.Map<List<ProductTypeResponseDto>>(productType);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetProductTypesByName([FromQuery] string name, CancellationToken cancellationToken)
        {
            var productType = await _repository.GetByNameAsync(name, cancellationToken);
            if (productType.IsNullOrEmpty())
                return NotFound("Bu isme sahip Product Type yok.");

            var result = _mapper.Map<List<ProductTypeResponseDto>>(productType);
            return Ok(result);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllProductTypes(CancellationToken cancellationToken)
        {
            if (!_cache.TryGetValue(CacheKey, out List<ProductTypeResponseDto> cachedAllProductTypes))
            {
                var types = await _repository.GetAllIncludingDeletedAsync(cancellationToken);
                if (types.IsNullOrEmpty())
                    return NotFound();

                var orderByProductType = types.OrderBy(x => x.Name);
                cachedAllProductTypes = _mapper.Map<List<ProductTypeResponseDto>>(orderByProductType);

                _cache.Set(CacheKey, cachedAllProductTypes, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
            }

            return Ok(cachedAllProductTypes);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateProductType([FromBody] ProductTypeRequestDto productTypeRequestDto, CancellationToken cancellationToken)
        {
            var product = _mapper.Map<ProductType>(productTypeRequestDto);
            await _repository.CreateAsync(product, cancellationToken);

            _cache.Remove(CacheKey);

            var result = _mapper.Map<ProductTypeResponseDto>(product);
            return Created("", result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProductType(int id, [FromBody] ProductTypeRequestDto productTypeRequestDto, CancellationToken cancellationToken)
        {
            var existingProductType = await _repository.GetByIdAsync(id, cancellationToken);
            if (existingProductType == null)
                return NotFound();

            _mapper.Map(productTypeRequestDto, existingProductType);

            await _repository.UpdateAsync(existingProductType, cancellationToken);

            _cache.Remove(CacheKey);

            var result = _mapper.Map<ProductTypeResponseDto>(existingProductType);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductType(int id, CancellationToken cancellationToken)
        {
            var productType = await _repository.GetByIdAsync(id, cancellationToken);
            if (productType == null)
                return NotFound($"Id = {id} bulunamadı.");

            await _repository.SoftDeleteAsync(productType, cancellationToken);

            _cache.Remove(CacheKey);

            return Ok($"{id} numaralı ProductType başarıyla kaldırıldı.");
        }
    }
}
