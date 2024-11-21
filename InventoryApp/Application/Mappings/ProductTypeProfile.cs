using AutoMapper;
using InventoryApp.Application.Dto;
using InventoryApp.Models.Entity;

namespace InventoryApp.Application.Mappings
{
    public class ProductTypeProfile : Profile
    {
        public ProductTypeProfile()
        {
            this.CreateMap<ProductType, ProductTypeResponseDto>();

            this.CreateMap<ProductTypeRequestDto, ProductType>();
        }
    }
}