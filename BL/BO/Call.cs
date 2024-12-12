
using Helpers;

namespace BO;

public class Call
{
    public int Id {  get; set; }
    public CallType CallType { get; set; }
    public string? Description { get; set; }
    public required string CallAddress { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime OpenTime { get; set; }
    public DateTime? MaxCloseTime { get; set; }
    public CallStatus Status { get; set; }
    public required List<BO.CallAssignInList> CallAssignments { get; set; }
    public override string ToString() => this.ToStringProperty();
}
