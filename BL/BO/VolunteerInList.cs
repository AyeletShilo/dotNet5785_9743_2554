
using Helpers;

namespace BO;

public class VolunteerInList
{
    public int Id { get; init; }
    public string FullName { get; set; }
    public bool IsActive {  get; set; }
    public int HandleCalls {  get; set; }
    public int CancelCalls {  get; set; }
    public int ExpiredCalls { get; set; }
    public int? CallId { get; init; }
    public CallInTreatment InTreatment { get; set; }
    public override string ToString() => this.ToStringProperty();
}
