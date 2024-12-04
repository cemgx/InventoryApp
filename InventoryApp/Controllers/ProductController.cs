using AutoMapper;
using InventoryApp.Application.Dto;
using InventoryApp.Application.Interfaces;
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
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductTypeRepository _typeRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        private const string CacheKey = "Inventories_List";

        public ProductController(IProductRepository productRepository, IProductTypeRepository typeRepository, IInvoiceRepository invoiceRepository, IMapper mapper, IMemoryCache cache)
        {
            _productRepository = productRepository;
            _typeRepository = typeRepository;
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts(CancellationToken cancellationToken)
        {
            if (!_cache.TryGetValue(CacheKey, out List<ProductResponseDto> cachedProducts))
            {
                var products = await _productRepository.GetAllAsync(cancellationToken);
                if (products.IsNullOrEmpty())
                {
                    return NotFound("Hiç product yok");
                }

                var orderByProducts = products.OrderBy(x => x.Name);
                cachedProducts = _mapper.Map<List<ProductResponseDto>>(orderByProducts);

                _cache.Set(CacheKey, cachedProducts, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
            }

            return Ok(cachedProducts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByProductIdAsync(id, cancellationToken);
            if (product.IsNullOrEmpty())
            {
                return NotFound();
            }

            var result = _mapper.Map<List<ProductResponseDto>>(product);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string name, CancellationToken cancellationToken)
        {
            if (!_cache.TryGetValue(CacheKey, out List<ProductResponseDto> cachedProductsByName))
            {
                var products = await _productRepository.GetByNameAsync(name, cancellationToken);
                if (products.IsNullOrEmpty())
                {
                    return NotFound("Bu isme sahip bir ürün bulunamadı.");
                }

                cachedProductsByName = _mapper.Map<List<ProductResponseDto>>(products);

                _cache.Set(CacheKey, cachedProductsByName, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
            }

            return Ok(cachedProductsByName);
        }

        [HttpGet("purchaseDateGet")]
        public async Task<IActionResult> GetProductsByInvoicePurchaseDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, CancellationToken cancellationToken)
        {
            if (!_cache.TryGetValue(CacheKey, out List<ProductResponseDto> cachedProductsByDate))
            {
                if (startDate > endDate)
                {
                    return BadRequest("Başlangıç tarihi, bitiş tarihinden büyük olamaz.");
                }

                var products = await _productRepository.GetByInvoicePurchaseDateAsync(startDate, endDate, cancellationToken);
                if (products.IsNullOrEmpty())
                {
                    return NotFound($"Belirtilen {startDate:yyyy-MM-dd} ve {endDate:yyyy-MM-dd} tarihleri arasında ürün bulunamadı.");
                }

                cachedProductsByDate = _mapper.Map<List<ProductResponseDto>>(products);

                _cache.Set(CacheKey, cachedProductsByDate, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
            }

            return Ok(cachedProductsByDate);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllProducts(CancellationToken cancellationToken)
        {
            if (!_cache.TryGetValue(CacheKey, out List<ProductResponseDto> cachedAllProducts))
            {
                var products = await _productRepository.GetAllIncludingDeletedAsync(cancellationToken);
                if (products.IsNullOrEmpty())
                {
                    return NotFound();
                }

                var orderByProducts = products.OrderBy(x => x.Name);
                cachedAllProducts = _mapper.Map<List<ProductResponseDto>>(orderByProducts);

                _cache.Set(CacheKey, cachedAllProducts, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
            }

            return Ok(cachedAllProducts);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateProduct([FromBody] ProductRequestDto productRequestDto, CancellationToken cancellationToken)
        {
            var matchedProductType = await _typeRepository.GetByIdAsync(productRequestDto.ProductTypeId, cancellationToken);
            if (matchedProductType == null)
                return BadRequest("Girdiğiniz Id'ye sahip bir Product Type bulunamadı.");

            var matchedInvoice = await _invoiceRepository.GetByIdAsync(productRequestDto.InvoiceId, cancellationToken);
            if (matchedInvoice == null)
                return BadRequest("Geçersiz bir InvoiceId girdiniz.");

            var product = _mapper.Map<Product>(productRequestDto);

            await _productRepository.CreateAsync(product, cancellationToken);

            _cache.Remove(CacheKey);

            var result = _mapper.Map<ProductResponseDto>(product);
            return Created("", result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductRequestDto productRequestDto, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken);
            if (product == null)
                return NotFound();

            var matchedProductType = await _typeRepository.GetByIdAsync(productRequestDto.ProductTypeId, cancellationToken);
            if (matchedProductType == null)
                return BadRequest("Girdiğiniz Id'ye sahip bir Product Type bulunamadı.");

            var matchedInvoice = await _invoiceRepository.GetByIdAsync(productRequestDto.InvoiceId, cancellationToken);
            if (matchedInvoice == null)
                return BadRequest("Geçersiz bir InvoiceId girdiniz.");

            _mapper.Map(productRequestDto, product);

            await _productRepository.UpdateAsync(product, cancellationToken);

            _cache.Remove(CacheKey);

            return Ok($"{id} numaralı Id ile eşleşen Product güncellendi");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken);
            if (product == null)
                return NotFound($"Id = {id} bulunamadı.");

            await _productRepository.SoftDeleteAsync(product, cancellationToken);

            _cache.Remove(CacheKey);

            return Ok($"{id} numaralı Product başarıyla kaldırıldı.");
        }
    }
}
