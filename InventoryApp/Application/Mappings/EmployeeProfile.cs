using AutoMapper;
using InventoryApp.Application.Dto;
using InventoryApp.Application.LogEntities;
using InventoryApp.Models.Entity;

namespace InventoryApp.Application.Mappings
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            CreateMap<Employee, EmployeeResponseDto>().ReverseMap();

            CreateMap<EmployeeRequestDto, Employee>();

            CreateMap<EmployeeRequestDto, EmployeeLog>();
        }
    }
}