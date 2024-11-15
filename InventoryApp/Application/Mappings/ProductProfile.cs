using AutoMapper;
using InventoryApp.Application.Dto;
using InventoryApp.Models.Entity;

namespace InventoryApp.Application.Mappings
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            this.CreateMap<Product, ProductDto>().ReverseMap();
        }
    }
}