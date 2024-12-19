namespace Dal;

using DalApi;
using DO;
using global::DalXml;

internal class AssignmentImplementation : IAssignment
{
    /// <summary>
    /// Create a new assignment in the xml file
    /// </summary>
    /// <param name="item"> item to add to the xml file</param>
    public void Create(Assignment item)
    {
        List<Assignment> listA = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        int NewId = Config.NextAssignmentId;
        Assignment newItem = new(NewId, item.CallId, item.VolunteerId, item.InterTime, item.EndTime, item.EndTreatment);
        listA.Add(newItem);
        XMLTools.SaveListToXMLSerializer(listA, Config.s_assignments_xml);
    }

    /// <summary>
    /// Delete an assignment item in the xml file
    /// </summary>
    /// <param name="id">ID of the assignment you want to delete</param>
    /// <exception cref="DalDoesNotExistException">Throw an exception in case the assignment you want to delete does not exist in the xml file</exception>
    public void Delete(int id)
    {
        List<Assignment> listA = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        Assignment? item = listA.Find(x => x.Id == id) ?? throw new DalDoesNotExistException($"Object of type Assignment with ID={id} does not exists");
        bool x = listA.Remove(item);
        XMLTools.SaveListToXMLSerializer(listA, Config.s_assignments_xml);
    }

    /// <summary>
    /// Delete all the assignments in the xml file
    /// </summary>
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Assignment>(), Config.s_assignments_xml);
    }

    /// <summary>
    /// Return an assignment from the existing xml file according to the ID received
    /// </summary>
    /// <param name="id">ID of the assignment you want to print</param>
    /// <returns>The appropriate assignment according to the ID received </returns>
    public Assignment? Read(int id)
    {
        List<Assignment> listA = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        Assignment? item = listA.FirstOrDefault(x => x.Id == id);
        XMLTools.SaveListToXMLSerializer(listA, Config.s_assignments_xml);
        return item;
    }

    /// <summary>
    /// Return an assignment from the existing xml file according to a boolean function received
    /// </summary>
    /// <param name="filter">boolean function</param>
    /// <returns>The first object in the xml file for which the function returns True</returns>
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        List<Assignment> listA = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        Assignment? item = listA.LastOrDefault(filter);
        XMLTools.SaveListToXMLSerializer(listA, Config.s_assignments_xml);
        return item;
    }

    /// <summary>
    /// Receive a pointer to a Boolean function, that will operate on the elements of the xml file and return the list of all objects in the xml file for which the function returns True.
    /// </summary>
    /// <param name="filter">Boolean function</param>
    /// <returns> List of all objects in the xml file for which the function returns True.</returns>
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        List<Assignment> listA = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml) ?? new List<Assignment>();
        IEnumerable<Assignment> filteredList = filter == null ? listA : listA.Where(filter);
        XMLTools.SaveListToXMLSerializer(listA, Config.s_assignments_xml);
        return filteredList;
    }

    /// <summary>
    /// Updates an existing assignment in the xml file with new data
    /// </summary>
    /// <param name="item">The updated assignment with the new data</param>
    /// <exception cref="DalDoesNotExistException">Throw an exception in case the assignment you want to update does not exist in the xml file </exception>
    public void Update(Assignment item)
    {
        List<Assignment> listA = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        Assignment itemToDelete = listA.Find(x => x.Id == item.Id) ?? throw new DalDoesNotExistException($"Object of type assignment with ID={item.Id} does not exists");
        bool? x = listA.Remove(itemToDelete);
        listA.Add(item);
        XMLTools.SaveListToXMLSerializer(listA, Config.s_assignments_xml);
    }
}

