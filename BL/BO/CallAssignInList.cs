

using DO;
using Helpers;

namespace BO;

public class CallAssignInList
{
    public int? VolunteerId { get; init; }
    public string? VolunteerName {  get; init; }
    DateTime InterTime { get; set; }
    DateTime? EndTime {  get; set; }
    EndTreatment? EndTreatment {  get; set; }
    public override string ToString() => this.ToStringProperty();
}
