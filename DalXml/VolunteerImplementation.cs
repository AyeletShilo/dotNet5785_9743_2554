namespace Dal;

using DalApi;
using DO;

internal class VolunteerImplementation : IVolunteer
{
    public void Create(Volunteer item)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public void DeleteAll()
    {
        throw new NotImplementedException();
    }

    public Volunteer? Read(int id)
    {
        throw new NotImplementedException();
    }

    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        throw new NotImplementedException();
    }

    public void Update(Volunteer item)
    {
        throw new NotImplementedException();
    }
}
