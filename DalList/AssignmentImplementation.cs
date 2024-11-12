
namespace Dal;
using DalApi;
using DO;

public class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
       //int NewId = DataSource.Config;
       //Assignment newItem();
       //newItem.Id = NewId;
       //if (!Assignments.search)
       // {
       //     Assignments.pushback(newItem);
       //     return NewId;
       //}
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public void DeleteAll()
    {
        throw new NotImplementedException();
    }

    public Assignment? Read(int id)
    {
        throw new NotImplementedException();
    }

    public List<Assignment> ReadAll()
    {
        throw new NotImplementedException();
    }

    public void Update(Assignment item)
    {
        throw new NotImplementedException();
    }
}
