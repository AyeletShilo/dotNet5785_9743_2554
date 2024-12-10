

using DO;
using Helpers;

namespace BO;

public class CallAssignInList
{
    public int? VolunteerId { get; init; }
    public string? VolunteerName {  get; init; }
    public DateTime InterTime { get; set; }
    public DateTime? EndTime {  get; set; }
    public EndTreatment? EndTreatment {  get; set; }
    public override string ToString() => this.ToStringProperty();
}
