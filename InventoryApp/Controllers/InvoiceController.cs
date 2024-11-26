using AutoMapper;
using InventoryApp.Application.Dto;
using InventoryApp.Application.Interfaces;
using InventoryApp.Application.Utility;
using InventoryApp.Models.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace InventoryApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceRepository repository;
        private readonly IProductRepository productRepository;
        private readonly IMapper mapper;

        public InvoiceController(IInvoiceRepository repository, IMapper mapper, IProductRepository productRepository)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.productRepository = productRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetInvoices(CancellationToken cancellationToken)
        {
            var invoices = await repository.GetAllAsync(cancellationToken);

            var result = mapper.Map<List<InvoiceResponseDto>>(invoices);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoice(int id, CancellationToken cancellationToken)
        {
            var invoice = await repository.GetByInvoiceIdAsync(id, cancellationToken);
            if (invoice.IsNullOrEmpty())
            {
                return NotFound();
            }

            var result = mapper.Map<List<InvoiceResponseDto>>(invoice);
            return Ok(result);
        }

        [HttpGet("Firma Adı")]
        public async Task<IActionResult> GetInvoicesByFirm([FromQuery] string name, CancellationToken cancellationToken)
        {
            name = AntiXssUtility.EncodeDto(name);

            var invoices = await repository.GetByFirmNameAsync(name, cancellationToken);
            if (invoices.IsNullOrEmpty())
            {
                return NotFound($"{name} ismine sahip firma bulunamadı.");
            }

            var result = mapper.Map<List<InvoiceResponseDto>>(invoices);

            return Ok(result);
        }

        [HttpGet("{invoiceId}/products")]
        public async Task<IActionResult> GetProductsByInvoiceId(int invoiceId, CancellationToken cancellationToken)
        {
            var productIds = await productRepository.GetProductIdsByInvoiceIdAsync(invoiceId, cancellationToken);

            if (productIds.IsNullOrEmpty())
            {
                return NotFound($"{invoiceId} id numaralı fatura ile ilişkilendirilmiş bir ürün bulunamadı.");
            }

            return Ok(productIds);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllInvoices(CancellationToken cancellationToken)
        {
            var invoices = await repository.GetAllIncludingDeletedAsync(cancellationToken);

            var result = mapper.Map<List<InvoiceResponseDto>>(invoices);
            return Ok(result);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateInvoice([FromBody] InvoiceRequestDto invoiceRequestDto, CancellationToken cancellationToken)
        {
            invoiceRequestDto = AntiXssUtility.EncodeDto(invoiceRequestDto);

            var invoice = mapper.Map<Invoice>(invoiceRequestDto);

            await repository.CreateAsync(invoice, cancellationToken);

            var result = mapper.Map<InvoiceResponseDto>(invoice);
            return Created("", result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateInvoice(int id, [FromBody] InvoiceRequestDto invoiceRequestDto, CancellationToken cancellationToken)
        {
            invoiceRequestDto = AntiXssUtility.EncodeDto(invoiceRequestDto);

            var invoice = await repository.GetByIdAsync(id, cancellationToken);
            if (invoice == null)
                return NotFound($"{id} numaralı Invoice bulunamadı.");

            mapper.Map(invoiceRequestDto, invoice);

            await repository.UpdateAsync(invoice, cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id, CancellationToken cancellationToken)
        {
            var invoice = await repository.GetByIdAsync(id, cancellationToken);
            if (invoice == null)
                return NotFound();

            await repository.SoftDeleteAsync(invoice, cancellationToken);

            return Ok($"{id} numaralı Invoice başarıyla kaldırıldı.");
        }
    }
}