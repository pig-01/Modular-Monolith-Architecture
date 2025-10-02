using Main.Domain.AggregatesModel.CustomTemplateAggregate;
using Main.Dto.ViewModel.CustomTemplate;

namespace Main.Repository.AggregatesModel.CustomTemplateAggregate;

public class CustomRequestUnitMappings : Profile
{
    public CustomRequestUnitMappings()
    {
        CreateMap<CustomRequestUnit, ViewCustomRequestUnit>();
        CreateMap<CustomPlanTemplateVersion, ViewCustomPlanTemplateVersion>()
            .ForMember(dest => dest.CustomPlanTemplates, opt => opt.MapFrom(src => src.CustomPlanTemplates));
        CreateMap<CustomPlanTemplate, ViewCustomPlanTemplate>();
        CreateMap<CustomPlanTemplateDetail, ViewCustomPlanTemplateDetail>();
    }
}
