namespace Common.Services;

using FluentValidation;

public class MailConfiguration
{
    public bool Enabled { get; set; } = true;

    public string From { get; set; }

    public string Host { get; set; }

    public int Port { get; set; } = 587;

    public string UserName { get; set; }

    public string Password { get; set; }

    public string DisplayName { get; set; }

    public class Validator : AbstractValidator<MailConfiguration>
    {
        public Validator()
        {
            this.RuleFor(c => c.Host)
                .NotNull().NotEmpty()
                .WithMessage("Host cannot be null or empty");

            this.RuleFor(c => c.Port)
                .GreaterThan(0)
                .WithMessage("Port cannot be null or empty");
        }
    }
}