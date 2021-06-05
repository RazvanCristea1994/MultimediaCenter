using FluentValidation;
using MultimediaCenter.Data;
using MultimediaCenter.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultimediaCenter.Validators
{
    public class MovieValidator : AbstractValidator<MovieViewModel>
    {
        private readonly ApplicationDbContext _context;

        public MovieValidator(ApplicationDbContext context)
        {
            _context = context;

            RuleFor(m => m.Title).MinimumLength(1);
            RuleFor(m => m.Description).MinimumLength(5);
            RuleFor(m => m.YearOfRelease).Must(BeAValidYearOfRelease).WithMessage("The year of release cannot be in the future.");
            RuleFor(m => m.Duration).GreaterThan(1);
            RuleFor(m => m.Rating).Null().When(m => m.Watched == false).WithMessage("You cannot rate a movie you haven't watched yet.");
            RuleFor(m => m.Rating).InclusiveBetween(1, 5).When(m => m.Watched == true);
            RuleFor(m => m.Director).MinimumLength(1);
            RuleFor(m => m.Genre).NotNull();
            RuleFor(m => m.AddedDate).Must(BeAddedInThePast).WithMessage("The movie cannot be added in the future.");
        }

        private bool BeAValidYearOfRelease(int releaseYear)
        {
            int currentYear = DateTime.Now.Year;
            return releaseYear <= currentYear;
        }

        private bool BeAddedInThePast(DateTime addedDate)
        {
            var currentDate = DateTime.Now;
            return addedDate <= currentDate;
        }
    }
}
