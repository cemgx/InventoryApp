using AutoMapper;
using InventoryApp.Application.Dto;
using InventoryApp.Models.Entity;

namespace InventoryApp.Application.Mappings
{
    public class InventoryProfile : Profile
    {
        public InventoryProfile()
        {
            this.CreateMap<Inventory, InventoryResponseDto>().ReverseMap();

            this.CreateMap<Inventory, InventoryRequestDto>().ReverseMap();
        }
    }
}