using AutoMapper;
using FinanceTracker.Domain.Models;
using FinanceTracker.Domain.Models.DTOs;
using FinanceTracker.Domain.Models.DTOs.BudgetDtos;
using FinanceTracker.Domain.Models.DTOs.RecurringTransactionDtos;
using FinanceTracker.Domain.Models.DTOs.TransactionDtos;

namespace FinanceTracker.Presentation.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Budget
            CreateMap<Budget, BudgetDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ReverseMap()
                .ForMember(dest => dest.Category, opt => opt.Ignore());

            CreateMap<BudgetCreateDto, Budget>();
            CreateMap<BudgetUpdateDto, Budget>();

            // RecurringTransaction
            CreateMap<RecurringTransaction, RecurringTransactionDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.AccountName, opt => opt.MapFrom(src => src.Account.Name));

            CreateMap<RecurringTransactionCreateDto, RecurringTransaction>();
            CreateMap<RecurringTransactionUpdateDto, RecurringTransaction>();

            // Transaction
            CreateMap<Transaction, TransactionDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.AccountName, opt => opt.MapFrom(src => src.Account.Name));

            CreateMap<TransactionCreateDto, Transaction>();
        }
    }
}
