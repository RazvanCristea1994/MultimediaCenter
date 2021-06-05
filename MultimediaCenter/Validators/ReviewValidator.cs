using FluentValidation;
using MultimediaCenter.Data;
using MultimediaCenter.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultimediaCenter.Validators
{
    public class ReviewValidator : AbstractValidator<ReviewViewModel>
    {
        private readonly ApplicationDbContext _context;

        public ReviewValidator(ApplicationDbContext context)
        {
            _context = context;

            RuleFor(r => r.Id).NotNull();
            RuleFor(r => r.Content).MinimumLength(5);
            RuleFor(r => r.Stars).InclusiveBetween(1, 5);
            RuleFor(r => r.DateTime).Must(BeAddedInThePast).WithMessage("The review cannot be added in the future.");
        }

        private bool BeAddedInThePast(DateTime addedDate)
        {
            var currentDate = DateTime.Now;
            return addedDate <= currentDate;
        }
    }
}
