
using Helpers;

namespace BO;

public class CallInProgress
{
    public int Id {  get; init; }
    public int CallId {  get; init; }
    public CallType CallType { get; set; }
    public string? Description { get; set; }
    public required string FullAddress {  get; set; }
    public DateTime OpenTime { get; set; }
    public DateTime? MaxCloseTime { get; set; }
    public DateTime EntryTime { get; set; }
    public double VolDistance {  get; set; }
    public Status status { get; set; }
    public override string ToString() => this.ToStringProperty();
}
