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
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceRepository repository;
        private readonly IMapper mapper;

        public InvoiceController(IInvoiceRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetInvoices()
        {
            var types = await repository.GetAllAsync();
            List<InvoiceDto> invoiceDto = mapper.Map<List<InvoiceDto>>(types);
            return Ok(invoiceDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoice(int id)
        {
            var invoice = await repository.GetByIdAsync(id);

            if (invoice == null)
            {
                return NotFound();
            }

            var invoiceDto = mapper.Map<InvoiceDto>(invoice);
            return Ok(invoiceDto);
        }

        [HttpGet("Firma Adı")]
        public async Task<IActionResult> GetInvoicesByFirm([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Ad kısmı boş geçilemez.");
            }

            var invoices = await repository.GetByFirmNameAsync(name);

            if (invoices.Count == 0)
            {
                return NotFound("Bu isme sahip fatura yok.");
            }

            List<InvoiceDto> invoicesDto = mapper.Map<List<InvoiceDto>>(invoices);

            return Ok(invoicesDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateInvoice([FromBody] InvoiceDto invoiceDto)
        {
            if (invoiceDto == null)
            {
                return BadRequest("Invoice boş bırakılamaz.");
            }

            var invoice = mapper.Map<Invoice>(invoiceDto);

            await repository.CreateAsync(invoice);

            var createdInvoiceDto = mapper.Map<InvoiceDto>(invoice);
            return Created("", createdInvoiceDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInvoice(int id, [FromBody] InvoiceDto invoiceDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = await repository.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            product.InvoiceNo = invoiceDto.InvoiceNo;
            product.FirmName = invoiceDto.FirmName;
            product.Price = invoiceDto.Price;
            product.PurchaseDate = invoiceDto.PurchaseDate;
            await repository.UpdateAsync(product);

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