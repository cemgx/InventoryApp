using AutoMapper;
using InventoryApp.Application.Dto;
using InventoryApp.Models.Entity;

namespace InventoryApp.Application.Mappings
{
    public class InventoryProfile : Profile
    {
        public InventoryProfile()
        {
            this.CreateMap<Inventory, InventoryResponseDto>();

            this.CreateMap<InventoryRequestDto, Inventory>();
        }
    }
}