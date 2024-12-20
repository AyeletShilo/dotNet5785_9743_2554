
using BO;
using DalApi;
using DO;
using System.Net;
using System.Text.Json;

namespace Helpers;

internal static class CallManager
{
    private static IDal s_dal = Factory.Get; //stage 4

    /// <summary>
    /// Computes and returns call status
    /// </summary>
    /// <param name="dataAssignments">current assignment of the call</param>
    /// <param name="MaxCloseTime">Maximum call completion time</param>
    /// <returns>call status</returns>
    internal static CallStatus MakeStatus(DO.Assignment dataAssignments, DateTime? MaxCloseTime)
    {
        var endTreatment = dataAssignments.EndTreatment;                          
        DateTime? endTime = dataAssignments.EndTime;

        if (endTreatment == DO.AssignmentEnum.CancelExpired) 
            return CallStatus.Expired;

        else if (endTime != null && endTreatment != DO.AssignmentEnum.CancelAdmin && endTreatment != DO.AssignmentEnum.SelfCancel) 
            return CallStatus.Closed;

        if (endTreatment == null && dataAssignments != null)
            return CallStatus.InTreatment;

        if ((MaxCloseTime - ClockManager.Now) < s_dal.Config.RiskRange)
            return CallStatus.OpenInRisk;
 
        return CallStatus.Opened;
    }

    /// <summary>
    /// Computes and returns call status for call in list
    /// </summary>
    /// <param name="currentAssignment">current assignment of the call</param>
    /// <param name="currentCall">The call</param>
    /// <returns>call status</returns>
    internal static CallListStatus MakeStatus(DO.Assignment currentAssignment, DO.Call currentCall)
    {
        
        if (currentAssignment.EndTreatment == null && currentAssignment != null)
        {
            if ((currentCall.MaxTime - ClockManager.Now) < s_dal.Config.RiskRange)
                return CallListStatus.InTreatmentInRisk;
            return CallListStatus.InTreatment;
        }
        return (BO.CallListStatus)MakeStatus(currentAssignment, currentCall.MaxTime);
    }

    /// <summary>
    /// Checks whether the call values ​​are logically correct
    /// </summary>
    /// <param name="toCheck">cakk to check</param>
    /// <returns>the corresponding call values ​​for the database</returns>
    /// <exception cref="BO.BlIntegrityOfValuesException">Throws an exception if one of the values ​​is logically incorrect</exception>
    internal static DO.Call CheckLogic(BO.Call toCheck)
    {
        bool currentMaxTime = (toCheck.MaxCloseTime > ClockManager.Now && toCheck.MaxCloseTime > toCheck.OpenTime) || toCheck.MaxCloseTime == null;

        if (currentMaxTime == false)
            throw new BO.BlIntegrityOfValuesException("""Error in value "MaxTime" integrity""");

        double[] AddressCoordinate = CallManager.GetCoordinates(toCheck.CallAddress); //לסדר חריגות פה

        DO.Call DoCall = new(toCheck.Id, (DO.TypeOfCall)toCheck.CallType, toCheck.CallAddress, AddressCoordinate[0],
            AddressCoordinate[1], toCheck.OpenTime, toCheck.Description, toCheck.MaxCloseTime);

        return DoCall;
    }


