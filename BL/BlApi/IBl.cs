
namespace BlApi;

public interface IBl
{
    IVolunteer Volunteer { get; }
    ICall call { get; }
    IAdmin admin { get; }

}
