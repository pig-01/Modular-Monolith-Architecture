using Main.Dto.ViewModel.Plan;

namespace Main.Repository.AggregatesModel.PlanAggregate;

public class PlanDetailMappings : Profile
{
    public PlanDetailMappings()
    {
        //這個方法讓我們可以將ViewPlanDetail的名稱依照不同語言顯示
        CreateMap<ViewPlanDetail, ViewPlanDetail>()
            .ForMember(dest => dest.I18nPlanDetailName, opt => opt.MapFrom((src, dest, destMember, context) =>
            {
                string language = context.Items.TryGetValue("Language", out object? languageObj) ? languageObj?.ToString() ?? "zh-CHT" : "zh-CHT";
                return language switch
                {
                    "zh-CHS" => src.PlanDetailChName ?? src.PlanDetailName,
                    "en-US" => src.PlanDetailEnName ?? src.PlanDetailName,
                    "jp-JP" => src.PlanDetailJpName ?? src.PlanDetailName,
                    _ => src.PlanDetailName
                };
            }));
    }
}
