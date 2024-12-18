
using BO;
using DalApi;
using DO;
using System.Net;
using System.Text.Json;

namespace Helpers;

internal static class CallManager
{
    private static IDal s_dal = Factory.Get; //stage 4

    internal static CallStatus MakeStatus(IEnumerable<DO.Assignment> dataAssignments, DateTime? MaxCloseTime)
    {
        var endTreatment = from item in dataAssignments
                           select item.EndTreatment;
        var endTime = from item in dataAssignments
                      select item.EndTime;

        if (endTreatment.LastOrDefault() != DO.AssignmentEnum.TakenCare)
        {
            if ((MaxCloseTime - ClockManager.Now) < s_dal.Config.RiskRange)
                return CallStatus.OpenInRisk;
            else if (MaxCloseTime > ClockManager.Now)
                return CallStatus.Opened;
        }

        else if (endTime != null)
            return CallStatus.Closed;

        else if (endTime.Last() < ClockManager.Now) //to vz
            return CallStatus.Expired;
        return CallStatus.InTreatment;
    }

    internal static CallListStatus MakeStatus(DO.Assignment CurrentAssignment, DO.Call currentCall)
    {
        if (CurrentAssignment.EndTime != null)
            return CallListStatus.Closed;

        if (CurrentAssignment.EndTreatment == DO.AssignmentEnum.TakenCare)
        {
            if ((currentCall.MaxTime - ClockManager.Now) < s_dal.Config.RiskRange)
                return CallListStatus.InTreatmentInRisk;
            return CallListStatus.InTreatment;
        }

        if (CurrentAssignment.EndTreatment == DO.AssignmentEnum.CancelAdmin || CurrentAssignment.EndTreatment == DO.AssignmentEnum.SelfCancel)
        {
            if ((currentCall.MaxTime - ClockManager.Now) < s_dal.Config.RiskRange)
                return CallListStatus.OpenInRisk;
            return CallListStatus.Opened;
        }

        return CallListStatus.Expired;
    }

