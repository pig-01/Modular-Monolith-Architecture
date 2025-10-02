using System.Linq.Expressions;
using Base.Infrastructure.Toolkits.Utilities.SqlBuilder;
using Main.Domain.AggregatesModel.PlanTemplateAggregate;
using Main.Dto.ViewModel.PlanTemplate;
using Main.Infrastructure.Demo.Context;

namespace Main.WebApi.Application.Queries.PlanTemplates.Impl;

public class PlanTemplateQuery(DemoContext context) : IPlanTemplateQuery
{
    public async Task<PlanTemplate?> GetByIdAsync(long id, CancellationToken cancellationToken = default) => await context.PlanTemplates
        .AsSplitQuery() // 避免 Cartesian 爆炸
        .Include(x => x.PlanTemplateForms)
        .Include(x => x.PlanTemplateRequestUnits)
        .Include(x => x.PlanTemplateDetails)
            .ThenInclude(x => x.PlanTemplateDetailExposeIndustry)
        .Include(x => x.PlanTemplateDetails)
            .ThenInclude(x => x.PlanTemplateDetailGriRules)
                .ThenInclude(x => x.GriRule)
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.PlanTemplateId == id, cancellationToken);

    public async Task<IEnumerable<string>> GetVersionListAsync(bool isAdmin = false, CancellationToken cancellationToken = default) => await context.PlanTemplates
        .Where(x => isAdmin || x.IsDeploy) // 如果不是 Admin，則只查詢已部署的版本
        .AsNoTracking().Select(x => x.Version)
        .Distinct()
        .OrderByDescending(version => version)
        .ToListAsync(cancellationToken);

    public async Task<IEnumerable<PlanTemplate>> ListAsync(string tenantId, string[]? indicatorIds = null, string? version = null, CancellationToken cancellationToken = default)
    {
        // 先查詢基本的 PlanTemplate 資料
        IQueryable<PlanTemplate> query = context.PlanTemplates
            .AsSplitQuery() // 避免 Cartesian 爆炸
            .Include(x => x.PlanTemplateForms)
            .Include(x => x.PlanTemplateDetails)
                .ThenInclude(x => x.PlanTemplateDetailExposeIndustry)
            .Include(x => x.PlanTemplateDetails)
                .ThenInclude(x => x.PlanTemplateDetailGriRules)
                    .ThenInclude(x => x.GriRule)
            .Where(x => x.IsDeploy);

        // 如果有指定指標 ID，則進行篩選
        if (indicatorIds != null && indicatorIds.Length > 0)
            query = query.Where(x => x.PlanTemplateRequestUnits.Any(unit => indicatorIds.Contains(unit.UnitCode)));

        // 如果沒有指定版本，使用子查詢找最新版本
        query = string.IsNullOrEmpty(version)
            ? query.Where(x => x.Version == context.PlanTemplates
                .Where(pt => pt.IsDeploy)
                .Max(pt => pt.Version))
            : query.Where(x => x.Version == version);

        // 根據 GroupId 進行左連接，並選擇需要的欄位
        return await query
            .GroupJoin(
                context.SystemDatas.AsNoTracking(),
                pt => pt.GroupId,
                sd => sd.SystemCode,
                (pt, systemDatas) => new { PlanTemplate = pt, SystemData = systemDatas.FirstOrDefault() }
            )
            .Select(joined => new PlanTemplate
            {
                PlanTemplateId = joined.PlanTemplate.PlanTemplateId,
                PlanTemplateName = joined.PlanTemplate.PlanTemplateName,
                PlanTemplateChName = joined.PlanTemplate.PlanTemplateChName,
                PlanTemplateEnName = joined.PlanTemplate.PlanTemplateEnName,
                PlanTemplateJpName = joined.PlanTemplate.PlanTemplateJpName,
                FormName = joined.PlanTemplate.FormName,
                GroupId = joined.PlanTemplate.GroupId,
                Version = joined.PlanTemplate.Version,
                RowNumber = joined.PlanTemplate.RowNumber,
                SortSequence = joined.PlanTemplate.SortSequence,
                IsDeploy = joined.PlanTemplate.IsDeploy,
                PlanTemplateDetails = joined.PlanTemplate.PlanTemplateDetails,
                PlanTemplateRequestUnits = joined.PlanTemplate.PlanTemplateRequestUnits,
                PlanTemplateForms = joined.PlanTemplate.PlanTemplateForms,
                I18nGroupName = joined.SystemData != null ? joined.SystemData.I18nCode : null
            })
            .OrderBy(x => x.SortSequence)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PlanTemplate>> ListAsync(Expression<Func<PlanTemplate, bool>> predicate, CancellationToken cancellationToken = default) => await context.PlanTemplates
            .AsSplitQuery() // 避免 Cartesian 爆炸
            .Where(x => x.IsDeploy)
            .Where(x => x.Version == context.PlanTemplates
                .Where(pt => pt.IsDeploy)
                .Max(pt => pt.Version)) // 只查詢最新版本
            .Include(x => x.PlanTemplateRequestUnits)
            .Include(x => x.PlanTemplateDetails)
                .ThenInclude(x => x.PlanTemplateDetailExposeIndustry)
            .Include(x => x.PlanTemplateDetails)
                .ThenInclude(x => x.PlanTemplateDetailGriRules)
                    .ThenInclude(x => x.GriRule)
            .AsNoTracking()
            .Where(predicate)
            .Select(planTemplate => new PlanTemplate
            {
                PlanTemplateId = planTemplate.PlanTemplateId,
                PlanTemplateName = planTemplate.PlanTemplateName,
                FormName = planTemplate.FormName,
                GroupId = planTemplate.GroupId,
                Version = planTemplate.Version,
                RowNumber = planTemplate.RowNumber,
                SortSequence = planTemplate.SortSequence,
                PlanTemplateDetails = planTemplate.PlanTemplateDetails,
                PlanTemplateRequestUnits = planTemplate.PlanTemplateRequestUnits,
                PlanTemplateForms = planTemplate.PlanTemplateForms,
                I18nGroupName = context.SystemDatas
                    .AsNoTracking()
                    .Where(systemData => systemData.SystemCode == planTemplate.GroupId)
                    .Select(systemData => systemData.I18nCode)
                    .FirstOrDefault()
            })
            .OrderBy(planTemplate => planTemplate.SortSequence)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<PlanTemplate>> ListAsync(CancellationToken cancellationToken = default) => await context.PlanTemplates
            .AsSplitQuery() // 避免 Cartesian 爆炸
            .Include(x => x.PlanTemplateRequestUnits)
            .Include(x => x.PlanTemplateDetails)
                .ThenInclude(x => x.PlanTemplateDetailExposeIndustry)
            .Include(x => x.PlanTemplateDetails)
                .ThenInclude(x => x.PlanTemplateDetailGriRules)
                    .ThenInclude(x => x.GriRule)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<GriRule>> GetGriRulesAsync(CancellationToken cancellationToken = default) => await context.GriRules
            .AsNoTracking()
            .Select(griRule => new GriRule
            {
                Id = griRule.Id,
                Code = griRule.Code,
                Unit = griRule.Unit,
                TagColor = griRule.TagColor,
                Icon = griRule.Icon,
                CreatedUser = griRule.CreatedUser,
                CreatedDate = griRule.CreatedDate,
                ModifiedDate = griRule.ModifiedDate,
                ModifiedUser = griRule.ModifiedUser,
                Description = griRule.Description,
                ChDescription = griRule.ChDescription,
                EnDescription = griRule.EnDescription,
                JpDescription = griRule.JpDescription
            })
            .ToListAsync(cancellationToken);
    public async Task<IEnumerable<ViewPlanTemplateExcelData>> GetPlanTemplateExcelDataAsync(string version, CancellationToken cancellationToken = default)
    {
        SqlBuilder sb = new SqlBuilder(@"
            SELECT
	            e.Name AS GroupName,
	            b.PlanTemplateName,
                b.PlanTemplateChName,
                b.PlanTemplateEnName,
                b.PlanTemplateJpName,
	            b.FormID,
                b.FormName,
                b.AcceptDataSource,
                b.IsDeploy,
	            a.Title AS PlanTemplateDetailTitle,
                a.ChTitle AS PlanTemplateDetailChTitle,
                a.EnTitle AS PlanTemplateDetailEnTitle,
                a.JpTitle AS PlanTemplateDetailJpTitle,
	            STUFF((SELECT ',' + CAST(gr.Code as NVARCHAR(30))
		            FROM GriRule gr
		            LEFT JOIN PlanTemplateDetailGriRule pgr ON gr.ID = pgr.GriRuleID
		            WHERE pgr.PlanTemplateDetailID = a.PlanTemplateDetailID
		            FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '') AS GriRuleCodes,
	            STUFF((SELECT ',' + CAST(pru.UnitCode as NVARCHAR(30))
		            FROM PlanTemplateRequestUnit pru
		            WHERE pru.PlanTemplateID = a.PlanTemplateID
		            FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '') AS RequestUnitIds,
	            STUFF((SELECT ',' + CAST(pei.IndustryID as NVARCHAR(30))
		            FROM PlanTemplateDetailExposeIndustry pei
		            WHERE pei.PlanTemplateDetailID = a.PlanTemplateDetailID
		            FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '') AS ExposeIndustryIds
            FROM PlanTemplateDetail a
            LEFT JOIN PlanTemplate b ON a.PlanTemplateID = b.PlanTemplateID
            LEFT JOIN SystemData e ON e.SystemCode = b.GroupID AND e.CodeType = 'DemoGroupId'
        ")
        .WhereScope(where => where.AddWhereClause(" AND b.Version = @version"))
        .AddOrderBy("b.SortSequence");

        return await context.QueryAsync<ViewPlanTemplateExcelData>(sb.ToString(), new { version });
    }
}

