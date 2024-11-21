using AutoMapper;
using InventoryApp.Application.Dto;
using InventoryApp.Models.Entity;

namespace InventoryApp.Application.Mappings
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            this.CreateMap<Employee, EmployeeResponseDto>();

            this.CreateMap<EmployeeRequestDto, Employee>();
        }
    }
}