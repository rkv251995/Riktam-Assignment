using FluentValidation;

namespace Assignment.Models.Message.Validators
{
    public class MessageModelValidator : AbstractValidator<MessageModel>
    {
        public MessageModelValidator()
        {
            RuleFor(r => r.Message).NotNull()
                                   .NotEmpty()
                                   .WithErrorCode("007")
                                   .WithMessage("Message is required.");
        }
    }
}
