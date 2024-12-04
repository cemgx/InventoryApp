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
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceRepository _repository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        private const string CacheKey = "Employees_List";

        public InvoiceController(IInvoiceRepository repository, IMapper mapper, IProductRepository productRepository, IMemoryCache cache)
        {
            _repository = repository;
            _mapper = mapper;
            _productRepository = productRepository;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> GetInvoices(CancellationToken cancellationToken)
        {
            if (!_cache.TryGetValue(CacheKey, out List<InvoiceResponseDto> cachedInvoices))
            {
                var invoices = await _repository.GetAllAsync(cancellationToken);
                if (invoices.IsNullOrEmpty())
                {
                    return NotFound("Hiç invoice yok");
                }

                cachedInvoices = _mapper.Map<List<InvoiceResponseDto>>(invoices);

                _cache.Set(CacheKey, cachedInvoices, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
            }
            return Ok(cachedInvoices);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoice(int id, CancellationToken cancellationToken)
        {
            var invoice = await _repository.GetByInvoiceIdAsync(id, cancellationToken);
            if (invoice.IsNullOrEmpty())
            {
                return NotFound();
            }

            var result = _mapper.Map<List<InvoiceResponseDto>>(invoice);
            return Ok(result);
        }

        [HttpGet("Firma Adı")]
        public async Task<IActionResult> GetInvoicesByFirm([FromQuery] string name, CancellationToken cancellationToken)
        {
            if (!_cache.TryGetValue(CacheKey, out List<InvoiceResponseDto> cachedInvoicesByFirm))
            {
                var invoices = await _repository.GetByFirmNameAsync(name, cancellationToken);
                if (invoices.IsNullOrEmpty())
                {
                    return NotFound($"{name} ismine sahip firma bulunamadı.");
                }

                cachedInvoicesByFirm = _mapper.Map<List<InvoiceResponseDto>>(invoices);

                _cache.Set(CacheKey, cachedInvoicesByFirm, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
            }

            return Ok(cachedInvoicesByFirm);
        }

        [HttpGet("{invoiceId}/products")]
        public async Task<IActionResult> GetProductsByInvoiceId(int invoiceId, CancellationToken cancellationToken)
        {
            var productIds = await _productRepository.GetProductIdsByInvoiceIdAsync(invoiceId, cancellationToken);

            if (productIds.IsNullOrEmpty())
            {
                return NotFound($"{invoiceId} id numaralı fatura ile ilişkilendirilmiş bir ürün bulunamadı.");
            }

            return Ok(productIds);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllInvoices(CancellationToken cancellationToken)
        {
            if (!_cache.TryGetValue(CacheKey, out List<InvoiceResponseDto> cachedAllInvoices))
            {
                var invoices = await _repository.GetAllIncludingDeletedAsync(cancellationToken);

                cachedAllInvoices = _mapper.Map<List<InvoiceResponseDto>>(invoices);

                _cache.Set(CacheKey, cachedAllInvoices, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
            }

            return Ok(cachedAllInvoices);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateInvoice([FromBody] InvoiceRequestDto invoiceRequestDto, CancellationToken cancellationToken)
        {
            var invoice = _mapper.Map<Invoice>(invoiceRequestDto);

            await _repository.CreateAsync(invoice, cancellationToken);

            _cache.Remove(CacheKey);

            var result = _mapper.Map<InvoiceResponseDto>(invoice);
            return Created("", result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateInvoice(int id, [FromBody] InvoiceRequestDto invoiceRequestDto, CancellationToken cancellationToken)
        {
            var invoice = await _repository.GetByIdAsync(id, cancellationToken);
            if (invoice == null)
                return NotFound($"{id} numaralı Invoice bulunamadı.");

            _mapper.Map(invoiceRequestDto, invoice);

            await _repository.UpdateAsync(invoice, cancellationToken);

            _cache.Remove(CacheKey);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id, CancellationToken cancellationToken)
        {
            var invoice = await _repository.GetByIdAsync(id, cancellationToken);
            if (invoice == null)
                return NotFound();

            await _repository.SoftDeleteAsync(invoice, cancellationToken);

            _cache.Remove(CacheKey);

            return Ok($"{id} numaralı Invoice başarıyla kaldırıldı.");
        }
    }
}