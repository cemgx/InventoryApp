using AutoMapper;
using InventoryApp.Application.Dto;
using InventoryApp.Models.Entity;

namespace InventoryApp.Application.Mappings
{
    public class ProductTypeProfile : Profile
    {
        public ProductTypeProfile()
        {
            this.CreateMap<ProductType, ProductTypeResponseDto>().ReverseMap();

            this.CreateMap<ProductType, ProductTypeRequestDto>().ReverseMap();
        }
    }
}