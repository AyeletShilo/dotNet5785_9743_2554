namespace Dal;
using DalApi;
using DO;
using System.Diagnostics.CodeAnalysis;
using System.IO;

internal class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
        int NewId = Config.nextAssignmentId;//?//
        Assignment newItem = new(NewId, item.CallId, item.VolunteerId, item.InterTime, item.EndTime, item.EndTreatment);
        DataSource.Assignments?.Add(newItem);
    }

    public void Delete(int id)
    {
        Assignment? item = DataSource.Assignments?.Find(x => x.Id == id) ?? throw new Exception($"Object of type Assignment with ID={id} does not exists");
        bool? x = DataSource.Assignments?.Remove(item);
    }

    public void DeleteAll()
    {
        DataSource.Assignments?.Clear();
    }

    public Assignment? Read(int id)
    {
        //Assignment? item = DataSource.Assignments?.Find(x => x.Id == id); //stage 1
        Assignment? item = DataSource.Assignments?.FirstOrDefault(x => x.Id == id); //stage 2
        return item;
    }

    public Assignment? Read(Func<Assignment, bool> filter) //stage 2
    {
        Assignment? item = DataSource.Assignments?.FirstOrDefault(filter);
        return item;
    }

    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null) //stage 2
        => filter == null
            ? DataSource.Assignments.Select(item => item)
            : DataSource.Assignments.Where(filter);


    public void Update(Assignment item)
    {
        Delete(item.Id);
        DataSource.Assignments?.Add(item);
    }
}
