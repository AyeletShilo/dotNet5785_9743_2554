using BlApi;
using Helpers;


namespace BlImplementation;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void CallToTreatment(int id, int callId)
    {
        try
        {

            BO.Call call = Read(callId) ?? throw new BO.BlDoesNotExistException($"Call with ID={callId} does not exists");
            Func<DO.Assignment, bool>? predicate = assignment => (assignment.CallId == callId && assignment.EndTreatment != DO.AssignmentEnum.SelfCancel && assignment.EndTreatment != DO.AssignmentEnum.CancelAdmin);
            var assignments = _dal.Assignment.ReadAll(predicate);
            if (assignments is not null)
                throw new BO.BlDoesAlreadyExistException($"Assignment for call with ID={callId} already exists");
            if (call.Status != BO.CallStatus.InTreatment && call.Status != BO.CallStatus.Expired && call.Status != BO.CallStatus.Closed)
                _dal.Assignment.Create(new(0, callId, id, ClockManager.Now, null, null)); //איך אני מביאה את המספר מזהה רץ? תשובה: זה מחשב לבד ביצירה של הקצאה, לכן אפשר לשים סתם int.
            else
                throw new BO.BlDoesAlreadyExistException($"Assignment for call with ID={callId} already exists"); //מה הולך פה עם החריגות? מה זה החריגה הזאת?
        }
        //catch (DO.DalDoesNotExistException ex)
        //{
        //    throw new BO.BlDoesNotExistException($"Call with ID={callId} does not exists", ex);
        //}
        catch (BO.BlDoesAlreadyExistException ex)
        {
            throw ex;
        }

    }

    /// <summary>
    /// Gets BO.Call object and asks the dl to add it to the call data.
    /// </summary>
    /// <param name="callToAdd"></param>
    public void Create(BO.Call callToAdd)
    {
        try
        {
            CallManager.CheckLogic(callToAdd); ///חריגה נזרקת
            //CallManager.CheckFormat(callToAdd);

            DO.Call doCall = new(callToAdd.Id, (DO.TypeOfCall)callToAdd.CallType, callToAdd.CallAddress, (double)callToAdd.Latitude,
                (double)callToAdd.Longitude, callToAdd.OpenTime, callToAdd.Description, callToAdd.MaxCloseTime);

            _dal.Call.Create(doCall);
        }
        catch (BO.BlIntegrityOfValuesException ex) //שאלה אם צריך לתפוס פה את החריגה
        {
            throw ex;
        }
    }

    /// <summary>
    /// Ask the dl to check if the call can be deleted.
    /// </summary>
    /// <param name="id"> id of the call that needs to be deleted</param>
    /// <exception cref="BO.CannotBeDeletedException"></exception>
    /// <exception cref="BO.BlDoesNotExisException"></exception>
    /// <exception cref="BO.BLCannotBeDeletedException"></exception>
    public void Delete(int id) //לבדוק את החריגות
    {
        try
        {
            //BO.Call toDelete = Read(id); // אם לא קיים כזה ID לאן תזרק החריגה?
            if (Read(id).Status == BO.CallStatus.Opened)
            {
                _dal.Call.Delete(id);
                return;
            }
            throw new BO.BlCannotBeDeletedException($"Call with ID={id} cannot be deleted");
        }
        catch (DO.DalDoesNotExistException ex) //חריגה ממתודת מחיקה של DO
        {
            throw new BO.BlDoesNotExistException($"Call with ID={id} does Not exist", ex);
        }
        //catch (BO.BlCannotBeDeletedException ex) //חריגה אם אי אפשר למחוק את סוג הסטטוס הזה- תתפס בשכבה הבאה
        //{
        //    throw ex;
        //}

    }

    /// <summary>
    /// Returns a list filtered by that volunteer's ID - all closed calls by that volunteer
    /// </summary>
    /// <param name="id">ID card of a volunteer whose calls are being checked</param>
    /// <param name="filter">ENUM value of the call type by which the list will be filtered.</param>
    /// <param name="sort">ENUM value of a field in the ClosedCallList, by which the list is sorted</param>
    /// <returns></returns>
    public IEnumerable<BO.ClosedCallInList> GetClosedCalls(int id, BO.CallType? filter = null, BO.CloseCallData? sort = null)
    {

        IEnumerable<DO.Call> OldCalls = _dal.Call.ReadAll(null);
        List<BO.ClosedCallInList>? Calls = new List<BO.ClosedCallInList>();
        //foreach (DO.Call item in OldCalls)
        //{
        //    if (Read(item.Id).Status == BO.CallStatus.Closed)
        //    {
        //        BO.CallAssignInList CallAssignment = Read(item.Id).CallAssignments.Last();
        //        Calls.Add(CallManager.ToCloseCall(item, CallAssignment));
        //    }
        //}
        Calls.AddRange(from item in OldCalls
                       let callDetails = Read(item.Id)
                       where callDetails.Status == BO.CallStatus.Closed && callDetails.CallAssignments?.Any() == true
                       let lastAssignment = callDetails.CallAssignments.Last()
                       select CallManager.ToCloseCall(item, lastAssignment)
                       );

        IEnumerable<BO.ClosedCallInList>? closeCall = Calls.Where(call => call.Id == id);
        if (filter != null)
        {
            string filterProperty = CallManager.GetPropertyName(filter.Value);
            closeCall = closeCall.Where(c => c.GetType().GetProperty(filterProperty)?.GetValue(c)?.Equals(filter) ?? false);
        }
        if (sort == null)
            return closeCall.OrderBy(c => c.Id);
        else
        {
            string sortProperty = CallManager.GetPropertyName(sort.Value);
            closeCall = closeCall.OrderBy(c => c.GetType().GetProperty(sortProperty)?.GetValue(c));
            return closeCall;
        }
    }

    public IEnumerable<BO.OpenCallInList> GetOpenedCalls(int id, BO.CallType? filter = null, BO.OpenCallData? sort = null)
    {
        IEnumerable<DO.Call> OldCalls = _dal.Call.ReadAll(null);
        List<BO.OpenCallInList>? Calls = new List<BO.OpenCallInList>();
        foreach (DO.Call item in OldCalls)
        {
            if (Read(item.Id).Status == BO.CallStatus.OpenInRisk || Read(item.Id).Status == BO.CallStatus.Opened)
            {
                BO.CallAssignInList CallAssignment = Read(item.Id).CallAssignments.Last();
                Calls.Add(CallManager.ToOpenCall(item, CallAssignment));
            }
        }

        IEnumerable<BO.OpenCallInList>? OpenCalls = Calls;
        if (filter != null)
        {
            string filterProperty = CallManager.GetPropertyName(filter.Value);
            OpenCalls = Calls.Where(c => c.GetType().GetProperty(filterProperty)?.GetValue(c)?.Equals(filter) ?? false);
        }
        if (sort == null)
            return OpenCalls.OrderBy(c => c.Id);
        else
        {
            string sortProperty = CallManager.GetPropertyName(sort.Value);
            OpenCalls = OpenCalls.OrderBy(c => c.GetType().GetProperty(sortProperty)?.GetValue(c));
            return OpenCalls;
        }
    }

    public BO.CallStatus[] HowManyCalls()
    {
        throw new NotImplementedException();
    }

    #region HowManyCalls //TODO
    //public BO.CallStatus[] HowManyCalls()
    //{
    //    IEnumerable<DO.Call> calls = _dal.Call.ReadAll() ?? throw new NoCallstException("The requested call does not exist.");

    //    var v = from item in calls
    //            group item by CallManager.GetPropertyName(item.Status) into groupedCalls
    //            select


    //            .ToDictionary(group => group.Key, group => group.Count());
    //}
    #endregion

    public BO.Call Read(int id) //add try and catch
    {
        Func<DO.Assignment, bool> func = item => item.CallId == id;
        IEnumerable<DO.Assignment> dataAssignments = _dal.Assignment.ReadAll(func);

        var doCall = _dal.Call.Read(id) ?? throw new BO.BlDoesNotExistException($"Call with ID={id} does Not exist");

        return new()
        {
            Id = id,
            CallType = (BO.CallType)doCall.CallType,
            Description = doCall.Description,
            CallAddress = doCall.CallAddress,
            Latitude = doCall.Latitude,
            Longitude = doCall.Longitude,
            OpenTime = doCall.OpenTime,
            MaxCloseTime = doCall.MaxTime,
            Status = CallManager.MakeStatus(dataAssignments, doCall.MaxTime),

            CallAssignments = dataAssignments.Select(assign => new BO.CallAssignInList
            {
                VolunteerId = assign.VolunteerId,
                VolunteerName = _dal.Volunteer.Read(id).FullName,
                InterTime = assign.InterTime,
                EndTime = assign.EndTime,
                EndTreatment = (BO.EndTreatment)assign.EndTreatment,

            }).ToList()

        };

        throw new NotImplementedException();
    }

    public IEnumerable<BO.CallInList> ReadAll(BO.CallData? filter = null, BO.CallData? sort = null, object? value = null)
    {
        IEnumerable<DO.Call> Oldcalls = _dal.Call.ReadAll(null);
        IEnumerable<DO.Assignment> OldAssignment = _dal.Assignment.ReadAll(null);
        IEnumerable<BO.CallInList> calls = CallManager.ToCallInList(Oldcalls, OldAssignment);

        if (filter != null)
        {
            string filterProperty = CallManager.GetPropertyName(filter.Value);
            calls = calls.Where(c => c.GetType().GetProperty(filterProperty)?.GetValue(c)?.Equals(value) ?? false);
        }
        if (sort == null)
            return calls.OrderBy(c => c.CallId);
        else
        {
            string sortProperty = CallManager.GetPropertyName(sort.Value);
            calls = calls.OrderBy(c => c.GetType().GetProperty(sortProperty)?.GetValue(c));
            return calls;
        }

    }

    public void Update(BO.Call callToUpdate)
    {

        CallManager.CheckLogic(callToUpdate); //תזרק חריגה
        //CallManager.CheckFormat(callToUpdate);

        DO.Call doCall = new(callToUpdate.Id, (DO.TypeOfCall)callToUpdate.CallType, callToUpdate.CallAddress, (double)callToUpdate.Latitude,
            (double)callToUpdate.Longitude, callToUpdate.OpenTime, callToUpdate.Description, callToUpdate.MaxCloseTime);
        try
        {
            _dal.Call.Update(doCall); // חריגה מהמחיקה שבעדכון שבDO
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Call with ID={callToUpdate.Id} does Not exists");
        }
    }

    public void UpdateCancelTreatment(int id, int assignmentId)
    {

        try
        {
            DO.Assignment assignment = _dal.Assignment.Read(assignmentId) ?? throw new BO.BlDoesNotExistException($"Assignment with ID={assignmentId} does not exists");
            DO.Volunteer volunteer = _dal.Volunteer.Read(id) ?? throw new BO.BlDoesNotExistException($"volunteer with ID={id} does not exists");
            if ((id == assignment.VolunteerId || volunteer.Job == DO.Role.Manager) && assignment.EndTime is null)
            {
                DO.Assignment UpdateAssignment;
                if (id == assignment.VolunteerId)
                {
                    UpdateAssignment = new(assignmentId, assignment.CallId, id,
                        assignment.InterTime, ClockManager.Now, DO.AssignmentEnum.SelfCancel);
                }
                else
                {
                    UpdateAssignment = new(assignmentId, assignment.CallId, id,
                       assignment.InterTime, ClockManager.Now, DO.AssignmentEnum.CancelAdmin);
                }
                _dal.Assignment.Update(UpdateAssignment); //חריגה מהמתודה של DO
            }
            else
                throw new BO.BlCantUpdateException($"Assignment with ID: {assignmentId} cannot be canceled"); // תזרק לשכבה הבאה
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Assignment or Volunteer with ID: {assignmentId} does not exists", ex); //מה לכתוב בחלק הראשון??
        }
        //catch (BO.BlCantUpdateException ex2)
        //{
        //    throw ex2;
        //}

    }

    public void UpdateEndTreatment(int id, int assignmentId)
    {
        try
        {
            DO.Assignment assignment = _dal.Assignment.Read(assignmentId) ?? throw new BO.BlDoesNotExistException($"Assignment with ID={assignmentId} does not exists");
            if (id == assignment.VolunteerId && assignment.EndTreatment is null) //? עוד בדיקות?
            {
                DO.Assignment UpdateAssignment = new(assignmentId, assignment.CallId, id,
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
        //catch (BO.CantUpdateException ex2)
        //{
        //    throw ex2;
        //}
    }

}
