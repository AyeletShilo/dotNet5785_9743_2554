using BO;

namespace BlApi;

public interface IVolunteer : IObservable //stage 5
{
    void Create(Volunteer boVolunteer);
    IEnumerable<VolunteerInList> ReadAll(bool? isActive = null, VolunteerData? sort = null , CallInTreatment? filter = CallInTreatment.None);
    Volunteer? Read(int id);
    void Update(int id, Volunteer volToUpdate);
    void Delete(int id);
    Role GetMyJob(int id);
}
