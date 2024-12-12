
using Helpers;

namespace BO;

public class ClosedCallInList
{
    public int Id { get; init; }
    public CallType CallType { get; set; }
    public required string FullAddress { get; set; }
    public DateTime OpenTime { get; set; }
    public DateTime InterTime {  get; set; }
    public DateTime? CloseTime { get; set; }
    public EndTreatment? EndTreatment { get; set; }
    public override string ToString() => this.ToStringProperty();
}
