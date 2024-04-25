namespace Modules.Shared.Application;
using Common.Services;
using FluentValidation;

public class SharedModuleConfiguration
{
    public MailConfiguration Mail { get; set; }

    public class Validator : AbstractValidator<SharedModuleConfiguration>
    {
        public Validator()
        {
            this.RuleFor(p => p.Mail).NotNull()
                .SetValidator(new MailConfiguration.Validator());
        }
    }
}