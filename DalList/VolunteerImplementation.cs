namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

internal class VolunteerImplementation : IVolunteer
{

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Volunteer item)
    {
        if (Read(item.Id) != null)
            throw new DalAlreadyExistException($"Object of type Volunteer with ID={item.Id} does already exist");
        else
            DataSource.Volunteers?.Add(item);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        Volunteer? item = DataSource.Volunteers?.Find(x => x.Id == id) ?? throw new DalDoesNotExistException($"Object of type Volunteer with ID={id} does not exist");
        bool? x = DataSource.Volunteers?.Remove(item);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        DataSource.Volunteers?.Clear();
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Volunteer? Read(int id)
    {
        //Volunteer? item = DataSource.Volunteers?.Find(x => x.Id == id); //stage 1
        Volunteer? item = DataSource.Volunteers?.FirstOrDefault(x => x.Id == id); //stage 2
        return item;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Volunteer? Read(Func<Volunteer, bool> filter) //stage 2
    {
        Volunteer? item = DataSource.Volunteers?.FirstOrDefault(filter);
        return item;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null) //stage 2
       => filter == null
            ? DataSource.Volunteers.Select(item => item)
            : DataSource.Volunteers.Where(filter);

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Volunteer item)
    {
        Delete(item.Id);
        DataSource.Volunteers?.Add(item);
    }
}
