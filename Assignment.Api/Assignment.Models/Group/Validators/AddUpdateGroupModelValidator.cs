using FluentValidation;

namespace Assignment.Models.Group.Validators
{
    public class AddUpdateGroupModelValidator : AbstractValidator<AddUpdateGroupModel>
    {
        public AddUpdateGroupModelValidator()
        {
            RuleFor(r => r.Name).NotNull()
                                .NotEmpty()
                                .WithErrorCode("005")
                                .WithMessage("Name is required.");

            RuleFor(r => r.Description).NotNull()
                                       .NotEmpty()
                                       .WithErrorCode("006")
                                       .WithMessage("Description is required.");
        }
    }
}
