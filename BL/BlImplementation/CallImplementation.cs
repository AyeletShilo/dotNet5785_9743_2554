using BlApi;
using Helpers;


namespace BlImplementation;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void CallToTreatment(int id, int callId)
    {
        BO.Call call = Read(callId) ?? throw new BO.BlDoesNotExistException($"Call with ID={callId} does not exists");
        Func<DO.Assignment, bool>? predicate = assignment => (assignment.CallId == callId && assignment.EndTreatment != DO.AssignmentEnum.SelfCancel
                                                             && assignment.EndTreatment != DO.AssignmentEnum.CancelAdmin);
        var assignments = _dal.Assignment.ReadAll(predicate).ToList();
        if (assignments.Count() != 0)
            throw new BO.BlDoesAlreadyExistException($"Assignment for call with ID={callId} already exists");
        if (call.Status != BO.CallStatus.InTreatment && call.Status != BO.CallStatus.Expired && call.Status != BO.CallStatus.Closed)
            _dal.Assignment.Create(new(0, callId, id, ClockManager.Now, null, null)); //איך אני מביאה את המספר מזהה רץ? תשובה: זה מחשב לבד ביצירה של הקצאה, לכן אפשר לשים סתם int.
        else
            throw new BO.BlCantHandleItException($"Unable to assign"); //מה הולך פה עם החריגות? מה זה החריגה הזאת?
    }

    /// <summary>
    /// Gets BO.Call object and asks the dl to add it to the call data.
    /// </summary>
    /// <param name="callToAdd"></param>
    public void Create(BO.Call callToAdd)
    {
        DO.Call doCall = CallManager.CheckLogic(callToAdd); //זורק חריגה
        _dal.Call.Create(doCall);
    }

    /// <summary>
    /// Ask the dl to check if the call can be deleted.
    /// </summary>
    /// <param name="id"> id of the call that needs to be deleted</param>
    /// <exception cref="BO.CannotBeDeletedException"></exception>
    /// <exception cref="BO.BlDoesNotExisException"></exception>
    /// <exception cref="BO.BLCannotBeDeletedException"></exception>
    public void Delete(int callId) //לבדוק את החריגות
    {
        try
        {
            BO.Call toDelete = Read(callId) ?? throw new BO.BlDoesNotExistException($"Call with ID={callId} does Not exist"); // אם לא קיים כזה ID לאן תזרק החריגה?
            if ((toDelete.Status == BO.CallStatus.Opened || toDelete.Status==BO.CallStatus.OpenInRisk) && toDelete.CallAssignments is null)
            {
                _dal.Call.Delete(callId);
                return;
            }
            throw new BO.BlCannotBeDeletedException($"Call with ID={callId} cannot be deleted");
        }
        catch (DO.DalDoesNotExistException ex) //חריגה ממתודת מחיקה של DO
        {
            throw new BO.BlDoesNotExistException($"Call with ID={callId} does Not exist", ex);
        }
    }

    /// <summary>
    /// Returns a collection filtered by that volunteer's ID - all closed calls by that volunteer
    /// </summary>
    /// <param name="id">ID card of a volunteer whose calls are being checked</param>
    /// <param name="filter">ENUM value of the call type by which the list will be filtered.</param>
    /// <param name="sort">ENUM value of a field in the ClosedCallList, by which the list is sorted</param>
    /// <returns></returns>
    public IEnumerable<BO.ClosedCallInList> GetClosedCalls(int volId, BO.CallType? filter = null, BO.CloseCallData? sort = null) ///צריך לבדוק ולדאוג שמוציאים רק את הקריאות שמתאימות לתז של המתנדב
    {
        #region draft
        //List<BO.ClosedCallInList>? calls = new List<BO.ClosedCallInList>();

        //foreach (DO.Assignment assignment in assignmentForVol)
        //{
        //    var re = Read(assignment.CallId);
        //    if (re.Status == BO.CallStatus.Closed)
        //    {
        //        BO.CallAssignInList callAssignment = re.CallAssignments.Last();
        //        calls.Add(CallManager.ToCloseCall(_dal.Call.Read(c => c.Id == assignment.CallId), callAssignment));

        //    }
        //}

        //IEnumerable<DO.Call> oldCalls =/* _dal.Call.*/ReadAll().Where(c=>c.Id==);
        //List<BO.ClosedCallInList>? calls = new List<BO.ClosedCallInList>();
        //foreach (DO.Call item in oldCalls)
        //{
        //    var statusToCheck = Read(item.Id).Status;
        //    Console.WriteLine(statusToCheck); ///addition
        //    if (statusToCheck/*Read(item.Id).Status*/ == BO.CallStatus.Closed)
        //    {
        //        BO.CallAssignInList CallAssignment = Read(item.Id).CallAssignments.Last();
        //        calls.Add(CallManager.ToCloseCall(item, CallAssignment));
        //    }
        //}
        //calls.AddRange(from item in OldCalls
        //               let callDetails = Read(item.Id)
        //               where callDetails.Status == BO.CallStatus.Closed && callDetails.CallAssignments?.Any() == true
        //               let lastAssignment = callDetails.CallAssignments.Last()
        //               select CallManager.ToCloseCall(item, lastAssignment)
        //               );

        //IEnumerable<BO.ClosedCallInList>? closeCall = calls.Select(c => c);
        #endregion
        IEnumerable<DO.Assignment> assignmentForVol = _dal.Assignment.ReadAll(a => a.VolunteerId == volId);
        IEnumerable<BO.ClosedCallInList> closeCalls = from assin in assignmentForVol
                                                     let tmpCall = Read(assin.CallId)
                                                     where tmpCall.Status == BO.CallStatus.Closed
                                                     let callAssin = tmpCall.CallAssignments.LastOrDefault()
                                                     select CallManager.ToCloseCall(tmpCall, callAssin);
        if (filter != null)
        {
            string filterProperty = CallManager.GetPropertyName(filter.Value);
            closeCalls = closeCalls.Where(c => c.GetType().GetProperty(filterProperty)?.GetValue(c)?.Equals(filter) ?? false);
        }
        if (sort != null)
        {
            string sortProperty = sort.Value.ToString();
            //string sortProperty = CallManager.GetPropertyName(sort.Value);
            closeCalls = closeCalls.OrderBy(c => c.GetType().GetProperty(sortProperty)?.GetValue(c));
            return closeCalls;
        }
        else
            return closeCalls.OrderBy(c => c.Id);
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
        #region draft
        //List<BO.OpenCallInList>? calls = new List<BO.OpenCallInList>(); //להחליף את הפור איצ?
        //foreach (DO.Call item in oldCalls) //להחליף פור איצ
        //{
        //    if (Read(item.Id).Status == BO.CallStatus.OpenInRisk || Read(item.Id).Status == BO.CallStatus.Opened)
        //    {
        //        BO.CallAssignInList CallAssignment = Read(item.Id).CallAssignments.LastOrDefault();
        //        if (CallAssignment != null)
        //           calls.Add(CallManager.ToOpenCall(item, CallAssignment));
        //    }
        //}


        //IEnumerable<BO.OpenCallInList> ? openCalls = calls.Select(c => c);
        #endregion

        IEnumerable<DO.Call> oldCalls = _dal.Call.ReadAll(null);
        var openCalls = from item in oldCalls
                        let tmpCall = Read(item.Id)
                        where tmpCall.Status == BO.CallStatus.OpenInRisk || tmpCall.Status == BO.CallStatus.Opened
                        select CallManager.ToOpenCall(item, volId);

        if (filter != null)
        {
            string filterProperty = CallManager.GetPropertyName(filter.Value);
            openCalls = openCalls.Where(c => c.GetType().GetProperty(filterProperty)?.GetValue(c)?.Equals(filter) ?? false);
        }
        if (sort != null)
        {
            string sortProperty = CallManager.GetPropertyName(sort.Value);
            openCalls = openCalls.OrderBy(c => c.GetType().GetProperty(sortProperty)?.GetValue(c));
            return openCalls;
        }
        else
            return openCalls.OrderBy(c => c.Id);
    }

    #region HowManyCalls
    public int[] HowManyCalls()
    {
        IEnumerable<BO.CallInList> calls = ReadAll(); //?? throw new BO.BlDoesNotExistException("The requested call does not exist."); לא צריך את זה
        int[] counts = (from item in calls
                        group item by item.Status into groupedCalls
                        orderby groupedCalls.Key
                        select groupedCalls.Count()).ToArray();
        return counts;
    }
    #endregion

    /// <summary>
    /// Requests the data layer (Read) to obtain details about the read and its list of assignments (if any)
    /// From the details received, creates an object of BO call.
    /// </summary>
    /// <param name=callId">Receives a call ID</param>
    /// <returns>BO Call</returns>
    /// <exception cref="BO.BlDoesNotExistException"></exception>
    public BO.Call Read(int callId)
    {
        var doCall = _dal.Call.Read(callId) ?? throw new BO.BlDoesNotExistException($"Call with ID={callId} does Not exist");

        Func<DO.Assignment, bool> func = item => item.CallId == callId;
        IEnumerable<DO.Assignment> dataAssignments = _dal.Assignment.ReadAll(func); //read all assignment of this Call.
        var callAssignments = new List<BO.CallAssignInList>();
        if (dataAssignments != null)
        {
            callAssignments = dataAssignments.Select(assign => new BO.CallAssignInList
            {
                VolunteerId = assign.VolunteerId,
                VolunteerName = _dal.Volunteer.Read(assign.VolunteerId).FullName ?? throw new BO.BlNullPropertyException("volunteer?"),
                InterTime = assign.InterTime,
                EndTime = assign.EndTime.HasValue ? assign.EndTime : null,
                EndTreatment = /*(BO.EndTreatment)*/assign.EndTreatment.HasValue ? (BO.EndTreatment)assign.EndTreatment : BO.EndTreatment.None,

            }).ToList();
        }

        //foreach (var assign in callAssignments)
        //{ Console.WriteLine(assign.VolunteerId + " " + assign.VolunteerName + " " + assign.EndTime + " " + assign.EndTreatment); }

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
            Status = CallManager.MakeStatus(dataAssignments.Last(), doCall.MaxTime), //הNULL מנוהל
            CallAssignments = callAssignments

        };
    }

    public IEnumerable<BO.CallInList> ReadAll(BO.CallData? filter = null, BO.CallData? sort = null, object? value = null)
    {
        IEnumerable<DO.Call> oldCalls = _dal.Call.ReadAll(null);
        IEnumerable<DO.Assignment> oldAssignment = _dal.Assignment.ReadAll(null);
        IEnumerable<BO.CallInList> calls = CallManager.ToCallInList(oldCalls, oldAssignment);

        if (filter != null)
        {
            string filterProperty = CallManager.GetPropertyName(filter.Value);
            calls = calls.Where(c => c.GetType().GetProperty(filterProperty)?.GetValue(c)?.Equals(value) ?? false);
        }
        if (sort != null)
        {
            string sortProperty = CallManager.GetPropertyName(sort.Value);
            calls = calls.OrderBy(c => c.GetType().GetProperty(sortProperty)?.GetValue(c));
            return calls;
        }
        else
            return calls.OrderBy(c => c.CallId);

    }

    public void Update(BO.Call callToUpdate)
    {
        try
        {
            DO.Call doCall = CallManager.CheckLogic(callToUpdate); //לסדר חריגות
            _dal.Call.Update(doCall); // חריגה מהמחיקה שבעדכון שבDO
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Call with ID={callToUpdate.Id} does Not exists", ex);
        }
    }

    public void UpdateCancelTreatment(int volId, int assignmentId)
    {
        try
        {
            DO.Assignment assignment = _dal.Assignment.Read(assignmentId) ?? throw new BO.BlDoesNotExistException($"Assignment with ID={assignmentId} does not exists");
            DO.Volunteer volunteer = _dal.Volunteer.Read(volId) ?? throw new BO.BlDoesNotExistException($"volunteer with ID={volId} does not exists");
            if ((volId == assignment.VolunteerId || volunteer.Job == DO.Role.Manager) && assignment.EndTime is null)
            {
                DO.Assignment UpdateAssignment;
                if (volId == assignment.VolunteerId)
                {
                    UpdateAssignment = new(assignmentId, assignment.CallId, volId,
                        assignment.InterTime, ClockManager.Now, DO.AssignmentEnum.SelfCancel);
                }
                else
                {
                    UpdateAssignment = new(assignmentId, assignment.CallId, volId,
                       assignment.InterTime, ClockManager.Now, DO.AssignmentEnum.CancelAdmin);
                }
                _dal.Assignment.Update(UpdateAssignment); //חריגה מהמתודה של DO
            }
            else
                throw new BO.BlCantUpdateException($"Assignment with ID: {assignmentId} cannot be canceled"); // תזרק לשכבה הבאה
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Assignment /*or Volunteer*/ with ID: {assignmentId} does not exists", ex); //מה לכתוב בחלק הראשון??
        }
    }

    public void UpdateEndTreatment(int volId, int assignmentId)
    {
        try
        {
            DO.Assignment assignment = _dal.Assignment.Read(assignmentId) ?? throw new BO.BlDoesNotExistException($"Assignment with ID={assignmentId} does not exists");
            if (volId == assignment.VolunteerId && assignment.EndTreatment is null) //? עוד בדיקות?
            {
                DO.Assignment UpdateAssignment = new(assignmentId, assignment.CallId, volId,
                    assignment.InterTime, ClockManager.Now, DO.AssignmentEnum.TakenCare);
                _dal.Assignment.Update(UpdateAssignment); //חריגה מDO
            }
            else
                throw new BO.BlCantUpdateException($"Assignment with ID: {assignmentId} cannot be closed"); //תזרק לשכבה הבאה
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Assignment with ID={assignmentId} does not exists", ex);
        }
    }
}