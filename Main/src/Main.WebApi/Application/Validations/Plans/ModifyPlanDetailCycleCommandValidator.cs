using Main.WebApi.Application.Commands.Plans;

namespace Main.WebApi.Application.Validations.Plans;

public class ModifyPlanDetailCycleCommandValidator : AbstractValidator<ModifyPlanDetailCycleCommand>
{
    public ModifyPlanDetailCycleCommandValidator(ILogger<ModifyPlanDetailCycleCommandValidator> logger)
    {
        // PlanDetailId must not be empty and greater than 0
        RuleFor(x => x.PlanDetailId)
            .NotEmpty()
            .WithMessage("計畫明細 ID 不能為空")
            .GreaterThan(0)
            .WithMessage("計畫明細 ID 必須大於 0");

        // CycleType must be year, month or quarter
        RuleFor(x => x.CycleType)
            .NotEmpty()
            .WithMessage("週期類型不能為空")
            .Must(x => x is "year" or "month" or "quarter")
            .WithMessage("週期類型必須是 year、month 或 quarter");

        //// When CycleType is quarter, CycleMonth must exist
        //When(x => x.CycleType == "quarter", () =>
        //{
        //    RuleFor(x => x.CycleMonth)
        //        .NotEmpty().WithMessage("週期月份不能為空")
        //        .GreaterThan(0).WithMessage("週期月份必須大於 0");

        //    // When CycleMonthLast is false, CycleDay must in 1-31
        //    When(x => !x.CycleMonthLast, () =>
        //    {
        //        RuleFor(x => x.CycleDay)
        //            .NotNull().WithMessage("當 CycleMonthLast 為 true 時，CycleDay 不可為 null。")
        //            .InclusiveBetween(1, 31).WithMessage("當 CycleMonthLast 為 true 時，CycleDay 必須介於 1 到 31。");
        //    });

        //    // When CycleMonthLast is true, CycleDay must be null
        //    When(x => x.CycleMonthLast, () =>
        //    {
        //        RuleFor(x => x.CycleDay)
        //            .Null().WithMessage("當 CycleMonthLast 為 false 時，CycleDay 必須為 null。");
        //    });
        //});

        //// When CycleType is month
        //When(x => x.CycleType == "month", () =>
        //{
        //    // When CycleMonthLast is false, CycleDay must in 1-31
        //    When(x => !x.CycleMonthLast, () =>
        //    {
        //        RuleFor(x => x.CycleDay)
        //            .NotNull().WithMessage("當 CycleMonthLast 為 true 時，CycleDay 不可為 null。")
        //            .InclusiveBetween(1, 31).WithMessage("當 CycleMonthLast 為 true 時，CycleDay 必須介於 1 到 31。");
        //    });
        //    // When CycleMonthLast is true, CycleDay must be null
        //    When(x => x.CycleMonthLast, () =>
        //    {
        //        RuleFor(x => x.CycleDay)
        //            .Null().WithMessage("當 CycleMonthLast 為 false 時，CycleDay 必須為 null。");
        //    });
        //});

        // EndDatArray must not be empty
        RuleFor(x => x.PlanDocumentCycleArray)
            .NotEmpty()
            .WithMessage("結束日期陣列不能為空")
            .Must(x => x.Count > 0)
            .WithMessage("結束日期陣列必須大於 0");

        if (logger.IsEnabled(LogLevel.Trace))
        {
            logger.LogTrace("{ClassName} initialized", GetType().Name);
        }
    }
}
