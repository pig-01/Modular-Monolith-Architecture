using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Scheduler.Models;

public class RegisterJobRequest(string jobName, string cronExpression, IDictionary<string, object>? jobArgs = null)
{
    [JsonPropertyName("jobName")]
    [DefaultValue("MailJob")]
    public required string JobName { get; set; } = jobName;

    [JsonPropertyName("jobArgs")]
    public IDictionary<string, object>? JobArgs { get; set; } = jobArgs;

    [JsonPropertyName("cronExpression")]
    [DefaultValue("0/5 * * * * ?")]
    public required string CronExpression { get; set; } = cronExpression;
}
