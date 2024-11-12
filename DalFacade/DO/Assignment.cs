
namespace DO;

public record Assignment
 (
    int Id,
    int CallId,
    int VolunteerId,
    DateTime InterTime,
    DateTime? EndTime=null,
    AssignmentEnum? EndTreatment = null
 )
{
    public Assignment():this(0,0,0,DateTime.MinValue) {}
}
