using System.Globalization;
using System.Linq.Expressions;
using Scheduler.Domain.AggregateModel.PlanAggregate;
using Scheduler.Domain.Enums;
using Scheduler.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Scheduler.Application.Queries.Plans;

public class PlanDocumentQuery(DemoContext context, ILogger<PlanDocumentQuery> logger) : IPlanDocumentQuery
{
    public async Task<PlanDocument?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await context.PlanDocuments
            .FirstOrDefaultAsync(x => x.PlanDocumentId == id, cancellationToken);
    }

    public async Task<IEnumerable<PlanDocument>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await context.PlanDocuments
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PlanDocument>> ListAsync(Expression<Func<PlanDocument, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await context.PlanDocuments
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 取得即將過期的表單，定義為結束日介於 (今天 + 過期天數 - 1) 與 (今天 + 過期天數) 之間的表單
    /// </summary>
    /// <param name="daysUntilExpiration">距離過期的天數</param>
    /// <param name="cancellationToken">取消權杖</param>
    /// <returns>即將過期的表單清單</returns>
    public async Task<IEnumerable<PlanDocument>> GetExpiringDocumentsAsync(int daysUntilExpiration, CancellationToken cancellationToken)
    {
        try
        {
            // Calculate the date range for expiring documents,
            // between yesterday + daysUntilExpiration and today + daysUntilExpiration
            DateTime currentDate = DateTime.UtcNow.Date;
            DateTime startDate = currentDate.AddDays(-1).AddDays(daysUntilExpiration);
            DateTime expirationDate = currentDate.AddDays(daysUntilExpiration);

            logger.LogInformation("Querying expiring documents from {StartDate} to {ExpirationDate}", startDate, expirationDate);

            string unwrittenStatus = DocumentStatus.UnWritten.Id.ToString(CultureInfo.InvariantCulture);
            string writtenStatus = DocumentStatus.Written.Id.ToString(CultureInfo.InvariantCulture);
            string approvingStatus = DocumentStatus.Approving.Id.ToString(CultureInfo.InvariantCulture);

            List<PlanDocument> expiringDocuments = await context.PlanDocuments
                .Where(x => x.EndDate.Date <= expirationDate &&
                           x.EndDate.Date >= startDate &&
                           (x.FormStatus == unwrittenStatus || // 待填寫
                            x.FormStatus == writtenStatus || // 已填寫
                            x.FormStatus == approvingStatus))  // 待審核
                .Include(x => x.PlanDetail)
                .ThenInclude(x => x.Plan)
                .OrderBy(x => x.EndDate)
                .ToListAsync(cancellationToken);

            logger.LogInformation("Found {Count} expiring documents", expiringDocuments.Count);

            return expiringDocuments;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while querying expiring documents");
            throw;
        }
    }
}
