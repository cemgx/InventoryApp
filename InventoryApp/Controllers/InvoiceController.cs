using AutoMapper;
using InventoryApp.Application.Dto;
using InventoryApp.Application.Interfaces;
using InventoryApp.Models.Entity;
using InventoryApp.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace InventoryApp.Controllers
{
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
        public async Task<IActionResult> GetInvoices()
        {
            var invoices = await repository.GetAllAsync();

            var result = mapper.Map<List<InvoiceResponseDto>>(invoices);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoice(int id)
        {
            var invoice = await repository.GetByInvoiceIdAsync(id);
            if (invoice.IsNullOrEmpty())
            {
                return NotFound();
            }

            var result = mapper.Map<List<InvoiceResponseDto>>(invoice);
            return Ok(result);
        }

        [HttpGet("Firma Adı")]
        public async Task<IActionResult> GetInvoicesByFirm([FromQuery] string name)
        {
            var invoices = await repository.GetByFirmNameAsync(name);
            if (invoices.IsNullOrEmpty())
            {
                return NotFound("Bu isme sahip firma yok.");
            }

            var result = mapper.Map<List<InvoiceResponseDto>>(invoices);

            return Ok(result);
        }

        [HttpGet("{invoiceId}/products")]
        public async Task<IActionResult> GetProductsByInvoiceId(int invoiceId)
        {
            var productIds = await productRepository.GetProductIdsByInvoiceIdAsync(invoiceId);

            if (productIds.IsNullOrEmpty())
            {
                return NotFound($"{invoiceId} id numaralı fatura ile ilişkilendirilmiş bir ürün bulunamadı.");
            }

            return Ok(productIds);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllInvoices()
        {
            var invoices = await repository.GetAllIncludingDeletedAsync();

            var result = mapper.Map<List<InvoiceResponseDto>>(invoices);
            return Ok(result);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateInvoice([FromBody] InvoiceRequestDto invoiceRequestDto)
        {
            var invoice = mapper.Map<Invoice>(invoiceRequestDto);

            await repository.CreateAsync(invoice);

            var result = mapper.Map<InvoiceRequestDto>(invoice);
            return Created("", result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateInvoice(int id, [FromBody] InvoiceRequestDto invoiceRequestDto)
        {
            var invoice = await repository.GetByIdAsync(id);
            if (invoice == null)
                return NotFound("{id} numaralı Invoice bulunamadı.");

            mapper.Map(invoiceRequestDto, invoice);

            await repository.UpdateAsync(invoice);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var invoice = await repository.GetByIdAsync(id);
            if (invoice == null)
                return NotFound();

            await repository.SoftDeleteAsync(invoice);

            return Ok($"{id} numaralı Invoice başarıyla kaldırıldı.");
        }
    }
}