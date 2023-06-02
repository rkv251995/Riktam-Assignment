using FluentValidation;

namespace Assignment.Models.Account.Validators
{
    public class LoginModelValidator : AbstractValidator<LoginModel>
    {
        public LoginModelValidator()
        {
            RuleFor(r => r.Email).NotNull()
                                 .NotEmpty()
                                 .WithErrorCode("001")
                                 .WithMessage("Email is required.")
                                 .EmailAddress()
                                 .WithMessage("Invalid Email.");

            RuleFor(r => r.Password).NotNull()
                                    .NotEmpty()
                                    .WithErrorCode("002")
                                    .WithMessage("Password is required.");
        }
    }
}
