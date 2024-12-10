using BO;
using DalApi;

namespace Helpers;

internal static class CallManager
{
    private static IDal s_dal = Factory.Get; //stage 4

    internal static CallStatus MakeStatus(IEnumerable<DO.Assignment> dataAssignments, DateTime MaxCloseTime)
    {
        var endTreatment = from item in dataAssignments
                           select item.EndTreatment; //The time of the end of treatment. If the call doesn't end return null.
        var endTime = from item in dataAssignments
                      select item.EndTime; //The time the call should end

        if (endTreatment.LastOrDefault() != DO.AssignmentEnum.TakenCare) //if the Call doesn't taken care.
        {
            if ((MaxCloseTime - ClockManager.Now) < s_dal.Config.RiskRange) //if it in the riskTime.
                return CallStatus.OpenInRisk;
            else if (MaxCloseTime > ClockManager.Now) //If now is before the time to finish call
                return CallStatus.Opened;
        }

        else if (endTime != null) //The call has ended.
            return CallStatus.Closed;

        else if (endTime.Last() < ClockManager.Now) //The time to finish Call has already passed.
            return CallStatus.Expired;

        return CallStatus.InTreatment; //The Call in treatment.

        //else if (endTime.ClockManager.Now) //to vz

    }
}