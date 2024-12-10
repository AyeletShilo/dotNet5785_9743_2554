using BO;
using DalApi;

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

        if (dataAssignments == null)
        {
            if (MaxCloseTime < ClockManager.Now)
                return CallStatus.Opened;
            else if ((ClockManager.Now - MaxCloseTime) < IConfig.RiskRange())
                return CallStatus.OpenInRisk;
        }


        else if (endTime != null)
            return CallStatus.Closed;
        else if (endTime.ClockManager.Now) //to vz

    }
}
L