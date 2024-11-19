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
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository productRepository;
        private readonly IProductTypeRepository typeRepository;
        private readonly IInvoiceRepository invoiceRepository;
        private readonly IMapper mapper;

        public ProductController(IProductRepository productRepository, IProductTypeRepository typeRepository, IInvoiceRepository invoiceRepository, IMapper mapper)
        {
            this.productRepository = productRepository;
            this.typeRepository = typeRepository;
            this.invoiceRepository = invoiceRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var types = await productRepository.GetAllAsync();
            List<ProductResponseDto> productResponseDto = mapper.Map<List<ProductResponseDto>>(types.OrderBy(x => x.Name));
            return Ok(productResponseDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await productRepository.GetByProductIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var productResponseDto = mapper.Map<List<ProductResponseDto>>(product);
            return Ok(productResponseDto);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string name)
        {
            var products = await productRepository.GetByNameAsync(name);
            if (products.IsNullOrEmpty())
            {
                return NotFound("Bu isme sahip bir ürün bulunamadı.");
            }

            var productResponseDtos = mapper.Map<List<ProductResponseDto>>(products);
            return Ok(productResponseDtos);
        }

        [HttpGet("purchaseDateGet")]
        public async Task<IActionResult> GetProductsByInvoicePurchaseDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
            {
                return BadRequest("Başlangıç tarihi, bitiş tarihinden büyük olamaz.");
            }

            var products = await productRepository.GetByInvoicePurchaseDateAsync(startDate, endDate);

            if (products.Count == 0)
            {
                return NotFound($"Belirtilen {startDate:yyyy-MM-dd} ve {endDate:yyyy-MM-dd} tarihleri arasında ürün bulunamadı.");
            }

            var productsResponseDto = mapper.Map<List<ProductResponseDto>>(products);

            return Ok(productsResponseDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductRequestDto productRequestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var matchedProductType = await typeRepository.GetByIdAsync(productRequestDto.ProductTypeId);
            if (matchedProductType == null)
                return BadRequest("Girdiğiniz Id'ye sahip bir Product Type bulunamadı.");

            var matchedInvoice = await invoiceRepository.GetByIdAsync(productRequestDto.InvoiceId);
            if (matchedInvoice == null)
                return BadRequest("Geçersiz bir InvoiceId girdiniz.");

            var product = mapper.Map<Product>(productRequestDto);

            await productRepository.CreateAsync(product);
            var createdProductDto = mapper.Map<ProductRequestDto>(product);

            return Created("", createdProductDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductRequestDto productRequestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = await productRepository.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            var matchedProductType = await typeRepository.GetByIdAsync(productRequestDto.ProductTypeId);
            if (matchedProductType == null)
                return BadRequest("Girdiğiniz Id'ye sahip bir Product Type bulunamadı.");

            var matchedInvoice = await invoiceRepository.GetByIdAsync(productRequestDto.InvoiceId);
            if (matchedInvoice == null)
                return BadRequest("Geçersiz bir InvoiceId girdiniz.");

            mapper.Map(productRequestDto, product);

            await productRepository.UpdateAsync(product);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await productRepository.GetByIdAsync(id);
            if (product == null)
                return NotFound($"Id = {id} bulunamadı.");

            await productRepository.RemoveAsync(product);

            return NoContent();
        }
    }
}
