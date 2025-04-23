using FinanceTracker.Domain.Models.DTOs.PageDto;
using FluentValidation;

namespace FinanceTracker.Presentation.Validators
{
    public class TransactionQueryValidator : AbstractValidator<TransactionQueryDto>
    {
        public TransactionQueryValidator()
        {
            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("Sahifa hajmi 1-100 oralig'ida bo'lishi kerak");

            RuleFor(x => x.SortBy)
                .Must(BeValidSortProperty)
                .WithMessage("Noto'g'ri tartib parametri");
        }

        private static bool BeValidSortProperty(string sortBy)
        {
            if (string.IsNullOrEmpty(sortBy)) return true;

            var allowed = new[] { "date", "amount", "category", "account" };
            return allowed.Contains(sortBy.ToLower());
        }
    }
}
