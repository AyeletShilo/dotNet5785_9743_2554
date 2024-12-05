
using DO;

namespace BO;

public class Volunteer
{
    public int Id { get; init; }
    public required string Name { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Email { get; set; }
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public Job Role {  get; set; }
    public bool IsActive { get; set; }
    public double? maxDis {  get; set; }
    public DisType Distance {  get; set; }
    public int HandleCalls {  get; set; }
    public int CancelCalls { get; set; }
    public int ExpiredCalls { get; set; }

    BO.CallInProgress? InCall;
    public override string ToString() => this.ToStringProperty();

}
  