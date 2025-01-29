using BlImplementation;
using BO;
using DalApi;
using DO;
using System.Net;
using System.Text.Json;

namespace Helpers;

internal static class CallManager
{
    private static IDal _dal = Factory.Get; //stage 4
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
            if ((MaxCloseTime - AdminManager.Now) < _dal.Config.RiskRange)
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
                if ((currentCall.MaxTime - AdminManager.Now) < _dal.Config.RiskRange)
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
    //internal static DO.Call CheckLogic(BO.Call toCheck)
    //{
    //    bool currentMaxTime = (toCheck.MaxCloseTime > AdminManager.Now && toCheck.MaxCloseTime > toCheck.OpenTime) || toCheck.MaxCloseTime == null;

    //    if (currentMaxTime == false)
    //        throw new BO.BlIntegrityOfValuesException("""Error in value "MaxTime" integrity""");

    //    double[] AddressCoordinate = CallManager.GetCoordinates(toCheck.CallAddress);

    //    DO.Call DoCall = new(toCheck.Id, (DO.TypeOfCall)toCheck.CallType, toCheck.CallAddress, AddressCoordinate[0],
    //        AddressCoordinate[1], toCheck.OpenTime, toCheck.Description, toCheck.MaxCloseTime);

    //    return DoCall;
    //}

    /// <summary>
    /// This method takes an address as input and returns an array with the latitude and longitude.
    /// The request is synchronous, meaning it waits for the response before continuing.
    /// </summary>
    /// <param name="address">The address to be geocoded</param>
    /// <returns>A double array containing the latitude and longitude</returns>
    //internal static double[] GetCoordinates(string address)
    //{
    //    // Checking if the address is null or empty
    //    if (string.IsNullOrWhiteSpace(address))
    //        throw new BO.BlNullPropertyException("Address cannot be empty or null." + nameof(address));

    //    //Constructing the URL for the geocoding service with the provided address

    //    string apiKey = "pk.902e35436f8c579a871d8158aee1bde2";
    //    //string apiKey = "pk.9c3fc19e8792d781f9847563010296cd";
    //    string url = $"https://us1.locationiq.com/v1/search?key={apiKey}&q={Uri.EscapeDataString(address)}&format=json";
    //    //string url = $"https://geocode.maps.co/search?q={Uri.EscapeDataString(address)}";

    //    using (HttpClient client = new HttpClient())
    //    {
    //        // שליחת בקשה סינכרונית (חסימתית)
    //        HttpResponseMessage response = client.GetAsync(url).Result;

    //        // בדיקת מצב התשובה
    //        if (!response.IsSuccessStatusCode)
    //            throw new BO.BlIntegrityOfValuesException($"Error in request: {response.StatusCode}");

    //        // קריאת התוכן כסטרינג
    //        string jsonResponse = response.Content.ReadAsStringAsync().Result;

    //        // פענוח התוכן מ-JSON
    //        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    //        var results = JsonSerializer.Deserialize<LocationResult[]>(jsonResponse, options);

    //        // בדיקה אם נמצאו תוצאות
    //        if (results == null || results.Length == 0)
    //            throw new BO.BlIntegrityOfValuesException("Wrong Address. No coordinates found for the given address.");

    //        // החזרת קואורדינטות
    //        return new double[] { double.Parse(results[0].Lat), double.Parse(results[0].Lon) };
    //    }
    //}

