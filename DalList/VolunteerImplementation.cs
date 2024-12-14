namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

internal class VolunteerImplementation : IVolunteer
{
    public void Create(Volunteer item)
    {
        if (Read(item.Id) != null)
            throw new DalAlreadyExistException($"Object of type Volunteer with ID={item.Id} does already exist");
        else
            DataSource.Volunteers?.Add(item);
    }

    public void Delete(int id)
    {
        Volunteer? item = DataSource.Volunteers?.Find(x => x.Id == id) ?? throw new DalDoesNotExistException($"Object of type Volunteer with ID={id} does not exist");
        bool? x = DataSource.Volunteers?.Remove(item);
    }
    public void DeleteAll()
    {
        DataSource.Volunteers?.Clear();
    }

    public Volunteer? Read(int id)
    {
        //Volunteer? item = DataSource.Volunteers?.Find(x => x.Id == id); //stage 1
        Volunteer? item = DataSource.Volunteers?.FirstOrDefault(x => x.Id == id); //stage 2
        return item;
    }

    public Volunteer? Read(Func<Volunteer, bool> filter) //stage 2
    {
        Volunteer? item = DataSource.Volunteers?.FirstOrDefault(filter);
        return item;
    }

    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null) //stage 2
       => filter == null
            ? DataSource.Volunteers.Select(item => item)
            : DataSource.Volunteers.Where(filter);

    public void Update(Volunteer item)
    {
        Delete(item.Id);
        DataSource.Volunteers?.Add(item);
    }
}
