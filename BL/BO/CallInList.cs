
using Helpers;

namespace BO;

public class CallInList
{
    public int? Id {  get; init; }
    public int CallId { get; init; }
    public CallType CallType { get; set; }
    public DateTime OpenTime { get; set; }
    public TimeSpan? LeftTime { get; set; }
    public string? LastVolunteer { get; set; }
    public TimeSpan? CompletionTime { get; set; }
    public CallListStatus Status { get; set; }
    public int TotalAssignments { get; set; }
    public override string ToString() => this.ToStringProperty();
}
