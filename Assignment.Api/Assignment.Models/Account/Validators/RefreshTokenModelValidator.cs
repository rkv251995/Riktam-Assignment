using FluentValidation;

namespace Assignment.Models.Account.Validators
{
    public class RefreshTokenModelValidator : AbstractValidator<RefreshTokenModel>
    {
        public RefreshTokenModelValidator()
        {
            RuleFor(r => r.AccessToken).NotNull().NotEmpty()
                                       .WithErrorCode("003")
                                       .WithMessage("AccessToken is required.");

            RuleFor(r => r.RefreshToken).NotNull()
                                        .NotEmpty()
                                        .WithErrorCode("004")
                                        .WithMessage("RefreshToken is required.");
        }
    }
}
