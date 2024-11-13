
namespace DO;
/// <summary>
/// Details for a manager or volunteer in the organization, including a unique ID number
/// </summary>
/// <param name="Id">A standard ID that uniquely identifies the volunteer.</param>
/// <param name="FullName">Full name of the volunteer</param>
/// <param name="PhoneNumber">Standard cell phone of the volunteer. 10 digits only. Starts with the number 0.</param>
/// <param name="Email">Email address is valid in terms of format</param>
/// <param name="Job">Role of the volunteer</param>
/// <param name="Active">Whether the volunteer is active or not</param>
/// <param name="Distance">Type of distance: aerial distance, walking distance, driving distance</param>
/// <param name="VolAddress">Full and real address in correct format, of the volunteer. where it is currently located and available to receive calls</param>
/// <param name="Latitude">A number indicating how far a point on Earth is south or north of the equator</param>
/// <param name="Longitude">A number indicating how far a point on Earth is east or west of the equator</param>
/// <param name="MaxDistance">The maximum distance for receiving a call that the each volunteer chose</param>
public record Volunteer
(
    int Id,
    string FullName,
    string PhoneNumber,
    string Email,
    Role Job,
    bool Active,
    Range Distance, //extra
    //string? password, //extra
    string? VolAddress = null,
    double? Latitude = null,
    double? Longitude =null,
    double? MaxDistance=null
)
    

{
    public Volunteer(): this(0,"","","",Role.Donater,false,Range.Air) {}
};