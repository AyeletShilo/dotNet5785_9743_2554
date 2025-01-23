namespace Dal;
using DalApi;
using DO;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;

internal class AssignmentImplementation : IAssignment
{
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Assignment item)
    {
        int NewId = Config.nextAssignmentId;
        Assignment newItem = new(NewId, item.CallId, item.VolunteerId, item.InterTime, item.EndTime, item.EndTreatment);
        DataSource.Assignments?.Add(newItem);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        Assignment? item = DataSource.Assignments?.Find(x => x.Id == id) ?? throw new DalDoesNotExistException($"Object of type Assignment with ID={id} does not exists");
        bool? x = DataSource.Assignments?.Remove(item);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        DataSource.Assignments?.Clear();
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(int id)
    {
        //Assignment? item = DataSource.Assignments?.Find(x => x.Id == id); //stage 1
        Assignment? item = DataSource.Assignments?.FirstOrDefault(x => x.Id == id); //stage 2
        return item;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(Func<Assignment, bool> filter) //stage 2
    {
        Assignment? item = DataSource.Assignments?.LastOrDefault(filter);
        return item;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null) //stage 2
        => filter == null
            ? DataSource.Assignments.Select(item => item)
            : DataSource.Assignments.Where(filter);

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Assignment item)
    {
        Delete(item.Id);
        DataSource.Assignments?.Add(item);
    }
}
