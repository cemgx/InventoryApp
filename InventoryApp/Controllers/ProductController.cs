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
    public class ProductController : ControllerBase
    {
        private readonly IRepository<Product> productRepository;
        private readonly IRepository<ProductType> typeRepository;
        private readonly IMapper mapper;

        public ProductController(IRepository<Product> productRepository, IRepository<ProductType> typeRepository, IMapper mapper)
        {
            this.productRepository = productRepository;
            this.typeRepository = typeRepository;
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
            var product = await productRepository.GetByIdAsync(id);
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
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("İsim kısmı boş geçilemez.");
            }

            var products = await productRepository.GetByNameAsync(name);
            if (products == null || products.Count == 0)
            {
                return NotFound("Bu isme sahip bir ürün bulunamadı.");
            }

            var productDtos = mapper.Map<List<ProductDto>>(products);
            return Ok(productDtos);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDto productDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var matchedProductType = await typeRepository.GetByIdAsync(productDto.ProductTypeId);
            if (matchedProductType == null)
                return BadRequest("Girdiğiniz Id'ye sahip bir Product Type bulunamadı.");

            var product = mapper.Map<Product>(productDto);
            product.ProductTypeId = matchedProductType.Id;

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
            product.PurchasePrice = productDto.PurchasePrice;
            product.PurchaseDate = productDto.PurchaseDate;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await productRepository.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            await productRepository.RemoveAsync(product);

            return NoContent();
        }
    }
}
