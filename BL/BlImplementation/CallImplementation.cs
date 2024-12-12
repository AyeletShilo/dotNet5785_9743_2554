using BlApi;
using BO;
using DalApi;
using Helpers;
using System.ComponentModel.DataAnnotations;
//using BO;

namespace BlImplementation;

internal class CallImplementation : BlApi.ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void CallToTreatment(int id, int assignmentId)
    {
        throw new NotImplementedException();
    }

    public void Create(BO.Call callToAdd)
    {
        CallManager.CheckLogic(callToAdd);
        CallManager.CheckFormat(callToAdd);

        DO.Call doCall = new(callToAdd.Id, (DO.TypeOfCall)callToAdd.CallType, callToAdd.CallAddress, callToAdd.Latitude,
            callToAdd.Longitude, callToAdd.OpenTime, callToAdd.Description, callToAdd.MaxCloseTime);

        _dal.Call.Create(doCall);
    }

    public void Delete(int id) //לבדוק את החריגות
    {
        try
        {
            //BO.Call toDelete = Read(id); // אם לא קיים כזה ID לאן תזרק החריגה?
            if (Read(id).Status == CallStatus.Opened)
            {
                _dal.Call.Delete(id);
            }
            throw new BO.CannotBeDeletedException(); //האם אפשר להשתמש באותו שם של מתנדב?
        }
        catch (DO.DalDoesNotExistException ex) //חריגה ממתודת מחיקה של DO
        {
            throw new BO.BlDoesNotExisException($"Call with ID={id} does Not exist", ex);
        }
        catch (BO.BLCannotBeDeletedException ex) //חריגה אם אי אפשר למחוק את סוג הסטטוס הזה
        {
            throw new BO.BLCannotBeDeletedException($"Call with ID={id} cannot be deleted", ex);
        }

    }

    public IEnumerable<BO.ClosedCallInList> GetClosedCalls(int id, BO.CallType? filter = null, BO.CloseCallData? sort = null)
    {
        
      IEnumerable<DO.Call> OldCalls = _dal.Call.ReadAll(null);
      List<BO.ClosedCallInList>? Calls = new List<BO.ClosedCallInList>();
        foreach (DO.Call item in OldCalls)
        {
            if (Read(item.Id).Status == CallStatus.Closed)
            {
                BO.CallAssignInList CallAssignment =Read(item.Id).CallAssignments.Last();
                Calls.Add(CallManager.ToCloseCall(item, CallAssignment));
            }
        }
                    
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
            if (Read(item.Id).Status == CallStatus.OpenInRisk || Read(item.Id).Status == CallStatus.Opened)
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
        IEnumerable<DO.Call> calls = _dal.Call.ReadAll() ?? throw new NoCallstException("The requested call does not exist.");
           
        
        .GroupBy(ticket => ticket.Status)
            .ToDictionary(group => group.Key, group => group.Count());
    }

    public BO.Call Read(int id) //add try and catch
    {
        Func<DO.Assignment, bool> func = item => item.CallId == id;
        IEnumerable<DO.Assignment> dataAssignments = _dal.Assignment.ReadAll(func);

        var doCall = _dal.Call.Read(id) ?? throw new NotExistException("The requested call does not exist.");

        return new()
        {
            Id = id,
            CallType = doCall.CallType,
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

            CallManager.CheckLogic(callToUpdate);
            CallManager.CheckFormat(callToUpdate);

        DO.Call doCall = new(callToUpdate.Id, (DO.TypeOfCall)callToUpdate.CallType, callToUpdate.CallAddress, callToUpdate.Latitude,
            callToUpdate.Longitude, callToUpdate.OpenTime, callToUpdate.Description, callToUpdate.MaxCloseTime);
            try
            {
                _dal.Call.Update(doCall);
            }
            catch (DO.DalDoesNotExistException ex)
            {
                throw new BO.BlDoesNotExistException($"Call with ID={callToUpdate.Id} does Not exists");
            }
    }

    public void UpdateCancelTreatment(int id, int assignmentId)
    {
        throw new NotImplementedException();
    }

    public void UpdateEndTreatment(int id, int assignmentId)
    {
        throw new NotImplementedException();
    }
}
