
using DO;

namespace BO;

public class Volunteer
{
    public int Id { get; init; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public Job Role {  get; set; }
    public bool IsActive { get; set; }
    public double? maxDis {  get; set; }
    public DisType Distance {  get; set; }
    public int HandelCalls {  get; set; }
    public int CancelCalls { get; set; }
    public int ExpiredCalls { get; set; }

    BO.CallInProgress InCall = null;
    public override string ToString() => this.ToStringProperty();

}
  