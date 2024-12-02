namespace Dal;

using DalApi;
using DO;
using global::DalXml;

internal class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
        List<Assignment> listA = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        int NewId = Config.NextAssignmentId;
        Assignment newItem = new(NewId, item.CallId, item.VolunteerId, item.InterTime, item.EndTime, item.EndTreatment);
        listA.Add(newItem);
        XMLTools.SaveListToXMLSerializer(listA, Config.s_assignments_xml);
    }

    public void Delete(int id)
    {
        List<Assignment> listA = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        Assignment? item = listA.Find(x => x.Id == id) ?? throw new DalDoesNotExistException($"Object of type Assignment with ID={id} does not exists");
        bool x = listA.Remove(item);
        XMLTools.SaveListToXMLSerializer(listA, Config.s_assignments_xml);
    }

    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Assignment>(), Config.s_assignments_xml);
    }

    public Assignment? Read(int id)
    {
        List<Assignment> listA = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        Assignment? item = listA.FirstOrDefault(x => x.Id == id);
        XMLTools.SaveListToXMLSerializer(listA, Config.s_assignments_xml);
        return item;
    }

    public Assignment? Read(Func<Assignment, bool> filter)
    {
        List<Assignment> listA = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        Assignment? item = listA.FirstOrDefault(filter);
        XMLTools.SaveListToXMLSerializer(listA, Config.s_assignments_xml);
        return item;
    }

    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        List<Assignment> listA = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml) ?? new List<Assignment>();
        IEnumerable<Assignment> filteredList = filter == null ? listA : listA.Where(filter);
        //XMLTools.SaveListToXMLSerializer(listA, Config.s_assignments_xml);
        return filteredList;
    }

    public void Update(Assignment item)
    {
        List<Assignment> listA = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        Delete(item.Id);
        listA.Add(item);
        XMLTools.SaveListToXMLSerializer(listA, Config.s_assignments_xml);
    }
}

