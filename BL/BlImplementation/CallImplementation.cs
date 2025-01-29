using BlApi;
using BO;
using DalApi;
using DO;
using Helpers;
using System.Globalization;
using System.Linq;


namespace BlImplementation;

internal class CallImplementation : BlApi.ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    /// <summary>
    /// Gets a volunteer number and a call, and if there is no open assignment of the call, 
    /// it creates a new assignment of the requesting volunteer and the call
    /// </summary>
    /// <param name="volId"> ID of a volunteer requesting to assign a call</param>
    /// <param name="callId"> Call requested for allocation.</param>
    /// <exception cref="BO.BlDoesNotExistException"></exception>
    /// <exception cref="BO.BlDoesAlreadyExistException"></exception>
    /// <exception cref="BO.BlCantHandleItException"></exception>
    public void CallToTreatment(int volId, int callId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        CallManager.CallToTreatment(volId, callId);
        #region draft
        //AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        //BO.Call call = Read(callId) ?? throw new BO.BlDoesNotExistException($"Call with ID={callId} does not exists");
        //Func<DO.Assignment, bool>? predicate = assignment => assignment.CallId == callId && assignment.EndTreatment != DO.AssignmentEnum.SelfCancel
        //                                                     && assignment.EndTreatment != DO.AssignmentEnum.CancelAdmin;
        //IEnumerable<DO.Assignment> assignments;
        //lock (AdminManager.BlMutex)
        //    assignments = _dal.Assignment.ReadAll(predicate).ToList();
        //if (assignments.Count() != 0)
        //    throw new BO.BlDoesAlreadyExistException($"Assignment for call with ID={callId} already exists");
        //if (call.Status != BO.CallStatus.InTreatment && call.Status != BO.CallStatus.Expired && call.Status != BO.CallStatus.Closed)
        //{
        //    lock (AdminManager.BlMutex)
        //        _dal.Assignment.Create(new(0, callId, volId, AdminManager.Now, null, null));
        //    CallManager.Observers.NotifyItemUpdated(call.Id);  //stage 5
        //    VolunteerManager.Observers.NotifyItemUpdated(volId);  //stage 5
        //    CallManager.Observers.NotifyListUpdated(); //stage 5
        //}
        //else
        //    throw new BO.BlCantHandleItException($"Unable to assign");
        #endregion
    }

    /// <summary>
    /// Gets BO.Call object and asks the dl to add it to the call data.
    /// </summary>
    /// <param name="callToAdd"></param>
    public void Create(BO.Call callToAdd)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        CallManager.CheckFormat(callToAdd);
        DO.Call oldCall;
        lock (AdminManager.BlMutex)
            oldCall = _dal.Call.Read(callToAdd.Id)!;
        //DO.Call doCall = CallManager.CheckLogic(callToAdd); //can throw Ex
        DO.Call doCall = new(callToAdd.Id, (DO.TypeOfCall)callToAdd.CallType, callToAdd.CallAddress, 0, 0, callToAdd.OpenTime, callToAdd.Description, callToAdd.MaxCloseTime); //CallManager.CheckLogic(callToUpdate); //can throw Ex
        try
        {
            lock (AdminManager.BlMutex)
                _dal.Call.Create(doCall); //can throw Ex
        }
        catch (DO.DalXMLFileLoadCreateException ex)
        {
            throw new BO.BlXMLFileLoadCreateException("Xml Error", ex);
        }
        CallManager.Observers.NotifyListUpdated();  //stage 5
        try
        {
            //compute the coordinates asynchronously without waiting for the results
            _ = CallManager.updateCoordinates(doCall); //stage 7
        }
        catch (BO.BlIntegrityOfValuesException ex)
        {
            lock (AdminManager.BlMutex)
                _dal.Call.Update(oldCall);
            CallManager.Observers.NotifyListUpdated();  //stage 5
            throw new BO.BlIntegrityOfValuesException("Wrong Address. No coordinates found for the given address.");
        }
    }

    /// <summary>
    /// Ask the dl to check if the call can be deleted.
    /// </summary>
    /// <param name="id"> id of the call that needs to be deleted</param>
    /// <exception cref="BO.CannotBeDeletedException"></exception>
    /// <exception cref="BO.BlDoesNotExisException"></exception>
    /// <exception cref="BO.BLCannotBeDeletedException"></exception>
    public void Delete(int callId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        try
        {
            BO.Call toDelete = Read(callId) ?? throw new BO.BlDoesNotExistException($"Call with ID={callId} does Not exist");
            if ((toDelete.Status == BO.CallStatus.Opened || toDelete.Status == BO.CallStatus.OpenInRisk) && toDelete.CallAssignments?.Count == 0)
            {
                lock (AdminManager.BlMutex)
                    _dal.Call.Delete(callId); //can throw Ex
                CallManager.Observers.NotifyListUpdated();  //stage 5
                return;
            }
            throw new BO.BlCannotBeDeletedException($"Call with ID={callId} cannot be deleted");
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Call with ID={callId} does Not exist", ex);
        }
        catch (DO.DalXMLFileLoadCreateException ex)
        {
            throw new BO.BlXMLFileLoadCreateException("Xml Error", ex);
        }
    }

    /// <summary>
    /// Returns a collection filtered by that volunteer's ID - all closed calls by that volunteer
    /// </summary>
    /// <param name="id">ID card of a volunteer whose calls are being checked</param>
    /// <param name="filter">ENUM value of the call type by which the list will be filtered.</param>
    /// <param name="sort">ENUM value of a field in the ClosedCallList, by which the list is sorted</param>
    /// <returns></returns>
    public IEnumerable<BO.ClosedCallInList> GetClosedCalls(int volId, BO.CallType? filter = null, BO.CloseCallData? sort = null)
    {
        try
        {
            IEnumerable<DO.Assignment> assignmentForVol;
            IEnumerable<BO.ClosedCallInList> closeCalls;

            lock (AdminManager.BlMutex)
                assignmentForVol = _dal.Assignment.ReadAll(a => a.VolunteerId == volId).ToList();
            closeCalls = from assin in assignmentForVol
                         let tmpCall = Read(assin.CallId)
                         where assin.EndTreatment == DO.AssignmentEnum.TakenCare
                         let callAssin = tmpCall.CallAssignments.LastOrDefault()
                         select CallManager.ToCloseCall(tmpCall, callAssin); //can throw Ex


            closeCalls = null == filter ? closeCalls : closeCalls.Where(call => (BO.CallType)filter == call.CallType);
            closeCalls = null == sort ? closeCalls.OrderBy(c => c.Id)
                : closeCalls.OrderBy<BO.ClosedCallInList, object>(call => sort switch
                {
                    BO.CloseCallData.Id => call.Id,
                    BO.CloseCallData.CallType => call.CallType,
                    BO.CloseCallData.CallAddress => call.FullAddress,
                    BO.CloseCallData.InterTime => call.InterTime,
                    BO.CloseCallData.OpenTime => call.OpenTime,
                    BO.CloseCallData.CloseTime => call.CloseTime,
                    BO.CloseCallData.EndTreatment => call.EndTreatment,
                    _ => call.Id,
                });
            return closeCalls;

        }
        catch (DO.DalXMLFileLoadCreateException ex)
        {
            throw new BO.BlXMLFileLoadCreateException("Xml Error", ex);
        }
    }

    /// <summary>
    /// Returns a collection of all the open calls with the distance from the Volunteer who asked.
    /// </summary>
    /// <param name="volId">Volunteer to calculate the distance</param>
    /// <param name="filter">ENUM value of the call type by which the list will be filtered</param>
    /// <param name="sort">ENUM value of a field in the ClosedCallList, by which the list is sorted</param>
    /// <returns></returns>
    public IEnumerable<BO.OpenCallInList> GetOpenedCalls(int volId, BO.CallType? filter = null, BO.OpenCallData? sort = null)
    {
        return CallManager.GetOpenedCalls(volId, filter, sort);
        #region draft
        //lock (AdminManager.BlMutex)
        //{
        //    DO.Volunteer vol;
        //    IEnumerable<DO.Call> oldCalls;
        //    lock (AdminManager.BlMutex)
        //    {
        //        vol = _dal.Volunteer.Read(volId) ?? throw new BO.BlDoesNotExistException($"There is no volunteer with ID:{volId}");
        //        oldCalls = _dal.Call.ReadAll(null);
        //    }
        //    var openCalls = from item in oldCalls
        //                    let tmpCall = Read(item.Id)
        //                    where (tmpCall.Status == BO.CallStatus.OpenInRisk || tmpCall.Status == BO.CallStatus.Opened) && CallManager.CorrectDis(vol, item.Latitude, item.Longitude)
        //                    select CallManager.ToOpenCall(item, volId); //can throw Ex


        //    openCalls = null == filter ? openCalls : from call in openCalls
        //                                             where (BO.CallType)filter == call.CallType
        //                                             select call;

        //    openCalls = null == sort ? openCalls.OrderBy(c => c.Id)
        //        : openCalls.OrderBy<BO.OpenCallInList, object>(call => (sort switch
        //        {
        //            BO.OpenCallData.Id => call.Id,
        //            BO.OpenCallData.CallType => call.CallType,
        //            BO.OpenCallData.Description => call.Description,
        //            BO.OpenCallData.CallAddress => call.FullAddress,
        //            BO.OpenCallData.OpenTime => call.OpenTime,
        //            BO.OpenCallData.MaxCloseTime => call.MaxCloseTime,
        //            BO.OpenCallData.VolDistance => call.VolDistance,
        //            _ => call.Id
        //        }));

        //    return openCalls;
        //}
        #endregion
    }

    /// <summary>
    /// Returns an array of quantities according to the call status.
    /// </summary>
    /// <returns>array of quantities </returns>
    public int[] HowManyCalls()
    {

        IEnumerable<BO.CallInList> calls = ReadAll();
        var groupedCalls = from item in calls
                           group item by item.Status into g
                           select new { Status = (int)g.Key, Count = g.Count() };


        int[] statusCounts = new int[6];
        groupedCalls.ToList().ForEach(g => statusCounts[g.Status] = g.Count);

        #region foreach vers
        //foreach (var group in groupedCalls)
        //{
        //    statusCounts[group.Status] = group.Count;
        //}
        #endregion
        return statusCounts;
    }

    /// <summary>
    /// Requests the data layer (Read) to obtain details about the read and its list of assignments (if any)
    /// From the details received, creates an object of BO call.
    /// </summary>
    /// <param name=callId">Receives a call ID</param>
    /// <returns>BO Call</returns>
    /// <exception cref="BO.BlDoesNotExistException"></exception>
    public BO.Call Read(int callId)
    {
        return CallManager.Read(callId);
        #region draft
        //lock (AdminManager.BlMutex)
        //{
        //    var doCall = _dal.Call.Read(callId) ?? throw new BO.BlDoesNotExistException($"Call with ID={callId} does Not exist");

        //    Func<DO.Assignment, bool> func = item => item.CallId == callId;

        //    IEnumerable<DO.Assignment> dataAssignments = _dal.Assignment.ReadAll(func); //read all assignment of this Call.
        //    var callAssignments = new List<BO.CallAssignInList>();
        //    if (dataAssignments.Count() != 0)
        //    {
        //        callAssignments = dataAssignments.Select(assign => new BO.CallAssignInList
        //        {
        //            VolunteerId = assign.VolunteerId,
        //            VolunteerName = (assign.VolunteerId != 0) ? _dal.Volunteer.Read(assign.VolunteerId).FullName : null,
        //            InterTime = assign.InterTime,
        //            EndTime = assign.EndTime.HasValue ? assign.EndTime : null,
        //            EndTreatment = assign.EndTreatment.HasValue ? (BO.EndTreatment)assign.EndTreatment : null, /*BO.EndTreatment.None,*/

        //        }).ToList();
        //    }



        //    var myStatus = dataAssignments.Count() != 0 ? CallManager.MakeStatus(dataAssignments.Last(), doCall.MaxTime) : BO.CallStatus.Opened;//הNULL מנוהל
        //    return new()
        //    {
        //        Id = callId,
        //        CallType = (BO.CallType)doCall.CallType,
        //        Description = doCall.Description,
        //        CallAddress = doCall.CallAddress,
        //        Latitude = doCall.Latitude,
        //        Longitude = doCall.Longitude,
        //        OpenTime = doCall.OpenTime,
        //        MaxCloseTime = doCall.MaxTime,
        //        Status = dataAssignments.Count() != 0 ? CallManager.MakeStatus(dataAssignments.Last(), doCall.MaxTime) : BO.CallStatus.Opened, //הNULL מנוהל
        //        CallAssignments = callAssignments
        //    };
        //}
        #endregion
    }

    /// <summary>
    /// Returns a list of calls filtered and sorted according to the parameters received.
    /// </summary>
    /// <param name="filter">Parameter for filtering the list</param>
    /// <param name="sort">Parameter for sorting the list</param>
    /// <param name="value">Value for filtering the list</param>
    /// <returns></returns>
    public IEnumerable<BO.CallInList> ReadAll(BO.CallData? filter = null, BO.CallData? sort = null, object? value = null)
    {
        try
        {
            IEnumerable<DO.Call> oldCalls;
            IEnumerable<DO.Assignment> oldAssignment;

            lock (AdminManager.BlMutex)
            {
                oldCalls = _dal.Call.ReadAll(null); //can throw Ex
                oldAssignment = _dal.Assignment.ReadAll(null); //can throw Ex
            }
            IEnumerable<BO.CallInList> calls = CallManager.ToCallInList(oldCalls, oldAssignment);

            calls = null == filter ? calls
                                : calls.Where(call => filter switch
                                {
                                    BO.CallData.Id => (int)value == call.Id,
                                    BO.CallData.CallId => (int)value == call.CallId,
                                    BO.CallData.CallType => (BO.CallType)value == call.CallType,
                                    BO.CallData.OpenTime => (DateTime)value == call.OpenTime,
                                    BO.CallData.LeftTime => (TimeSpan)value == call.LeftTime,
                                    BO.CallData.LastVolunteer => value.ToString() == call.LastVolunteer,
                                    BO.CallData.CompletionTime => (TimeSpan)value == call.CompletionTime,
                                    BO.CallData.Status => (BO.CallListStatus)value == call.Status,
                                    BO.CallData.TotalAssignments => (int)value == call.TotalAssignments,
                                    _ => true
                                });


            calls = null == sort ? calls.OrderBy(c => c.CallId)
                : calls.OrderBy<BO.CallInList, object>(call => (sort switch
                {
                    BO.CallData.Id => call.Id,
                    BO.CallData.CallId => call.CallId,
                    BO.CallData.CallType => call.CallType,
                    BO.CallData.OpenTime => call.OpenTime,
                    BO.CallData.LeftTime => call.LeftTime,
                    BO.CallData.LastVolunteer => call.LastVolunteer,
                    BO.CallData.CompletionTime => call.CompletionTime,
                    BO.CallData.Status => call.Status,
                    BO.CallData.TotalAssignments => call.TotalAssignments,
                    _ => call.Id
                }));
            return calls;
        }
        catch (DO.DalXMLFileLoadCreateException ex)
        {
            throw new BO.BlXMLFileLoadCreateException("Xml Error", ex);
        }
    }

    /// <summary>
    /// Updating an existing call
    /// </summary>
    /// <param name="callToUpdate">Updated values ​​of the call</param>
    /// <exception cref="BO.BlDoesNotExistException">Throws an exception when the call you want to update does not exist in the database.</exception>
    public void Update(BO.Call callToUpdate)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        CallManager.CheckFormat(callToUpdate);
        DO.Call oldCall;
        lock (AdminManager.BlMutex)
            oldCall = _dal.Call.Read(callToUpdate.Id)!;

        DO.Call doCall = new(callToUpdate.Id, (DO.TypeOfCall)callToUpdate.CallType, callToUpdate.CallAddress, 0, 0, callToUpdate.OpenTime, callToUpdate.Description, callToUpdate.MaxCloseTime); //CallManager.CheckLogic(callToUpdate); //can throw Ex
        try
        {
            lock (AdminManager.BlMutex)
                _dal.Call.Update(doCall); //can throw Ex
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Call with ID={callToUpdate.Id} does Not exists", ex);
        }
        catch (DO.DalXMLFileLoadCreateException ex)
        {
            throw new BO.BlXMLFileLoadCreateException("Xml Error", ex);
        }
        CallManager.Observers.NotifyItemUpdated(callToUpdate.Id);  //stage 5
        CallManager.Observers.NotifyListUpdated();  //stage 5

        try
        {
            //compute the coordinates asynchronously without waiting for the results
            _ = CallManager.updateCoordinates(doCall); //stage 7
        }
        catch (BO.BlIntegrityOfValuesException ex)
        {
            lock (AdminManager.BlMutex)
                _dal.Call.Update(oldCall);
            CallManager.Observers.NotifyItemUpdated(doCall.Id);  //stage 5
            CallManager.Observers.NotifyListUpdated();  //stage 5
            throw new BO.BlIntegrityOfValuesException("Wrong Address. No coordinates found for the given address.");
        }

    }

    /// <summary>
    /// Unassigns a call in database
    /// </summary>
    /// <param name="volId">The volunteer who wants to update the call</param>
    /// <param name="assignmentId">The allocation number of the requested call</param>
    /// <exception cref="BO.BlDoesNotExistException">Throws an exception when the call you want to update does not exist in the database.</exception>
    /// <exception cref="BO.BlCantUpdateException">Throws an exception when the call you want to update is not updatable.</exception>
    public void UpdateCancelTreatment(int volId, int assignmentId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        CallManager.UpdateCancelTreatment(volId, assignmentId);
        #region draft
        //AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        //try
        //{
        //    DO.Assignment assignment;
        //    lock (AdminManager.BlMutex)
        //    {
        //        assignment = _dal.Assignment.Read(assignmentId) ?? throw new BO.BlDoesNotExistException($"Assignment with ID={assignmentId} does not exists"); //can throw Ex
        //        DO.Volunteer volunteer = _dal.Volunteer.Read(volId) ?? throw new BO.BlDoesNotExistException($"volunteer with ID={volId} does not exists");
        //        if ((volId == assignment.VolunteerId || volunteer.Job == DO.Role.Manager) && assignment.EndTime is null)
        //        {
        //            DO.Assignment updateAssignment;
        //            if (volId == assignment.VolunteerId)
        //            {
        //                updateAssignment = new(assignmentId, assignment.CallId, volId,
        //                    assignment.InterTime, AdminManager.Now, DO.AssignmentEnum.SelfCancel);
        //            }
        //            else
        //            {
        //                updateAssignment = new(assignmentId, assignment.CallId, volId,
        //                   assignment.InterTime, AdminManager.Now, DO.AssignmentEnum.CancelAdmin);
        //            }
        //            _dal.Assignment.Update(updateAssignment); //can throw Ex
        //        }
        //        else
        //            throw new BO.BlCantUpdateException($"Assignment with ID: {assignmentId} cannot be canceled");
        //    }

        //    CallManager.Observers.NotifyItemUpdated(assignment.CallId);  //stage 5
        //    CallManager.Observers.NotifyListUpdated();  //stage 5
        //    VolunteerManager.Observers.NotifyListUpdated(); //stage 5
        //}
        //catch (DO.DalDoesNotExistException ex)
        //{
        //    throw new BO.BlDoesNotExistException($"item with ID: {assignmentId} does not exists", ex);
        //}
        //catch (DO.DalXMLFileLoadCreateException ex)
        //{
        //    throw new BO.BlXMLFileLoadCreateException("Xml Error", ex);
        //}
        #endregion
    }

    /// <summary>
    /// Updates the assignment of a call to closed status
    /// </summary>
    /// <param name="volId">The volunteer who wants to update the call</param>
    /// <param name="assignmentId">The allocation number of the requested call</param>
    /// <exception cref="BO.BlDoesNotExistException">Throws an exception when the call you want to update does not exist in the database</exception>
    /// <exception cref="BO.BlCantUpdateException">Throws an exception when the call you want to update is not updatable</exception>
    public void UpdateEndTreatment(int volId, int assignmentId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        CallManager.UpdateEndTreatment(volId, assignmentId);
        #region draft
        //AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        //try
        //{
        //    DO.Assignment updateAssignment;
        //    lock (AdminManager.BlMutex)
        //    {
        //        DO.Assignment assignment = _dal.Assignment.Read(assignmentId) ?? throw new BO.BlDoesNotExistException($"Assignment with ID={assignmentId} does not exists"); //can throw Ex
        //        if (volId == assignment.VolunteerId && assignment.EndTreatment == null)
        //        {
        //            updateAssignment = new(assignmentId, assignment.CallId, volId,
        //            assignment.InterTime, AdminManager.Now, DO.AssignmentEnum.TakenCare);
        //            _dal.Assignment.Update(updateAssignment); //can throw Ex
        //        }
        //        else
        //            throw new BO.BlCantUpdateException($"Assignment with ID: {assignmentId} cannot be closed");
        //    }
        //    CallManager.Observers.NotifyItemUpdated(updateAssignment.Id);  //stage 5
        //    CallManager.Observers.NotifyListUpdated();  //stage 5
        //    VolunteerManager.Observers.NotifyListUpdated(); //stage 5
        //}
        //catch (DO.DalDoesNotExistException ex)
        //{
        //    throw new BO.BlDoesNotExistException($"Assignment with ID={assignmentId} does not exists", ex);
        //}
        //catch (DO.DalXMLFileLoadCreateException ex)
        //{
        //    throw new BO.BlXMLFileLoadCreateException("Xml Error", ex);
        //}
        #endregion
    }

    public void GetAssignmentToEnd(int volId, int callId)
    {
        CallManager.GetAssignmentToEnd(volId, callId);
        #region draft
        //int assignId;
        //lock (AdminManager.BlMutex)
        //{ 
        //    DO.Assignment assignment = _dal.Assignment.ReadAll().Where(a => a.CallId == callId).LastOrDefault() ?? throw new BO.BlDoesNotExistException($"Call with ID={callId} does not have a assignment");
        //    assignId = assignment.Id;
        //}
        //UpdateEndTreatment(volId, assignId);
        #endregion
    }

    public void GetAssignmentToCancel(int volId, int callId)
    {
        CallManager.GetAssignmentToCancel(volId, callId);
        #region draft
        //int assignId;
        //lock (AdminManager.BlMutex)
        //{
        //    DO.Assignment? assignment = _dal.Assignment.ReadAll().Where(a => a.CallId == callId).LastOrDefault() ?? throw new BO.BlDoesNotExistException($"Call with ID={callId} does not have a assignment");
        //    assignId = assignment.Id;
        //}
        //UpdateCancelTreatment(volId, assignId);
        #endregion
    }

    #region Stage 5
    public void AddObserver(Action listObserver) =>
    CallManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
    CallManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
    CallManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
    CallManager.Observers.RemoveObserver(id, observer); //stage 5
    #endregion Stage 5
}