using BlImplementation;
using BO;
using DalApi;
using DO;
using System.Net;
using System.Text.Json;

namespace Helpers;

internal static class CallManager
{
    private static IDal s_dal = Factory.Get; //stage 4
    internal static ObserverManager Observers = new(); //stage 5 
    private static readonly BlApi.ICall callImplementation = new CallImplementation();

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

        lock (AdminManager.BlMutex)
            if ((MaxCloseTime - AdminManager.Now) < s_dal.Config.RiskRange)
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
            lock (AdminManager.BlMutex)
                if ((currentCall.MaxTime - AdminManager.Now) < s_dal.Config.RiskRange)
                    return CallListStatus.InTreatmentInRisk;
            return CallListStatus.InTreatment;
        }
        return (BO.CallListStatus)MakeStatus(currentAssignment, currentCall.MaxTime);
    }

    /// <summary>
    /// Checks whether the call values ​​are logically correct
    /// </summary>
    /// <param name="toCheck">call to check</param>
    /// <returns>the corresponding call values ​​for the database</returns>
    /// <exception cref="BO.BlIntegrityOfValuesException">Throws an exception if one of the values ​​is logically incorrect</exception>
    internal static DO.Call CheckLogic(BO.Call toCheck)
    {
        bool currentMaxTime = (toCheck.MaxCloseTime > AdminManager.Now && toCheck.MaxCloseTime > toCheck.OpenTime) || toCheck.MaxCloseTime == null;

        if (currentMaxTime == false)
            throw new BO.BlIntegrityOfValuesException("""Error in value "MaxTime" integrity""");

        double[] AddressCoordinate = CallManager.GetCoordinates(toCheck.CallAddress);

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
    internal static double[] GetCoordinates(string address)
    {
        // Checking if the address is null or empty
        if (string.IsNullOrWhiteSpace(address))
            throw new BO.BlNullPropertyException("Address cannot be empty or null." + nameof(address));

        //Constructing the URL for the geocoding service with the provided address

        
        //string apiKey = "pk.9c3fc19e8792d781f9847563010296cd";
        //string url = $"https://geocode.maps.co/search?q={Uri.EscapeDataString(address)}&key={apiKey}";
        string url = $"https://geocode.maps.co/search?q={Uri.EscapeDataString(address)}";

        //// Creating a synchronous HTTP request
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "GET";
        //#pragma warning disable SYSLIB0014
        //        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        //        request.Method = "GET";
        //#pragma warning restore SYSLIB0014

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




    //public static double[] GetCoordinates(string address)
    //{

            //    // URL לשירות Geocode.maps.co (שאינו דורש API Key)
            //    string url = $"https://geocode.maps.co/search?q={Uri.EscapeDataString(address)}";

            //    using (HttpClient client = new HttpClient())
            //    {
            //        // שליחת בקשה GET לשירות
            //        HttpResponseMessage response = client.GetAsync(url).GetAwaiter().GetResult();

            //        // בדיקת סטטוס התשובה
            //        if (!response.IsSuccessStatusCode)
            //        {
            //            string errorResponse = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            //            Console.WriteLine($"Error Response: {errorResponse}");
            //            throw new Exception($"Error in request: {response.StatusCode}");
            //        }

            //        // קריאת גוף התשובה כטקסט
            //        string jsonResponse = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            //        // עיבוד התשובה ופרשנות JSON
            //        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            //        var results = JsonSerializer.Deserialize<LocationResult[]>(jsonResponse, options);

            //        // אם אין תוצאות
            //        if (results == null || results.Length == 0)
            //        {
            //            throw new Exception("No coordinates found for the given address.");
            //        }

            //        // החזרת הקואורדינטות
            //        return new double[] { double.Parse(results[0].Lat), double.Parse(results[0].Lon) };
            //    }
            //}


            // מחלקת עזר לפענוח JSON



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
    /// <exception cref="BO.BlXMLFileLoadCreateException">Thrown exception when there is problem to load the xml file</exception>
    internal static IEnumerable<BO.CallInList> ToCallInList(IEnumerable<DO.Call> oldCalls, IEnumerable<DO.Assignment> oldAssignment)
    {
        try
        {
            //IEnumerable<DO.Volunteer> oldVolunteer = s_dal.Volunteer.ReadAll(null); //can throw Ex
            List<BO.CallInList>? callInLists = new List<BO.CallInList>();

            foreach (DO.Call item in oldCalls)
            {
                DO.Assignment? callAssignment = oldAssignment.LastOrDefault(a => a.CallId == item.Id);

                
                if (callAssignment is null || callAssignment.VolunteerId == 0)
                {

                    CallListStatus _Status;
                    if ((item.MaxTime - AdminManager.Now) > AdminManager.MaxRange || item.MaxTime is null)
                        _Status = BO.CallListStatus.Opened;
                    else if ((item.MaxTime - AdminManager.Now) > TimeSpan.Zero)
                        _Status = BO.CallListStatus.OpenInRisk;
                    else
                        _Status = BO.CallListStatus.Expired;


                        callInLists.Add(new()
                    {
                        Id = null,
                        CallId = item.Id,
                        CallType = (BO.CallType)item.CallType,
                        OpenTime = item.OpenTime,
                        LeftTime = item.MaxTime != null ? (TimeSpan)(item.MaxTime - AdminManager.Now) : null,
                        LastVolunteer = null,
                        CompletionTime = null,
                        Status = _Status,
                        TotalAssignments = 0
                    });
                }

                else
                {
                    string? lastVolunteer;
                    lock (AdminManager.BlMutex)
                        lastVolunteer = s_dal.Volunteer.Read(callAssignment.VolunteerId)!.FullName;
                    callInLists.Add(new()
                    {
                        Id = callAssignment.Id,
                        CallId = item.Id,
                        CallType = (BO.CallType)item.CallType,
                        OpenTime = item.OpenTime,
                        LeftTime = item.MaxTime - AdminManager.Now,
                        LastVolunteer = lastVolunteer,
                        CompletionTime = (callAssignment.EndTreatment == DO.AssignmentEnum.TakenCare) ? (callAssignment.EndTime - item.OpenTime) : null,
                        Status = MakeStatus(callAssignment, item),
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
    /// <param name="callAssignment">The assignment of the call</param>
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
            EndTreatment = (BO.EndTreatment?)callAssignment.EndTreatment
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
        string volAddress;
        lock (AdminManager.BlMutex)
            volAddress = s_dal.Volunteer.Read(v => v.Id == volId).VolAddress;
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
    internal static bool CorrectDis(DO.Volunteer vol, double callLat, double callLon)
    {
        if (vol.MaxDistance == null)
            return true;
        if (vol.VolAddress != null)
        {
            //double lat = (double)vol.Latitude - callLat;
            //double lon = (double)vol.Longitude - callLon;
            const double R = 6371; // רדיוס כדור הארץ בקילומטרים

            // המרת מעלות לרדיאנים
            double ToRadians(double angle) => Math.PI * angle / 180.0;

            double phi1 = ToRadians((double)vol.Latitude);
            double phi2 = ToRadians(callLat);

            double deltaLat = ToRadians(callLat - (double)vol.Latitude);
            double deltaLon = ToRadians(callLon - (double)vol.Longitude);

            // חישוב המרחק בקירוב לשטח מישורי
            double x = deltaLon * Math.Cos((phi1 + phi2) / 2);
            double y = deltaLat;

            double distance = R * Math.Sqrt(x * x + y * y);
            return distance <= vol.MaxDistance;
            //lat = Math.Pow(lat, 2);
            //lon = Math.Pow(lon, 2);
            //lat += lon;
            //lat = Math.Sqrt(lat);
            //return lat <= vol.MaxDistance;
        }
        return true;
    }

    /// <summary>
    /// Goes through all calls whose maximum completion time has passed and whose 
    /// processing has not yet finished - and closes them with a completion type of "Expired":
    /// </summary>
    /// <param name="newClock">the time of the update</param>
    internal static void UpdateExpiredCalls(DateTime oldClock, DateTime newClock)
    {
        bool assignUpdated = false;
        lock (AdminManager.BlMutex)
        {
            var calls = s_dal.Call.ReadAll(m => m.MaxTime < newClock);
            //bool listUpdated = false;

            Func<BO.Call, bool> predicate = c => c.Status == CallStatus.InTreatment || c.Status == CallStatus.Opened || c.Status == CallStatus.OpenInRisk;
            IEnumerable<BO.Call> callsToUp = calls.Select(c => callImplementation.Read(c.Id)).Where(predicate);

            foreach (var call in callsToUp)
            {
                if (call.CallAssignments.Count == 0 || call.CallAssignments.Last().EndTime != null)
                {
                    assignUpdated = true;
                    //listUpdated = true;
                    s_dal.Assignment.Create(new(0, call.Id, 0, newClock, newClock, DO.AssignmentEnum.CancelExpired));
                    Observers.NotifyItemUpdated(call.Id);
                }
                else
                {
                    assignUpdated = true;
                    //listUpdated = true;
                    DO.Assignment assignToUp = s_dal.Assignment.Read(c => c.CallId == call.Id);
                    s_dal.Assignment.Update(new(assignToUp.Id, assignToUp.CallId, assignToUp.VolunteerId, assignToUp.InterTime, newClock, DO.AssignmentEnum.CancelExpired));
                    Observers.NotifyItemUpdated(assignToUp.Id);
                    Observers.NotifyItemUpdated(call.Id);
                }
            }
        }
        if (oldClock != newClock || assignUpdated)
        {
            Observers.NotifyListUpdated();
        }
    }


}