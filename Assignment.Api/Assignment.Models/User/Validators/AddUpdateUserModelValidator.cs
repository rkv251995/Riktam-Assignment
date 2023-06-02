using FluentValidation;
using System.Globalization;

namespace Assignment.Models.User.Validators
{
    public class AddUpdateUserModelValidator : AbstractValidator<AddUpdateUserModel>
    {
        public AddUpdateUserModelValidator()
        {
            RuleFor(r => r.Email).NotNull()
                                 .NotEmpty()
                                 .WithErrorCode("007")
                                 .WithMessage("Email is required.")
                                 .EmailAddress()
                                 .WithMessage("Invalid Email.");

            RuleFor(r => r.FirstName).NotNull()
                                     .NotEmpty()
                                     .WithErrorCode("008")
                                     .WithMessage("FirstName is required.");

            RuleFor(r => r.LastName).NotNull()
                                    .NotEmpty()
                                    .WithErrorCode("009")
                                    .WithMessage("LastName is required.");

            RuleFor(r => r.Username).NotNull()
                                    .NotEmpty()
                                    .WithErrorCode("010")
                                    .WithMessage("Username is required.");

            RuleFor(r => r.Password).NotNull()
                                    .NotEmpty()
                                    .WithErrorCode("011")
                                    .WithMessage("Password is required.");

            RuleFor(r => r.Mobile).NotNull()
                                  .NotEmpty()
                                  .WithErrorCode("012")
                                  .WithMessage("Mobile is required.")
                                  .MaximumLength(10)
                                  .WithMessage("Mobile should not be greater than 10 digits.")
                                  .MinimumLength(10)
                                  .WithMessage("Mobile should not be less than 10 digits.");

            RuleFor(r => r.DateOfBirth).NotNull()
                                       .NotEmpty()
                                       .WithErrorCode("013")
                                       .WithMessage("DateOfBirth is required.")
                                       .Must(m => DateTime.TryParseExact(m, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
                                       .WithMessage("Invalid DateOfBirth Format. Please provide in this format dd-MM-yyyy.");

            RuleFor(r => r.Address).NotNull()
                                   .NotEmpty()
                                   .WithErrorCode("014")
                                   .WithMessage("Address is required.");

            RuleFor(r => r.City).NotNull()
                                .NotEmpty()
                                .WithErrorCode("015")
                                .WithMessage("City is required.");

            RuleFor(r => r.State).NotNull()
                                 .NotEmpty()
                                 .WithErrorCode("016")
                                 .WithMessage("State is required.");

            RuleFor(r => r.Country).NotNull()
                                   .NotEmpty()
                                   .WithErrorCode("017")
                                   .WithMessage("Country is required.");
        }
    }
}
