namespace Dal;

using DalApi;
using DalXml;
using DO;
using System;
using System.Linq;
using System.Xml.Linq;

internal class VolunteerImplementation : IVolunteer
{
    static Volunteer getVolunteer(XElement v)
    {
        return new DO.Volunteer()
        {
            Id = v.ToIntNullable("Id") ?? throw new FormatException("can't convert id"),
            FullName = (string?)v.Element("FullName") ?? " ",
            PhoneNumber = (string?)v.Element("PhoneNumber") ?? " ",
            Email = (string?)v.Element("Email") ?? " ",
            Job = v.ToEnumNullable<Role>("Job") ?? Role.Donater,
            Active = (bool?)v.Element("Active") ?? false,
            Distance = v.ToEnumNullable<RangeType>("Distance") ?? RangeType.Walking,
            VolAddress = (string?)v.Element("VolAddress") ?? null,
            Latitude = (double?)v.Element("Latitude") ?? null,
            Longitude = (double?)v.Element("Longitude") ?? null,
            MaxDistance = (double?)v.Element("MaxDistance") ?? null,
        };
    }



    public void Create(Volunteer item)
    {
        XElement newVolunteer= new XElement("Volunteer");
        newVolunteer.Add(item.Id, item.FullName, item.PhoneNumber, item.Email, item.Job, item.Active, item.Distance, item.VolAddress, 
            item.Latitude,item.Longitude, item.MaxDistance);
        newVolunteer.Save(Config.s_volunteers_xml);
    }

    public void Delete(int id)
    {
        XElement? elemToDelete = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().FirstOrDefault(vol=> (int?)vol.Element("Id")==id);
        if(elemToDelete!=null)
            elemToDelete.Remove();
        XMLTools.SaveListToXMLElement(elemToDelete,Config.s_volunteers_xml);
        //elemToDelete.Save(Config.s_volunteers_xml);
    }

    public void DeleteAll()
    {
        XElement? resetVol = new XElement("Volunteer");
        XMLTools.SaveListToXMLElement(resetVol, Config.s_volunteers_xml);
       // XElement? elemToDelete= XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().FirstOrDefault
    }

    public Volunteer? Read(int id)
    {
        XElement? volunteerElem =
XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().FirstOrDefault(vt => (int?)vt.Element("Id") == id);
        return volunteerElem is null ? null : getVolunteer(volunteerElem);
    }

    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().Select(v => getVolunteer(v)).FirstOrDefault(filter);
    }

    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().Select(v => getVolunteer(v)).Where(filter);
    }

    public void Update(Volunteer item)
    { 
        XElement volunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);

        (volunteersRootElem.Elements().FirstOrDefault(st => (int?)st.Element("Id") == item.Id)
        ?? throw new DO.DalDoesNotExistException($"Volunteer with ID={item.Id} does Not exist"))
                .Remove();

        //volunteersRootElem.Add(new XElement()
        volunteersRootElem.Add(new XElement(/*"Volunteer",*/ createVolunteerElement(item)));

        XMLTools.SaveListToXMLElement(volunteersRootElem, Config.s_volunteers_xml);
    }
    private XElement createVolunteerElement(Volunteer item)
    {
        return new XElement("Volunteer", new XElement("Id", item.Id), new XElement("FullName", item.FullName),
                                                        new XElement("Email", item.Email), new XElement("Job", item.Job),
                                                        new XElement("Active", item.Active), new XElement("Distance", item.Distance),
                                                        new XElement("VolAddress", item.VolAddress), new XElement("Latitude", item.Latitude),
                                                        new XElement("Longitude", item.Longitude), new XElement("MaxDistance", item.MaxDistance));
    }
}
