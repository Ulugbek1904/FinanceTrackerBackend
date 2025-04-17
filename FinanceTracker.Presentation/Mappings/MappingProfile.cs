using AutoMapper;
using FinanceTracker.Domain.Models;
using FinanceTracker.Domain.Models.DTOs;

namespace FinanceTracker.Presentation.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Budget, BudgetDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

            CreateMap<BudgetCreateDto, Budget>();
            CreateMap<BudgetUpdateDto, Budget>();
        }
    }
}
