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

    public void Create(BO.Call CallToAdd)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
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
        var result= _dal.Call.
    }

    public BO.Call Read(int id)
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

    public void Update(BO.Call CallToUpdate)
    {
        throw new NotImplementedException();
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
