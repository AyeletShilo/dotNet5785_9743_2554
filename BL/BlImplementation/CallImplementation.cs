using BlApi;
using BO;
using DalApi;
using Helpers;
using System.ComponentModel.DataAnnotations;
//using BO;

namespace BlImplementation;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void CallToTreatment(int id, int assignmentId)
    {
        throw new NotImplementedException();
    }

    public void Create(BO.Call callToAdd)
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    public IEnumerable<BO.OpenCallInList> GetOpenedCalls(int id, BO.CallType? filter = null, BO.OpenCallData? sort = null)
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    public void Update(BO.Call callToUpdate)
    {
        BO.Call isManager = Read(callToUpdate.Id);
        try
        {
            if (id == callToUpdate.Id || isManager.Job == BO.Role.Manager)
            {

                VolunteerManager.CheckLogic(volToUpdate);
                VolunteerManager.CheckFormat(volToUpdate);

                var oldVolunteer = _dal.Volunteer.Read(volToUpdate.Id);
                if ((oldVolunteer.Job != (DO.Role)volToUpdate.Job) && isManager.Job != BO.Role.Manager)
                    throw new BO.CantUpdateException("Danater cant update this");

                DO.Volunteer doVolunteer = new(volToUpdate.Id, volToUpdate.FullName, volToUpdate.PhoneNumber, volToUpdate.Email, (DO.Role)(volToUpdate.Job), volToUpdate.IsActive, (DO.RangeType)volToUpdate.Distance,
             volToUpdate.Address, volToUpdate.Latitude, volToUpdate.Longitude, volToUpdate.MaxDis);
                try
                {
                    _dal.Volunteer.Update(doVolunteer);
                }
                catch (DO.BlDoesNotExistException ex)
                {
                    throw new BO.BlDoesNotExistException($"Volunteer with ID={volToUpdate.Id} does Not exists", ex);
                }
            }
            else
                throw new BO.CantUpdateException("Danater cant update this");
        }
        catch (BO.CantUpdateException ex)
        {
            throw new BO.CantUpdateException("Danater cant update this", ex);
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
