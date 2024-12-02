namespace Dal;

using DalApi;
using DO;
using global::DalXml;
using System.Runtime.CompilerServices;

internal class CallImplementation : ICall
{

    /// <summary>
    ///  Create a new call in the xml file
    /// </summary>
    /// <param name="item">item to add to the xml file</param>
    public void Create(Call item)
    {
        List<Call> listC = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        int NewId = Config.NextCallId;
        Call newItem = new(NewId, item.CallType, item.CallAddress, item.Latitude, item.Longitude, item.OpenTime, item.Description, item.MaxTime);
        listC.Add(newItem);
        XMLTools.SaveListToXMLSerializer<Call>(listC, Config.s_calls_xml);
    }

    /// <summary>
    /// Delete a call item in the xml file
    /// </summary>
    /// <param name="id">ID of the call you want to delete</param>
    /// <exception cref="DalDoesNotExistException">Throw an exception in case the call you want to delete does not exist in the xml file</exception>
    public void Delete(int id)
    {
        List<Call> listC = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        Call item = listC.Find(x => x.Id == id) ?? throw new DalDoesNotExistException($"Object of type Call with ID={id} does not exists");
        bool? x = listC.Remove(item);
        XMLTools.SaveListToXMLSerializer<Call>(listC, Config.s_calls_xml);
    }

    /// <summary>
    /// Delete all the calls in the xml file
    /// </summary>
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer<Call>(new List<Call>(), Config.s_calls_xml);
    }

    /// <summary>
    /// Return a call from the existing xml file according to the ID received
    /// </summary>
    /// <param name="id">ID of the call you want to print</param>
    /// <returns>The appropriate call according to the ID received</returns>
    public Call? Read(int id)
    {
        List<Call> listC = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        Call? item = listC.FirstOrDefault(x => x.Id == id); 
        XMLTools.SaveListToXMLSerializer<Call>(listC, Config.s_calls_xml);
        return item;
    }

    /// <summary>
    /// Return a call from the existing xml file according to a boolean function received
    /// </summary>
    /// <param name="filter">boolean function </param>
    /// <returns>The first object in the xml file for which the function returns True</returns>
    public Call? Read(Func<Call, bool> filter)
    {
        List<Call> listC = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        Call? item = listC.FirstOrDefault(filter);
        XMLTools.SaveListToXMLSerializer<Call>(listC, Config.s_calls_xml);
        return item;
    }

    /// <summary>
    /// Receive a pointer to a Boolean function, that will operate on the elements of the xml file and return the list of all objects in the xml file for which the function returns True.
    /// </summary>
    /// <param name="filter">Boolean function</param>
    /// <returns>List of all calls in the xml file for which the function returns True.</returns>
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        List<Call> listC = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml) ?? new List<Call>();
        IEnumerable<Call> filteredList = filter == null ? listC : listC.Where(filter);
        XMLTools.SaveListToXMLSerializer<Call>(listC, Config.s_calls_xml);
        return filteredList;
    }

    /// <summary>
    /// Updates an existing call in the xml file with new data
    /// </summary>
    /// <param name="item">The updated call with the new data</param>
    /// <exception cref="DalDoesNotExistException">Throw an exception in case the call you want to update does not exist in the xml file</exception>
    public void Update(Call item)
    {
        List<Call> listC = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        Call itemTodelete = listC.Find(x => x.Id == item.Id) ?? throw new DalDoesNotExistException($"Object of type Call with ID={item.Id} does not exists");
        bool? x = listC.Remove(itemTodelete);
        listC.Add(item);
        XMLTools.SaveListToXMLSerializer<Call>(listC, Config.s_calls_xml);
    }
}

