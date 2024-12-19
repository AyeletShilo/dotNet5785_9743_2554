
namespace DO;

/// <summary>
/// Records the assignment of a call to the volunteer who chose to handle it. 
/// Among other things, it will contain the volunteer's ID and the call's serial number. 
/// It will also record the type of completion of the call's handling.
/// </summary>
/// <param name="Id">Running ID number of the allocation entity</param>
/// <param name="CallId">Represents a number that identifies the call that the volunteer chose to handle.</param>
/// <param name="VolunteerId">Represents the ID of the volunteer who chose to handle the call.</param>
/// <param name="InterTime">Time (date and time) when the current call came in for treatment. The time when the current volunteer first chose to treat it.</param>
/// <param name="EndTime">Time (date and time) when the current volunteer finished handling the current call.</param>
/// <param name="EndTreatment">The manner in which the current call was handled by the current volunteer.</param>
public record Assignment
 (
    int Id,
    int CallId,
    int VolunteerId,
    DateTime InterTime,
    DateTime? EndTime = null,
    AssignmentEnum? EndTreatment = null
 )
{
    public Assignment() : this(0, 0, 0, DateTime.MinValue) { }
}