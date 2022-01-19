using FluentValidation;
using Travel.Application.Common.Interfaces;
using Travel.Application.TourLists.Commands.UpdateTourList;

namespace Travel.Application.TourLists.Commands.CreateTourList;

public class UpdateTourListCommandValidator:AbstractValidator<UpdateTourListCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateTourListCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.City)
            .NotEmpty().WithMessage("City is required.")
            .MaximumLength(90).WithMessage("City must not exceed 90 character.");

        RuleFor(v => v.About)
            .NotEmpty().WithMessage("About is required.");

        RuleFor(v => v.Country)
            .NotEmpty().WithMessage("Country is required.")
            .MaximumLength(60).WithMessage("Country must not exceed 60 character.");
    }
}