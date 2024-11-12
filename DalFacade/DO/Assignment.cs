
namespace DO;

public record Assignment
 (
    int Id,
    int CallId,
    int VolunteerId,
    DateTime InterTime,
    DateTime? EndTime=null
 )
{
}
