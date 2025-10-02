using FluentValidation;
using Scheduler.Application.Commands.Mails;

namespace Scheduler.Application.Validations.Mails;

public class InsertDatabaseMailValidator : AbstractValidator<InsertDatabaseMailCommand>
{
    public InsertDatabaseMailValidator()
    {
        RuleFor(x => x.Recipients)
            .NotEmpty().WithMessage("收件者不可為空")
            .Must(recipients => recipients.All(r => !string.IsNullOrWhiteSpace(r))).WithMessage("收件者列表中不可包含空值");

        RuleFor(x => x.Subject)
            .NotEmpty().WithMessage("主旨不可為空")
            .MaximumLength(200).WithMessage("主旨長度不可超過200字元");

        RuleFor(x => x.Body)
            .NotEmpty().WithMessage("郵件內容不可為空");
    }
}
