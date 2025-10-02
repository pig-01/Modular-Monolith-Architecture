using Main.Domain.AggregatesModel.PlanTemplateAggregate;
using Main.Dto.ViewModel.PlanTemplate;

namespace Main.Repository.AggregatesModel.PlanTemplateAggregate;

public class PlanTemplateMappings : Profile
{
    public PlanTemplateMappings()
    {
        CreateMap<PlanTemplate, ViewPlanTemplate>()
            .ForMember(dest => dest.RequestUnits, opt => opt.MapFrom(src => src.PlanTemplateRequestUnits.Select(x => x.UnitCode)))
            .ForMember(dest => dest.FormId, opt => opt.MapFrom(src => src.PlanTemplateForms.First().FormId))
            .ForMember(dest => dest.I18nPlanTemplateName, opt => opt.MapFrom((src, dest, destMember, context) =>
            {
                string language = context.Items.TryGetValue("Language", out object? languageObj) ? languageObj?.ToString() ?? "zh-CHT" : "zh-CHT";
                return language switch
                {
                    "zh-CHS" => src.PlanTemplateChName ?? src.PlanTemplateName,
                    "en-US" => src.PlanTemplateEnName ?? src.PlanTemplateName,
                    "jp-JP" => src.PlanTemplateJpName ?? src.PlanTemplateName,
                    _ => src.PlanTemplateName
                };
            }));

        CreateMap<PlanTemplateDetail, ViewPlanTemplateDetail>()
            .ForMember(dest => dest.PlanTemplateDetailId, opt => opt.MapFrom(src => src.PlanTemplateDetailId))
            .ForMember(dest => dest.PlanTemplateId, opt => opt.MapFrom(src => src.PlanTemplateId))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.I18nTitle, opt => opt.MapFrom((src, dest, destMember, context) =>
            {
                string language = context.Items.TryGetValue("Language", out object? languageObj) ? languageObj?.ToString() ?? "zh-CHT" : "zh-CHT";
                return language switch
                {
                    "zh-CHS" => src.ChTitle ?? src.Title,
                    "en-US" => src.EnTitle ?? src.Title,
                    "jp-JP" => src.JpTitle ?? src.Title,
                    _ => src.Title
                };
            }))
            .ForMember(dest => dest.SortSequence, opt => opt.MapFrom(src => src.SortSequence))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
            .ForMember(dest => dest.CreatedUser, opt => opt.MapFrom(src => src.CreatedUser))
            .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate))
            .ForMember(dest => dest.ModifiedUser, opt => opt.MapFrom(src => src.ModifiedUser))
            .ForMember(dest => dest.ExposeIndustryIds, opt => opt.MapFrom(src => string.Join(',', src.PlanTemplateDetailExposeIndustry.Select(x => x.IndustryId))))
           .ForMember(dest => dest.Rules, opt => opt.MapFrom(src =>
                src.PlanTemplateDetailGriRules != null && src.PlanTemplateDetailGriRules.Any()
                    ? src.PlanTemplateDetailGriRules.Select(x => x.GriRule)
                    : new List<GriRule>()))
            .ReverseMap();

        CreateMap<GriRule, ViewGriRule>()
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => string.Join(',', src.Code)))
            .ForMember(dest => dest.I18nDescription, opt => opt.MapFrom((src, dest, destMember, context) =>
            {
                string language = context.Items.TryGetValue("Language", out object? languageObj) ? languageObj?.ToString() ?? "zh-CHT" : "zh-CHT";
                return language switch
                {
                    "zh-CHS" => src.ChDescription ?? src.Description,
                    "en-US" => src.EnDescription ?? src.Description,
                    "jp-JP" => src.JpDescription ?? src.Description,
                    _ => src.Description
                };
            }));

    }
}