    internal static void CheckFormat(BO.Call toCheck)
    {
        bool currentMaxTime = (toCheck.MaxCloseTime > AdminManager.Now && toCheck.MaxCloseTime > toCheck.OpenTime) || toCheck.MaxCloseTime == null;

        if (currentMaxTime == false)
            throw new BO.BlIntegrityOfValuesException("""Error in value "MaxTime" integrity""");
    }
    internal static async Task updateCoordinates(DO.Call doCall)
    {
        double?[] AddressCoordinate = await CallManager.GetCoordinates(doCall.CallAddress);

        if (AddressCoordinate is null)
            throw new BO.BlIntegrityOfValuesException("Wrong Address. No coordinates found for the given address.");

        //DO.Call DoCall = new(toCheck.Id, (DO.TypeOfCall)toCheck.CallType, toCheck.CallAddress, (double)AddressCoordinate[0]!,
        //    (double)AddressCoordinate[1]!, toCheck.OpenTime, toCheck.Description, toCheck.MaxCloseTime);
        //return DoCall;
        lock (AdminManager.BlMutex)
            _dal.Call.Update(
                new(doCall.Id,
                doCall.CallType,
                doCall.CallAddress,
                (double)AddressCoordinate[0]!,
                (double)AddressCoordinate[1]!,
                doCall.OpenTime,
                doCall.Description,
                doCall.MaxTime));
    }
    internal static async Task<double?[]> GetCoordinates(string address)
    {
        // Checking if the address is null or empty
        if (string.IsNullOrWhiteSpace(address))
            throw new BO.BlNullPropertyException("Address cannot be empty or null." + nameof(address));

        //Constructing the URL for the geocoding service with the provided address
        string apiKey = "pk.902e35436f8c579a871d8158aee1bde2";
        string url = $"https://us1.locationiq.com/v1/search?key={apiKey}&q={Uri.EscapeDataString(address)}&format=json";

        using (HttpClient client = new HttpClient())
        {
            // Sending asynchronous request
            HttpResponseMessage response = await client.GetAsync(url);

            // Checking the response status
            if (!response.IsSuccessStatusCode)
                throw new BO.BlIntegrityOfValuesException($"Error in request: {response.StatusCode}");

            // Reading the content asynchronously
            string jsonResponse = await response.Content.ReadAsStringAsync();

            // Deserializing the JSON content
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var results = JsonSerializer.Deserialize<LocationResult[]>(jsonResponse, options);

            // Checking if results are found
            if (results == null || results.Length == 0)
                throw new BO.BlIntegrityOfValuesException("Wrong Address. No coordinates found for the given address.");

            // Returning the coordinates
            return new double?[] { double.Parse(results[0].Lat), double.Parse(results[0].Lon) };
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
    /// <exception cref="BO.BlXMLFileLoadCreateException">Thrown exception when there is problem to load the xml file</exception>
    internal static IEnumerable<BO.CallInList> ToCallInList(IEnumerable<DO.Call> oldCalls, IEnumerable<DO.Assignment> oldAssignment)
    {
        try
        {
            //IEnumerable<DO.Volunteer> oldVolunteer = _dal.Volunteer.ReadAll(null); //can throw Ex
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
                        lastVolunteer = _dal.Volunteer.Read(callAssignment.VolunteerId)!.FullName;
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
            volAddress = _dal.Volunteer.Read(v => v.Id == volId).VolAddress;
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
    #region stage 7
    internal static IEnumerable<BO.OpenCallInList> GetOpenedCalls(int volId, BO.CallType? filter = null, BO.OpenCallData? sort = null)
    {
        lock (AdminManager.BlMutex)
        {
            DO.Volunteer vol;
            IEnumerable<DO.Call> oldCalls;
            lock (AdminManager.BlMutex)
            {
                vol = _dal.Volunteer.Read(volId) ?? throw new BO.BlDoesNotExistException($"There is no volunteer with ID:{volId}");
                oldCalls = _dal.Call.ReadAll(null);
            }
            var openCalls = from item in oldCalls
                            let tmpCall = Read(item.Id)
                            where (tmpCall.Status == BO.CallStatus.OpenInRisk || tmpCall.Status == BO.CallStatus.Opened) && CallManager.CorrectDis(vol, item.Latitude, item.Longitude)
                            select CallManager.ToOpenCall(item, volId); //can throw Ex


            openCalls = null == filter ? openCalls : from call in openCalls
                                                     where (BO.CallType)filter == call.CallType
                                                     select call;

            openCalls = null == sort ? openCalls.OrderBy(c => c.Id)
                : openCalls.OrderBy<BO.OpenCallInList, object>(call => (sort switch
                {
                    BO.OpenCallData.Id => call.Id,
                    BO.OpenCallData.CallType => call.CallType,
                    BO.OpenCallData.Description => call.Description,
                    BO.OpenCallData.CallAddress => call.FullAddress,
                    BO.OpenCallData.OpenTime => call.OpenTime,
                    BO.OpenCallData.MaxCloseTime => call.MaxCloseTime,
                    BO.OpenCallData.VolDistance => call.VolDistance,
                    _ => call.Id
                }));

            return openCalls;
        }

    }

    internal static BO.Call Read(int callId)
    {
        lock (AdminManager.BlMutex)
        {
            var doCall = _dal.Call.Read(callId) ?? throw new BO.BlDoesNotExistException($"Call with ID={callId} does Not exist");

            Func<DO.Assignment, bool> func = item => item.CallId == callId;

            IEnumerable<DO.Assignment> dataAssignments = _dal.Assignment.ReadAll(func); //read all assignment of this Call.
            var callAssignments = new List<BO.CallAssignInList>();
            if (dataAssignments.Count() != 0)
            {
                callAssignments = dataAssignments.Select(assign => new BO.CallAssignInList
                {
                    VolunteerId = assign.VolunteerId,
                    VolunteerName = (assign.VolunteerId != 0) ? _dal.Volunteer.Read(assign.VolunteerId).FullName : null,
                    InterTime = assign.InterTime,
                    EndTime = assign.EndTime.HasValue ? assign.EndTime : null,
                    EndTreatment = assign.EndTreatment.HasValue ? (BO.EndTreatment)assign.EndTreatment : null, /*BO.EndTreatment.None,*/

                }).ToList();
            }

            var myStatus = dataAssignments.Count() != 0 ? CallManager.MakeStatus(dataAssignments.Last(), doCall.MaxTime) : BO.CallStatus.Opened;//הNULL מנוהל
            return new()
            {
                Id = callId,
                CallType = (BO.CallType)doCall.CallType,
                Description = doCall.Description,
                CallAddress = doCall.CallAddress,
                Latitude = doCall.Latitude,
                Longitude = doCall.Longitude,
                OpenTime = doCall.OpenTime,
                MaxCloseTime = doCall.MaxTime,
                Status = dataAssignments.Count() != 0 ? CallManager.MakeStatus(dataAssignments.Last(), doCall.MaxTime) : BO.CallStatus.Opened, //הNULL מנוהל
                CallAssignments = callAssignments
            };
        }
    }
    internal static void CallToTreatment(int volId, int callId)
    {
        BO.Call call = Read(callId) ?? throw new BO.BlDoesNotExistException($"Call with ID={callId} does not exists");
        Func<DO.Assignment, bool>? predicate = assignment => assignment.CallId == callId && assignment.EndTreatment != DO.AssignmentEnum.SelfCancel
                                                             && assignment.EndTreatment != DO.AssignmentEnum.CancelAdmin;
        IEnumerable<DO.Assignment> assignments;
        lock (AdminManager.BlMutex)
            assignments = _dal.Assignment.ReadAll(predicate).ToList();
        if (assignments.Count() != 0)
            throw new BO.BlDoesAlreadyExistException($"Assignment for call with ID={callId} already exists");
        if (call.Status != BO.CallStatus.InTreatment && call.Status != BO.CallStatus.Expired && call.Status != BO.CallStatus.Closed)
        {
            lock (AdminManager.BlMutex)
                _dal.Assignment.Create(new(0, callId, volId, AdminManager.Now, null, null));
            CallManager.Observers.NotifyItemUpdated(call.Id);  //stage 5
            VolunteerManager.Observers.NotifyItemUpdated(volId);  //stage 5
            CallManager.Observers.NotifyListUpdated(); //stage 5
        }
        else
            throw new BO.BlCantHandleItException($"Unable to assign");
    }

    internal static void UpdateEndTreatment(int volId, int assignmentId)
    {
        try
        {
            DO.Assignment updateAssignment;
            lock (AdminManager.BlMutex)
            {
                DO.Assignment assignment = _dal.Assignment.Read(assignmentId) ?? throw new BO.BlDoesNotExistException($"Assignment with ID={assignmentId} does not exists"); //can throw Ex
                if (volId == assignment.VolunteerId && assignment.EndTreatment == null)
                {
                    updateAssignment = new(assignmentId, assignment.CallId, volId,
                    assignment.InterTime, AdminManager.Now, DO.AssignmentEnum.TakenCare);
                    _dal.Assignment.Update(updateAssignment); //can throw Ex
                }
                else
                    throw new BO.BlCantUpdateException($"Assignment with ID: {assignmentId} cannot be closed");
            }
            CallManager.Observers.NotifyItemUpdated(updateAssignment.Id);  //stage 5
            CallManager.Observers.NotifyListUpdated();  //stage 5
            VolunteerManager.Observers.NotifyListUpdated(); //stage 5
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Assignment with ID={assignmentId} does not exists", ex);
        }
        catch (DO.DalXMLFileLoadCreateException ex)
        {
            throw new BO.BlXMLFileLoadCreateException("Xml Error", ex);
        }
    }

    internal static void GetAssignmentToEnd(int volId, int callId)
    {
        int assignId;
        lock (AdminManager.BlMutex)
        {
            DO.Assignment assignment = _dal.Assignment.ReadAll().Where(a => a.CallId == callId).LastOrDefault() ?? throw new BO.BlDoesNotExistException($"Call with ID={callId} does not have a assignment");
            assignId = assignment.Id;
        }
        UpdateEndTreatment(volId, assignId);
    }

    internal static void UpdateCancelTreatment(int volId, int assignmentId)
    {
        try
        {
            DO.Assignment assignment;
            lock (AdminManager.BlMutex)
            {
                assignment = _dal.Assignment.Read(assignmentId) ?? throw new BO.BlDoesNotExistException($"Assignment with ID={assignmentId} does not exists"); //can throw Ex
                DO.Volunteer volunteer = _dal.Volunteer.Read(volId) ?? throw new BO.BlDoesNotExistException($"volunteer with ID={volId} does not exists");
                if ((volId == assignment.VolunteerId || volunteer.Job == DO.Role.Manager) && assignment.EndTime is null)
                {
                    DO.Assignment updateAssignment;
                    if (volId == assignment.VolunteerId)
                    {
                        updateAssignment = new(assignmentId, assignment.CallId, volId,
                            assignment.InterTime, AdminManager.Now, DO.AssignmentEnum.SelfCancel);
                    }
                    else
                    {
                        updateAssignment = new(assignmentId, assignment.CallId, volId,
                           assignment.InterTime, AdminManager.Now, DO.AssignmentEnum.CancelAdmin);
                    }
                    _dal.Assignment.Update(updateAssignment); //can throw Ex
                }
                else
                    throw new BO.BlCantUpdateException($"Assignment with ID: {assignmentId} cannot be canceled");
            }

            CallManager.Observers.NotifyItemUpdated(assignment.CallId);  //stage 5
            CallManager.Observers.NotifyListUpdated();  //stage 5
            VolunteerManager.Observers.NotifyListUpdated(); //stage 5
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"item with ID: {assignmentId} does not exists", ex);
        }
        catch (DO.DalXMLFileLoadCreateException ex)
        {
            throw new BO.BlXMLFileLoadCreateException("Xml Error", ex);
        }
    }

    internal static void GetAssignmentToCancel(int volId, int callId)
    {
        int assignId;
        lock (AdminManager.BlMutex)
        {
            DO.Assignment? assignment = _dal.Assignment.ReadAll().Where(a => a.CallId == callId).LastOrDefault() ?? throw new BO.BlDoesNotExistException($"Call with ID={callId} does not have a assignment");
            assignId = assignment.Id;
        }
        UpdateCancelTreatment(volId, assignId);
    }

    #endregion
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
        IEnumerable<DO.Call> calls;
        DO.Assignment assignToUp;

        lock (AdminManager.BlMutex)
            calls = _dal.Call.ReadAll(m => m.MaxTime < newClock).ToList();
        //bool listUpdated = false;

        Func<BO.Call, bool> predicate = c => c.Status == CallStatus.InTreatment || c.Status == CallStatus.Opened || c.Status == CallStatus.OpenInRisk;
        IEnumerable<BO.Call> callsToUp = calls.Select(c => callImplementation.Read(c.Id)).Where(predicate).ToList();

        foreach (var call in callsToUp)
        {
            if (call.CallAssignments.Count == 0 || call.CallAssignments.Last().EndTime != null)
            {
                assignUpdated = true;
                //listUpdated = true;
                lock (AdminManager.BlMutex)
                    _dal.Assignment.Create(new(0, call.Id, 0, newClock, newClock, DO.AssignmentEnum.CancelExpired));
                Observers.NotifyItemUpdated(call.Id);
            }
            else
            {
                assignUpdated = true;
                //listUpdated = true;
                lock (AdminManager.BlMutex)
                {
                    assignToUp = _dal.Assignment.Read(c => c.CallId == call.Id); ///קורא לראשונה או לאחרונה?אם לאחרונה צריך לשנות
                    _dal.Assignment.Update(new(assignToUp.Id, assignToUp.CallId, assignToUp.VolunteerId, assignToUp.InterTime, newClock, DO.AssignmentEnum.CancelExpired));
                }
                Observers.NotifyItemUpdated(assignToUp.Id);
                Observers.NotifyItemUpdated(call.Id);
            }
        }

        if (oldClock != newClock || assignUpdated)
        {
            Observers.NotifyListUpdated();
        }
    }
}