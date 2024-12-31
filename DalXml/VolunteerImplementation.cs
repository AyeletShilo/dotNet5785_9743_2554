namespace Dal;

using DalApi;
using global::DalXml;
using DO;
using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Collections;

internal class VolunteerImplementation : IVolunteer
{
    /// <summary>
    /// Receives an Xelement and returns the volunteer who is inside
    /// </summary>
    /// <param name="v">Xelement with a volunteer inside it </param>
    /// <returns> The volunteer who is inside the XElement</returns>
    /// <exception cref="FormatException">Throw exception when  the function unable to convert id from XElement </exception>
    static Volunteer getVolunteer(XElement v)
    {
        return new DO.Volunteer()
        {
            Id = v.ToIntNullable("Id") ?? throw new FormatException("can't convert id"),
            FullName = (string?)v.Element("FullName") ?? " ",
            PhoneNumber = (string?)v.Element("PhoneNumber") ?? " ",
            Email = (string?)v.Element("Email") ?? " ",
            Job = v.ToEnumNullable<Role>("Job") ?? Role.Volunteer,
            Active = (bool?)v.Element("Active") ?? false,
            Distance = v.ToEnumNullable<RangeType>("Distance") ?? RangeType.Walking,
            VolAddress = (string?)v.Element("VolAddress") ?? null,
            Latitude = v.ToDoubleNullable("Latitude") ?? null,
            Longitude = v.ToDoubleNullable("Longitude") ?? null,
            MaxDistance = v.ToDoubleNullable("MaxDistance") ?? null,
        };
    }


    /// <summary>
    ///  Create a new volunteer in the xml file
    /// </summary>
    /// <param name="item">item to add to the xml file</param>
    public void Create(Volunteer item)
    {
        XElement rootVolunteer = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);
        rootVolunteer.Add(createVolunteerElement(item));
        XMLTools.SaveListToXMLElement(rootVolunteer,Config.s_volunteers_xml);
    }

    /// <summary>
    /// Delete a volunteer item in the xml file
    /// </summary>
    /// <param name="id">ID of the volunteer you want to delete</param>
    /// <exception cref="DalDoesNotExistException">Throw an exception in case the volunteer you want to delete does not exist in the xml file</exception>
    public void Delete(int id)
    {
        XElement? rootVolunteer = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);
        XElement? elemToDelete= rootVolunteer.Elements().FirstOrDefault(vol => (int?)vol.Element("Id") == id);
        if (elemToDelete!=null)
            elemToDelete.Remove();
        else
            throw new DalDoesNotExistException($"Object of type volunteer with ID={id} does not exists"); 

        XMLTools.SaveListToXMLElement(rootVolunteer,Config.s_volunteers_xml);
    }

    /// <summary>
    /// Delete all the volunteers in the xml file
    /// </summary>
    public void DeleteAll()
    {
        XElement? resetVol = new XElement("ArrayOfVolunteers");
        XMLTools.SaveListToXMLElement(resetVol, Config.s_volunteers_xml);
    }

    /// <summary>
    /// Return a volunteer from the existing xml file according to the ID received
    /// </summary>
    /// <param name="id">ID of the volunteer you want to print</param>
    /// <returns>The appropriate volunteer according to the ID received</returns>
    public Volunteer? Read(int id)
    {
        XElement? volunteerElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().FirstOrDefault(vt => (int?)vt.Element("Id") == id);
        return volunteerElem is null ? null : getVolunteer(volunteerElem);
    }

    /// <summary>
    /// Return a volunteer from the existing xml file according to a boolean function received
    /// </summary>
    /// <param name="filter">boolean function </param>
    /// <returns>The first object in the xml file for which the function returns True</returns>
    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().Select(v => getVolunteer(v)).FirstOrDefault(filter);
    }

    /// <summary>
    /// Receive a pointer to a Boolean function, that will operate on the elements of the xml file and return the list of all objects in the xml file for which the function returns True.
    /// </summary>
    /// <param name="filter">Boolean function</param>
    /// <returns>List of all volunteers in the xml file for which the function returns True.</returns>
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
       var toFilter = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);
        if (filter == null)
            return toFilter.Elements().Select(v => getVolunteer(v));
        else
           return toFilter.Elements().Select(v => getVolunteer(v)).Where(filter); 
        
    }

    /// <summary>
    /// Updates an existing volunteer in the xml file with new data
    /// </summary>
    /// <param name="item">The updated volunteer with the new data</param>
    /// <exception cref="DO.DalDoesNotExistException">Throw an exception in case the volunteer you want to update does not exist in the xml file</exception>
    public void Update(Volunteer item)
    { 
        XElement volunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);

        (volunteersRootElem.Elements().FirstOrDefault(st => (int?)st.Element("Id") == item.Id)
        ?? throw new DO.DalDoesNotExistException($"Volunteer with ID={item.Id} does Not exist"))
                .Remove();

        volunteersRootElem.Add(new XElement(createVolunteerElement(item)));

        XMLTools.SaveListToXMLElement(volunteersRootElem, Config.s_volunteers_xml);
    }

    /// <summary>
    /// Creates XElement with new volunteer from the receive volunteer
    /// </summary>
    /// <param name="item">Volunteer to add to the xml file</param>
    /// <returns> The XElement with the new volunteer</returns>
    private XElement createVolunteerElement(Volunteer item)
    {
        return new XElement("Volunteer", new XElement("Id", item.Id), new XElement("FullName", item.FullName),
                                                        new XElement("PhoneNumber",item.PhoneNumber),
                                                        new XElement("Email", item.Email), new XElement("Job", item.Job),
                                                        new XElement("Active", item.Active), new XElement("Distance", item.Distance),
                                                        new XElement("VolAddress", item.VolAddress), new XElement("Latitude", item.Latitude),
                                                        new XElement("Longitude", item.Longitude), new XElement("MaxDistance", item.MaxDistance));
    }
}
