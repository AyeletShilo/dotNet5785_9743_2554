
namespace DO;

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
    public Volunteer(): this(0,"","","",Role.donater,false,Range.air) {}
};