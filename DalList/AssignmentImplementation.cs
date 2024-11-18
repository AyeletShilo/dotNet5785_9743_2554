
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
        Assignment newItem = new(NewId,item.CallId, item.VolunteerId, item.InterTime, item.EndTime, item.EndTreatment);
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
        Assignment? item = DataSource.Assignments?.Find(x => x.Id == id);
        return item;
    }

    public List<Assignment> ReadAll()
    {
        List<Assignment>? copyList= new();
        if (DataSource.Assignments != null)
            copyList.AddRange(DataSource.Assignments);
        return copyList;
    }

    public void Update(Assignment item)
    {
        Delete(item.Id);
        DataSource.Assignments?.Add(item);
    }
}