    internal static DO.Call CheckLogic(BO.Call toCheck)
    {
        bool currentMaxTime = (toCheck.MaxCloseTime > ClockManager.Now && toCheck.MaxCloseTime > toCheck.OpenTime) ||toCheck.MaxCloseTime==null;

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
    public static double[] GetCoordinates(string address)//לטפל בחריגות!!!!!
    {
        // Checking if the address is null or empty
        if (string.IsNullOrWhiteSpace(address))
            throw new BO.BlNullPropertyException("Address cannot be empty or null." + nameof(address));

        // Constructing the URL for the geocoding service with the provided address
        string url = $"https://geocode.maps.co/search?q={Uri.EscapeDataString(address)}";

        // Creating a synchronous HTTP request
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "GET";

        //try
        //{
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
            //}
        }
        //catch (WebException ex)
        //{
        //    // Handling web exceptions (e.g., network issues)
        //    throw new Exception("Error sending web request: " + ex.Message);
        //}
        //catch (Exception ex)
        //{
        //    // Handling general exceptions
        //    throw new Exception("General error: " + ex.Message);
        //}
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

    internal static string GetPropertyName(BO.CallData sortOrFilter)
    {
        return sortOrFilter switch
        {
            BO.CallData.Id => nameof(BO.CallInList.Id),
            //BO.CallData.CallId => nameof(BO.CallInList.CallId),
            BO.CallData.CallType => nameof(BO.CallInList.CallType),
            BO.CallData.OpenTime => nameof(BO.CallInList.OpenTime),
            BO.CallData.LeftTime => nameof(BO.CallInList.LeftTime),
            BO.CallData.LastVolunteer => nameof(BO.CallInList.LastVolunteer),
            BO.CallData.CompletionTime => nameof(BO.CallInList.CompletionTime),
            BO.CallData.Status => nameof(BO.CallInList.Status),
            BO.CallData.TotalAssignments => nameof(BO.CallInList.TotalAssignments),
            _ => nameof(BO.CallInList.Id)
        };
    }

    internal static string GetPropertyName(BO.CloseCallData sortOrFilter)
    {
        return sortOrFilter switch
        {
            BO.CloseCallData.Id => nameof(BO.CloseCallData.Id),
            BO.CloseCallData.CallType => nameof(BO.CloseCallData.CallType),
            BO.CloseCallData.CallAddress => nameof(BO.CloseCallData.CallAddress),
            BO.CloseCallData.OpenTime => nameof(BO.CloseCallData.OpenTime),
            BO.CloseCallData.InterTime => nameof(BO.CloseCallData.InterTime),
            BO.CloseCallData.CloseTime => nameof(BO.CloseCallData.CloseTime),
            BO.CloseCallData.EndTreatment => nameof(BO.CloseCallData.EndTreatment),
            _ => nameof(BO.CloseCallData.Id)
        };
    }

    internal static string GetPropertyName(BO.OpenCallData sortOrFilter)
    {
        return sortOrFilter switch
        {
            BO.OpenCallData.Id => nameof(BO.OpenCallData.Id),
            BO.OpenCallData.CallType => nameof(BO.OpenCallData.CallType),
            BO.OpenCallData.Description => nameof(BO.OpenCallData.Description),
            BO.OpenCallData.CallAddress => nameof(BO.OpenCallData.CallAddress),
            BO.OpenCallData.OpenTime => nameof(BO.OpenCallData.OpenTime),
            BO.OpenCallData.MaxCloseTime => nameof(BO.OpenCallData.MaxCloseTime),
            BO.OpenCallData.VolDistance => nameof(BO.OpenCallData.VolDistance),
            _ => nameof(BO.OpenCallData.Id)
        };
    }

    internal static string GetPropertyName(BO.CallType sortOrFilter)
    {
        return sortOrFilter switch
        {
            BO.CallType.Shopping => nameof(BO.CallType.Shopping),
            BO.CallType.Cleaning => nameof(BO.CallType.Cleaning),
            BO.CallType.Repairing => nameof(BO.CallType.Repairing),
            BO.CallType.TechnologyHelp => nameof(BO.CallType.TechnologyHelp),
            BO.CallType.Talking => nameof(BO.CallType.Talking),
            _ => nameof(BO.CallType.Shopping)
        };
    }



    //internal static string GetPropertyName(BO.CallListStatus sortOrFilter)
    //{
    //    return sortOrFilter switch
    //    {
    //        BO.CallListStatus.Opened => nameof(BO.CallListStatus.Opened),
    //        BO.CallListStatus.InTreatment => nameof(BO.CallListStatus.InTreatment),
    //        BO.CallListStatus.Expired => nameof(BO.CallListStatus.Expired),
    //        BO.CallListStatus.Closed => nameof(BO.CallListStatus.Closed),
    //        BO.CallListStatus.OpenInRisk => nameof(BO.CallListStatus.OpenInRisk),
    //        BO.CallListStatus.InTreatmentInRisk=> nameof(BO.CallListStatus.InTreatmentInRisk),
    //        _=> nameof(BO.CallListStatus.Opened)
    //    };
    //}

    internal static IEnumerable<BO.CallInList> ToCallInList(IEnumerable<DO.Call> oldCalls, IEnumerable<DO.Assignment> oldAssignment)
    {
        IEnumerable<DO.Volunteer> oldVolunteer = s_dal.Volunteer.ReadAll(null);
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

    
    internal static BO.ClosedCallInList ToCloseCall(DO.Call item, BO.CallAssignInList CallAssignment)
    {

        return new()
        {
            Id = item.Id,
            CallType = (BO.CallType)item.CallType,
            FullAddress = item.CallAddress,
            OpenTime = item.OpenTime,
            InterTime = CallAssignment.InterTime,
            CloseTime = CallAssignment.EndTime,
            EndTreatment = (BO.EndTreatment)CallAssignment.EndTreatment
        };
    }

    internal static BO.OpenCallInList ToOpenCall(DO.Call item, BO.CallAssignInList CallAssignment)
    {
        Func<DO.Volunteer, bool> predicate = volunteer => volunteer.Id == CallAssignment.VolunteerId;
        //DO.Volunteer vol= s_dal.Volunteer.Read(predicate)??throw new BO. //מה המתודה עושה???
        string volAddress = s_dal.Volunteer.Read(predicate).VolAddress;
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