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
    }
}