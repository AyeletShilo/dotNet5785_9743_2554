using BO;

namespace BlApi;

public interface IVolunteer
{
    void Create(Volunteer boVolunteer);
    IEnumerable<VolunteerInList> ReadAll(bool? isActive = null, VolunteerData? sort = null);
    Volunteer? Read(int id);
    void Update(int id, Volunteer volToUpdate);
    void Delete(int id);
    Role GetMyJob(int id);
}
