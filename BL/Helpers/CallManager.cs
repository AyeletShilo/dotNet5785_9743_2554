
using BO;
using DalApi;
using DO;

namespace Helpers;

internal static class CallManager
{
    private static IDal s_dal = Factory.Get; //stage 4

    internal static CallStatus MakeStatus(int id, IEnumerable<DO.Assignment> dataAssignments, DateTime MaxCloseTime)
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

    internal static void checkFormat(BO.Call callToCheck)
    {
        bool IdIisNum = int.TryParse(callToCheck.Id, out int Id);
    }

    internal static void CheckLogic(BO.Call toCheck) //where T: BO.Volunteer, BO.Call
    {
        bool isId = checkId(toCheck.Id);

        bool isAddress = checkAddress(toCheck.CallAddress);

        bool correntMaxTime = toCheck.MaxCloseTime > ClockManager.Now && toCheck.MaxCloseTime > toCheck.OpenTime;

        if (isId == false || isAddress == false || correntMaxTime == false)
            throw new BO.IntegrityOfValuesException("Error in value integrity");
    }

    private static bool checkId(int id)
    {
        int sum = 0;
        string idString = id.ToString();
        for (int i = 0; i < 8; i++)
        {
            int digit = idString[i] - '0'; // המרת התו למספר
            int multiplier = (i % 2 == 0) ? 1 : 2; // זוגי/אי-זוגי
            int product = digit * multiplier;
            sum += (product > 9) ? product - 9 : product; // סכום הספרות
        }
        if (idString[8] != (10 - (sum % 10)) % 10)
            return false;
        return true;
    }
    private static bool checkAddress(string address)
    {
        if (address == null) return true;

        //??
        //עדכון קווי אורך רוחב
        return true;
    }


    internal static string GetPropertyName(BO.CallData sortOrFilter)
    {
        return sortOrFilter switch
        {
            BO.CallData.Id => nameof(BO.CallInList.Id),
            BO.CallData.CallId => nameof(BO.CallInList.CallId),
            BO.CallData.CallType => nameof(BO.CallInList.CallType),
            BO.CallData.OpenTime => nameof(BO.CallInList.OpenTime),
            BO.CallData.LeftTime => nameof(BO.CallInList.LeftTime),
            BO.CallData.LastVolunteer => nameof(BO.CallInList.LastVolunteer),
            BO.CallData.CompletionTime => nameof(BO.CallInList.CompletionTime),
            BO.CallData.Status => nameof(BO.CallInList.Status),
            BO.CallData.TotalAssignments => nameof(BO.CallInList.TotalAssignments)
        };
    }

    internal static string GetPropertyName(BO.CloseCallData sortOrFilter)
    {
        return sortOrFilter switch
        {
            BO.CloseCallData.Id => nameof(BO.CloseCallData.Id),
            BO.CloseCallData.CallType => nameof(BO.CloseCallData.CallType),
            BO.CloseCallData.FullAddress => nameof(BO.CloseCallData.FullAddress),
            BO.CloseCallData.OpenTime => nameof(BO.CloseCallData.OpenTime),
            BO.CloseCallData.InterTime => nameof(BO.CloseCallData.InterTime),
            BO.CloseCallData.CloseTime => nameof(BO.CloseCallData.CloseTime),
            BO.CloseCallData.EndTreatment => nameof(BO.CloseCallData.EndTreatment),
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
        };
    }

    internal static string GetPropertyName(BO.OpenCallData sortOrFilter)
    {
        return sortOrFilter switch
        {
            BO.OpenCallData.Id => nameof(BO.OpenCallData.Id),
            BO.OpenCallData.CallType => nameof(BO.OpenCallData.CallType),
            BO.OpenCallData.Description => nameof(BO.OpenCallData.Description),
            BO.OpenCallData.FullAddress => nameof(BO.OpenCallData.FullAddress),
            BO.OpenCallData.OpenTime => nameof(BO.OpenCallData.OpenTime),
            BO.OpenCallData.MaxCloseTime => nameof(BO.OpenCallData.MaxCloseTime),
            BO.OpenCallData.VolDistance => nameof(BO.OpenCallData.VolDistance),
        };
    }

    internal static IEnumerable<BO.CallInList> ToCallInList(IEnumerable<DO.Call> Oldcalls, IEnumerable<DO.Assignment> OldAssignment)
    {
        IEnumerable<DO.Volunteer> OldVolunteer = s_dal.Volunteer.ReadAll(null);
        List<BO.CallInList>? callInLists = new List<BO.CallInList>();
        foreach (DO.Call item in Oldcalls)
        {
            DO.Assignment? CallAssignment = OldAssignment.LastOrDefault(a => a.CallId == item.Id) ?? null;

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
                callInLists.Add(new()
                {
                    Id = CallAssignment.Id,
                    CallId = item.Id,
                    CallType = (BO.CallType)item.CallType,
                    OpenTime = item.OpenTime,
                    LeftTime = (TimeSpan)(item.MaxTime - ClockManager.Now),
                    LastVolunteer = (OldVolunteer.FirstOrDefault(v => (v.Id == CallAssignment.VolunteerId)) ?? throw BLException).FullName,
                    CompletionTime = (CallAssignment.EndTreatment == DO.AssignmentEnum.TakenCare) ? CallAssignment.EndTime - item.OpenTime : null,
                    Status = MakeStatus(CallAssignment, item),
                    TotalAssignments = OldAssignment.Count(a => a.CallId == item.Id)
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
        string VolAddress = s_dal.Volunteer.Read(predicate).VolAddress;
        return new()
        {
            Id = item.Id,
            CallType = (BO.CallType)item.CallType,
            Description = item.Description,
            FullAddress = item.CallAddress,
            OpenTime = item.OpenTime,
            MaxCloseTime = item.MaxTime,
            VolDistance = Tools.CalculateDis(VolAddress, item.CallAddress)

        };
    }
}