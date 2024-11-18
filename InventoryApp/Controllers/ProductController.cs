using AutoMapper;
using InventoryApp.Application.Dto;
using InventoryApp.Application.Interfaces;
using InventoryApp.Models.Entity;
using Microsoft.AspNetCore.Mvc;

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
            List<ProductDto> productDto = mapper.Map<List<ProductDto>>(types.OrderBy(x => x.Name));
            return Ok(productDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await productRepository.GetByProductIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var productDto = mapper.Map<ProductDto>(product);
            return Ok(productDto);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string name)
        {
            var products = await productRepository.GetByNameAsync(name);
            if (products == null || products.Count == 0)
            {
                return NotFound("Bu isme sahip bir ürün bulunamadı.");
            }

            var productDtos = mapper.Map<List<ProductDto>>(products);
            return Ok(productDtos);
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

            var productsDto = mapper.Map<List<ProductDto>>(products);

            return Ok(productsDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDto productDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var matchedProductType = await typeRepository.GetByIdAsync(productDto.ProductTypeId);
            if (matchedProductType == null)
                return BadRequest("Girdiğiniz Id'ye sahip bir Product Type bulunamadı.");

            var matchedInvoice = await invoiceRepository.GetByIdAsync(productDto.InvoiceId);
            if (matchedInvoice == null)
                return BadRequest("Geçersiz bir InvoiceId girdiniz.");

            var product = mapper.Map<Product>(productDto);
            product.ProductTypeId = matchedProductType.Id;
            product.InvoiceId = matchedInvoice.Id;

            await productRepository.CreateAsync(product);

            return Created("", mapper.Map<ProductDto>(product));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDto productDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = await productRepository.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            product.Name = productDto.Name;
            product.ProductTypeId = productDto.ProductTypeId;
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
