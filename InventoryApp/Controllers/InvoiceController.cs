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
            var types = await repository.GetAllAsync();
            List<InvoiceResponseDto> invoiceResponseDto = mapper.Map<List<InvoiceResponseDto>>(types);
            return Ok(invoiceResponseDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoice(int id)
        {
            var invoice = await repository.GetByInvoiceIdAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }

            var invoiceResponseDto = mapper.Map<InvoiceResponseDto>(invoice);
            return Ok(invoiceResponseDto);
        } //InvoiceController'ımın içerisinde Invoice Id'ye göre invoice'ların sahip oldukları product id'lerini görmek istiyorum. bunun için gerekli kodu yaz. 

        [HttpGet("Firma Adı")]
        public async Task<IActionResult> GetInvoicesByFirm([FromQuery] string name)
        {
            var invoices = await repository.GetByFirmNameAsync(name);
            if (invoices.Count == 0)
            {
                return NotFound("Bu isme sahip firma yok.");
            }

            List<InvoiceResponseDto> invoicesResponseDto = mapper.Map<List<InvoiceResponseDto>>(invoices);

            return Ok(invoicesResponseDto);
        }

        [HttpGet("{invoiceId}/products")]
        public async Task<IActionResult> GetProductsByInvoiceId(int invoiceId)
        {
            var productIds = await productRepository.GetProductIdsByInvoiceIdAsync(invoiceId);

            if (!productIds.IsNullOrEmpty())
            {
                return NotFound($"{invoiceId} id numaralı fatura ile ilişkilendirilmiş bir ürün bulunamadı.");
            }

            return Ok(productIds);
        }


        [HttpPost]
        public async Task<IActionResult> CreateInvoice([FromBody] InvoiceRequestDto invoiceRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var invoice = mapper.Map<Invoice>(invoiceRequestDto);

            await repository.CreateAsync(invoice);

            var createdInvoiceDto = mapper.Map<InvoiceRequestDto>(invoice);
            return Created("", createdInvoiceDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInvoice(int id, [FromBody] InvoiceRequestDto invoiceRequestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var invoice = await repository.GetByIdAsync(id);
            if (invoice == null)
                return NotFound("{id} numaralı Id ile eşleşen bir Invoice bulunamadı.");

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

            await repository.RemoveAsync(invoice);

            return NoContent();
        }
    }
}