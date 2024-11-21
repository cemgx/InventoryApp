using AutoMapper;
using InventoryApp.Application.Dto;
using InventoryApp.Models.Entity;

namespace InventoryApp.Application.Mappings
{
    public class InvoiceProfile : Profile
    {
        public InvoiceProfile() 
        {
            this.CreateMap<Invoice, InvoiceResponseDto>();

            this.CreateMap<InvoiceRequestDto, Invoice>();
        }
    }
}