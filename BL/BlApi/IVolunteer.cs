using BO;

namespace BlApi;

public interface IVolunteer
{
    void Create(Volunteer boVolunteer);
    IEnumerable<VolunteerInList> ReadAll(bool? isActive = null, CallInTreatment? callType = null /*ToCange*/);
    Volunteer? Read(int id); 
    void Update(int id, Volunteer volToUpdate);
    void Delete(int id);
    Job GetMyJob(int id);
}
