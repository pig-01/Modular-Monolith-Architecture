using AutoMapper;
using Main.Domain.AggregatesModel.PlanPermissionAggregate;
using Main.Dto.ViewModel.PlanPermission;

namespace Main.Repository.AggregatesModel.PlanPermissionAggregate;

public class PlanPermissionMappings : Profile
{
    public PlanPermissionMappings()
    {
        // PlanPermission to ViewPlanPermission
        CreateMap<PlanPermission, ViewPlanPermission>()
            .ForMember(dest => dest.PlanPermissionId, opt => opt.MapFrom(src => src.PlanPermissionID))
            .ForMember(dest => dest.IsAll, opt => opt.MapFrom(src => src.IsAll))
            .ForMember(dest => dest.OnlyCreated, opt => opt.MapFrom(src => src.OnlyCreated))
            .ForMember(dest => dest.OnlyResponseible, opt => opt.MapFrom(src => src.OnlyResponseible))
            .ForMember(dest => dest.OnlyApprove, opt => opt.MapFrom(src => src.OnlyApprove))
            .ForMember(dest => dest.IsManager, opt => opt.MapFrom(src => src.IsManager))
            .ForMember(dest => dest.PlanPermissionRelated, opt => opt.Ignore()); // 需要手動處理

        // PlanPermissionUser to ViewPlanPermissionUser
        CreateMap<PlanPermissionRelatedItem, ViewPlanPermissionRelatedItem>()
            .ForMember(dest => dest.RelatedId, opt => opt.MapFrom(src =>
                src.RelatedType == "member" ? src.UserTenant.User.UserId : src.RelatedId.ToString()
            ))
            .ForMember(dest => dest.DisplayRelatedName, opt => opt.MapFrom(src =>
                src.RelatedType == "member" ? src.UserTenant.User.UserName :
                src.RelatedType == "company" ? (src.CompanyEvent != null ? src.CompanyEvent.CompanyName : $"Company {src.RelatedId}") :
                src.RelatedType == "organization" ? (src.Organization != null ? src.Organization.OrgName : $"Organization {src.RelatedId}") :
                $"{src.RelatedType} {src.RelatedId}"
            ));
    }
}
