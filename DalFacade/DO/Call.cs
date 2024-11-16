namespace DO;

/// <summary>
/// Contains details for a call opened by an administrator, 
/// and handled by a volunteer,including a running ID number
/// </summary>
/// <param name="Id">Personal unique ID of the call</param>
/// <param name="callType">Types of calls received in the organization</param>
/// <param name="callAddress">Full and real address in correct format, of the call location</param>
/// <param name="Latitude">A number indicating how far a point on Earth is south or north of the equator</param>
/// <param name="Longitude">A number indicating how far a point on Earth is east or west of the equator</param>
/// <param name="OpenTime">Time (date and hour) when the call was opened by the manager.</param>
/// <param name="description">Description of the call</param>
/// <param name="MaxTime">Time (date and hour) by which the call should be closed.</param>
public record Call
(
   int Id,
   Type CallType,
   string CallAddress,
   double Latitude,
   double Longitude,
   DateTime OpenTime,
   string? Description=null,
   DateTime? MaxTime=null
)
{
    public Call() : this(0, Type.shopping, "", 0, 0, DateTime.MinValue) { }
}
