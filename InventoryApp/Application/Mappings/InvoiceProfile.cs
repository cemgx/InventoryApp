using AutoMapper;
using InventoryApp.Application.Dto;
using InventoryApp.Models.Entity;

namespace InventoryApp.Application.Mappings
{
    public class InvoiceProfile : Profile
    {
        public InvoiceProfile() 
        {
            this.CreateMap<Invoice, InvoiceResponseDto>().ReverseMap();

            this.CreateMap<Invoice, InvoiceRequestDto>().ReverseMap();
        }
    }
}