    /// <summary>
    /// This method takes an address as input and returns an array with the latitude and longitude.
    /// The request is synchronous, meaning it waits for the response before continuing.
    /// </summary>
    /// <param name="address">The address to be geocoded</param>
    /// <returns>A double array containing the latitude and longitude</returns>
    public static double[] GetCoordinates(string address)
    {
        // Checking if the address is null or empty
        if (string.IsNullOrWhiteSpace(address))
            throw new BO.BlNullPropertyException("Address cannot be empty or null." + nameof(address));

        // Constructing the URL for the geocoding service with the provided address
        string url = $"https://geocode.maps.co/search?q={Uri.EscapeDataString(address)}";

        // Creating a synchronous HTTP request
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "GET";

        // Sending the request and getting the response synchronously
        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        {
            // Checking if the response status is OK
            if (response.StatusCode != HttpStatusCode.OK)
                throw new BO.BlIntegrityOfValuesException($"Error in request: {response.StatusCode}");

            // Reading the response body as a string
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string jsonResponse = reader.ReadToEnd();

                // Deserializing the JSON response to extract location data
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var results = JsonSerializer.Deserialize<LocationResult[]>(jsonResponse, options);

                // If no results are found, throwing an exception
                if (results == null || results.Length == 0)
                    throw new BO.BlIntegrityOfValuesException($"Wrong Address. No coordinates found for the given address.");

                // Returning the latitude and longitude as an array
                return new double[] { double.Parse(results[0].Lat), double.Parse(results[0].Lon) };
            }
           
        }
    }

    /// <summary>
    /// Class to represent the structure of the geocoding response (latitude and longitude)
    /// </summary>
    private class LocationResult
    {
        // Latitude as string in the JSON response
        public string Lat { get; set; }
        // Longitude as string in the JSON response
        public string Lon { get; set; }
    }


    /// <summary>
    /// Converts a list of calls from the database to the data entity form-Call in list
    /// </summary>
    /// <param name="oldCalls"> A list of calls from the database</param>
    /// <param name="oldAssignment">list of all the assignment from database</param>
    /// <returns>List of Call in list</returns>
    /// <exception cref="BO.BlNullPropertyException">Thrown exception when  there is assignment for call without volunteer<</exception>
    /// <exception cref="BO.BlXMLFileLoadCreateException">Thrown exception when there is problemto load the xml file</exception>
    internal static IEnumerable<BO.CallInList> ToCallInList(IEnumerable<DO.Call> oldCalls, IEnumerable<DO.Assignment> oldAssignment)
    {
        try
        {
            IEnumerable<DO.Volunteer> oldVolunteer = s_dal.Volunteer.ReadAll(null); //can throw Ex
            List<BO.CallInList>? callInLists = new List<BO.CallInList>();
            foreach (DO.Call item in oldCalls)
            {
                DO.Assignment? CallAssignment = oldAssignment.LastOrDefault(a => a.CallId == item.Id);

                if (CallAssignment is null)
                {
                    callInLists.Add(new()
                    {
                        Id = null,
                        CallId = item.Id,
                        CallType = (BO.CallType)item.CallType,
                        OpenTime = item.OpenTime,
                        LeftTime = (TimeSpan)(item.MaxTime - ClockManager.Now),
                        LastVolunteer = null,
                        CompletionTime = null,
                        Status = BO.CallListStatus.Opened,
                        TotalAssignments = 0
                    });
                }

                else
                {
                    DO.Volunteer? first = oldVolunteer.FirstOrDefault(v => v.Id == CallAssignment.VolunteerId) ?? throw new BO.BlNullPropertyException("Cannot use a null attribute value.");
                    callInLists.Add(new()
                    {
                        Id = CallAssignment.Id,
                        CallId = item.Id,
                        CallType = (BO.CallType)item.CallType,
                        OpenTime = item.OpenTime,
                        LeftTime = item.MaxTime - ClockManager.Now,
                        LastVolunteer = first.FullName,
                        CompletionTime = (CallAssignment.EndTreatment == DO.AssignmentEnum.TakenCare) ? (CallAssignment.EndTime - item.OpenTime) : null,
                        Status = MakeStatus(CallAssignment, item),
                        TotalAssignments = oldAssignment.Count(a => a.CallId == item.Id)
                    });
                }
            };
            return callInLists.AsEnumerable();
        }
        catch (DO.DalXMLFileLoadCreateException ex)
        {
            throw new BO.BlXMLFileLoadCreateException("Xml Error", ex);
        }
    }

    /// <summary>
    /// Converts a call from the database to the data entity form-close call in list
    /// </summary>
    /// <param name="item">The call to convert</param>
    /// <param name="callAssignment">The assigment of the call</param>
    /// <returns>close call in list</returns>
    internal static BO.ClosedCallInList ToCloseCall(BO.Call item, BO.CallAssignInList callAssignment)
    {
        return new()
        {
            Id = item.Id,
            CallType = (BO.CallType)item.CallType,
            FullAddress = item.CallAddress,
            OpenTime = item.OpenTime,
            InterTime = callAssignment.InterTime,
            CloseTime = callAssignment.EndTime,
            EndTreatment = (BO.EndTreatment)callAssignment.EndTreatment
        };
    }

    /// <summary>
    /// Converts a call from the database to the data entity form-open call in list
    /// </summary>
    /// <param name="item">The call to convert<</param>
    /// <param name="volId">The volunteer to whom the call was assigned</param>
    /// <returns> open call in list</returns>
    internal static BO.OpenCallInList ToOpenCall(DO.Call item, int volId)
    {
        string volAddress = s_dal.Volunteer.Read(v => v.Id == volId).VolAddress; 
        return new()
        {
            Id = item.Id,
            CallType = (BO.CallType)item.CallType,
            Description = item.Description,
            FullAddress = item.CallAddress,
            OpenTime = item.OpenTime,
            MaxCloseTime = item.MaxTime,
            VolDistance = VolunteerManager.CalculateDis(volAddress, item.CallAddress)
        };
    }
}