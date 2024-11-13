
namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class VolunteerImplementation : IVolunteer
{
    public void Create(Volunteer item)
    {
        if (Read(item.Id) == item)
            throw new Exception("\"Object of type Assignment with such ID does already exist\""/*?*/);
        else
            DataSource.Volunteers?.Add(item);
    }

    public void Delete(int id)
    {
        Volunteer? item = DataSource.Volunteers?.Find(x => x.Id == id);
        if (item != null)
        {
            bool? x = DataSource.Volunteers?.Remove(item);
        }
        else
            throw new Exception("\"Object of type Assignment with such ID does not exist\""/*?*/);
    }

    public void DeleteAll()
    {
        DataSource.Volunteers?.Clear();
    }

    public Volunteer? Read(int id)
    {

        Volunteer? item = DataSource.Volunteers?.Find(x => x.Id == id);
        return item;
    }

    public List<Volunteer> ReadAll()
    {
        List<Volunteer>? copyList = new();
        if (DataSource.Volunteers != null)
            copyList.AddRange(DataSource.Volunteers);
        return copyList;
    }

    public void Update(Volunteer item)
    {
        Delete(item.Id);
        DataSource.Volunteers?.Add(item);
    }
}
