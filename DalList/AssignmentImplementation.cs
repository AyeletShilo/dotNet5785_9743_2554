
namespace Dal;
using DalApi;
using DO;
using System.Diagnostics.CodeAnalysis;
using System.IO;

public class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
        int NewId = Config.nextAssignmentId;
        Assignment newItem = new(NewId,item.CallId, item.VolunteerId, item.InterTime, item.EndTime, item.EndTreatment);
        DataSource.Assignments?.Add(newItem);
    }

    public void Delete(int id)
    {
        Assignment? item = DataSource.Assignments?.Find(x => x.Id == id);
        if (item != null)
        {
            bool? x = DataSource.Assignments?.Remove(item);
        }
        else
            throw new Exception($"Object of type Assignment with ID={item?.Id} does not exists");   
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
