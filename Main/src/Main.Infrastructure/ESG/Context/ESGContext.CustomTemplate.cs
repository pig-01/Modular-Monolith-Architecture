using Main.Domain.AggregatesModel.CustomTemplateAggregate;
using Microsoft.EntityFrameworkCore;

namespace Main.Infrastructure.Demo.Context;

public partial class DemoContext
{
    public virtual DbSet<CustomExposeIndustry> CustomExposeIndustries { get; set; }

    public virtual DbSet<CustomPlanTemplate> CustomPlanTemplates { get; set; }

    public virtual DbSet<CustomPlanTemplateDetail> CustomPlanTemplateDetails { get; set; }

    public virtual DbSet<CustomPlanTemplateVersion> CustomPlanTemplateVersions { get; set; }

    public virtual DbSet<CustomRequestUnit> CustomRequestUnits { get; set; }

    static partial void OnModelCreatingPartialCustomTemplate(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new Configurations.CustomExposeIndustryConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.CustomPlanTemplateConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.CustomPlanTemplateDetailConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.CustomPlanTemplateVersionConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.CustomRequestUnitConfiguration());
    }
}
