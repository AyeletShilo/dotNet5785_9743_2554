
using Helpers;

namespace BO;

public class OpenCallInList
{
    public int Id { get; init; }
    public CallType CallType { get; set; }
    public string? Description { get; set; }
    public string FullAddress {  get; set; }
    public DateTime OpenTime { get; set; }
    public DateTime? MaxCloseTime { get; set; }
    public double VolDistance { get; set; }
    public override string ToString() => this.ToStringProperty();
}
