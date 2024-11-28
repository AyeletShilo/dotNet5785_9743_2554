namespace Dal;

using DalApi;
using DO;
using DalXml;
using System.Runtime.CompilerServices;

internal class CallImplementation : ICall
{
    public void Create(Call item)
    {
        List<Call> listC = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        int NewId = Config.NextCallId;
        Call newItem = new(NewId, item.CallType, item.CallAddress, item.Latitude, item.Longitude, item.OpenTime, item.Description, item.MaxTime);
        listC.Add(newItem);
        XMLTools.SaveListToXMLSerializer<Call>(listC, Config.s_calls_xml);
    }

    public void Delete(int id)
    {
        List<Call> listC = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        Call item = listC.Find(x => x.Id == id) ?? throw new DalDoesNotExistException($"Object of type Call with ID={id} does not exists");
        bool? x = listC.Remove(item);
        XMLTools.SaveListToXMLSerializer<Call>(listC, Config.s_calls_xml);
    }


    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer<Call>(new List<Call>(), Config.s_calls_xml);
    }

    public Call? Read(int id)
    {
        List<Call> listC = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        Call? item = listC.FirstOrDefault(x => x.Id == id); 
        XMLTools.SaveListToXMLSerializer<Call>(listC, Config.s_calls_xml);
        return item;
    }

    public Call? Read(Func<Call, bool> filter)
    {
        List<Call> listC = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        Call? item = listC.FirstOrDefault(filter);
        XMLTools.SaveListToXMLSerializer<Call>(listC, Config.s_calls_xml);
        return item;
    }

    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        List<Call> listC = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
         //=> filter == null
         //   ? listC.Select(item => item)
         //   : listC.Where(filter);
        XMLTools.SaveListToXMLSerializer<Call>(listC, Config.s_calls_xml);
    }

    public void Update(Call item)
    {
        List<Call> listC = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        Delete(item.Id);
        listC.Add(item);
        XMLTools.SaveListToXMLSerializer<Call>(listC, Config.s_calls_xml);
    }
}

