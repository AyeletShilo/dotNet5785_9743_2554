
namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

internal class VolunteerImplementation : IVolunteer
{
    public void Create(Volunteer item)
    {
        if (Read(item.Id) == item)
            throw new Exception($"Object of type Assignment with ID={item.Id} does already exist");
        else
            DataSource.Volunteers?.Add(item);
    }

    public void Delete(int id)
    {
        Volunteer? item = DataSource.Volunteers?.Find(x => x.Id == id) ?? throw new Exception($"Object of type Assignment with ID={id} does not exist");
        bool? x = DataSource.Volunteers?.Remove(item);
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
        if (DataSource.Volunteers is not null)
            copyList.AddRange(DataSource.Volunteers);
        return copyList;
    }

    public void Update(Volunteer item)
    {
        Delete(item.Id);
        DataSource.Volunteers?.Add(item);
    }
}
