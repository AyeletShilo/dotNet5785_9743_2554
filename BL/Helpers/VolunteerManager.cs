

using BO;
using DalApi;


namespace Helpers;

internal static class VolunteerManager
{
    private static IDal s_dal = Factory.Get; //stage 4


    internal static void CheckFormat(BO.Volunteer volToCheck)
    {

        bool isEmail = checkEmail(volToCheck.Email);

        bool PhoneIisNum = int.TryParse(volToCheck.PhoneNumber, out int number);

        bool PhoneIsCorrect = volToCheck.PhoneNumber.Length != 10 && volToCheck.PhoneNumber[0] == '0' && volToCheck.PhoneNumber[1] == '5';

        bool isDis = volToCheck.MaxDis > 0 || volToCheck.MaxDis == null;

        bool isId = volToCheck.Id > 9999999 && volToCheck.Id < 1000000000;

        if (isEmail == false || PhoneIisNum == false || PhoneIsCorrect == false || isDis == false || isId == false)
            throw new BO.BlIntegrityOfValuesException("Error in value integrity");

    }

    private static bool checkEmail(string email)
    {
        if (email.Contains("@gmail.com") == false && email.Contains("@walla.co.il") == false && email.Contains("@g.jct.ac.il") == false)
            return false;
        if (email[0] == '.' || email[email.Length - 1] == '.')
            return false;
        if (email.Contains("..") == true || email.Contains(" ") == true)
            return false;

        return true;
    }

    internal static void CheckLogic(BO.Volunteer volToCheck)
    {

        bool isId = checkId(volToCheck.Id);

        bool IsAddress = checkAddress(volToCheck.Address);

        if (isId == false || IsAddress == false)
            throw new BO.BlIntegrityOfValuesException("Error in value integrity");
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

    internal static BO.CallInProgress VolCall(int id, string address)
    {
        DO.Call CurrentCall = s_dal.Call.Read(id) ?? throw new BO.BlNullPropertyException($"Call with ID: {id} does not exist");
        Func<DO.Assignment, bool> func = item => item.CallId == CurrentCall.Id;
        DO.Assignment CurrentAss = s_dal.Assignment.Read(func) ?? throw new BO.BlNullPropertyException($"Assignment does not exist");
        return new()
        {
            Id = id,
            CallId = CurrentCall.Id,
            CallType = (BO.CallType)CurrentCall.CallType,
            Description = CurrentCall.Description,
            FullAddress = CurrentCall.CallAddress,
            OpenTime = CurrentCall.OpenTime,
            MaxCloseTime = CurrentCall.MaxTime,
            EntryTime = CurrentAss.InterTime,
            VolDistance = VolunteerManager.CalculateDis(address, CurrentCall.CallAddress),
            status = (CurrentCall.MaxTime - ClockManager.Now) < s_dal.Config.RiskRange ? Status.InTreatment : Status.InRiskTreatment
        };
    }


    internal static string GetPropertyName(BO.VolunteerData sortOrFilter)
    {
        return sortOrFilter switch
        {
            BO.VolunteerData.Id => nameof(BO.VolunteerInList.Id),
            BO.VolunteerData.FullName => nameof(BO.VolunteerInList.PullName),
            BO.VolunteerData.IsActive => nameof(BO.VolunteerInList.IsActive),
            BO.VolunteerData.HandleCalls => nameof(BO.VolunteerInList.HandleCalls),
            BO.VolunteerData.CancelCalls => nameof(BO.VolunteerInList.CancelCalls),
            BO.VolunteerData.ExpiredCalls => nameof(BO.VolunteerInList.ExpiredCalls),
            BO.VolunteerData.CallId => nameof(BO.VolunteerInList.CallId),
            BO.VolunteerData.InTreatment => nameof(BO.VolunteerInList.InTreatment)
        };
    }

    internal static IEnumerable<BO.VolunteerInList> ToVolunteerInList(IEnumerable<DO.Volunteer> OldVolunteer)
    {
        IEnumerable<DO.Assignment> oldAssignments = s_dal.Assignment.ReadAll(null);
        List<BO.VolunteerInList> volunteerInLists = new List<BO.VolunteerInList>();
        foreach (DO.Volunteer item in OldVolunteer)
        {
            DO.Assignment? VolunteerAssignment = oldAssignments.FirstOrDefault(assignment => (assignment.VolunteerId == item.Id) && (assignment.EndTime != null));
            volunteerInLists.Add(new()
            {
                Id = item.Id,
                PullName = item.FullName,
                IsActive = item.Active,
                HandleCalls = oldAssignments.Count(assignment => (assignment.VolunteerId == item.Id) && (assignment.EndTreatment == DO.AssignmentEnum.CancelExpired)),
                CancelCalls = oldAssignments.Count(assignment => (assignment.VolunteerId == item.Id) && (assignment.EndTreatment == DO.AssignmentEnum.TakenCare)),
                ExpiredCalls = oldAssignments.Count(assignment => (assignment.VolunteerId == item.Id) && (assignment.EndTreatment == DO.AssignmentEnum.SelfCancel || assignment.EndTreatment == DO.AssignmentEnum.CancelAdmin)),
                CallId = VolunteerAssignment != null ? VolunteerAssignment.CallId : null,
                InTreatment = VolunteerAssignment != null ? (BO.CallInTreatment)s_dal.Call.Read(VolunteerAssignment.CallId)!.CallType : BO.CallInTreatment.None
            });
        };
        return volunteerInLists.AsEnumerable();
    }
    public static double CalculateDis(string? volAddress, string callAddress)
    {
        if (volAddress is null || callAddress is null) return 0.0;
        Func<DO.Volunteer, bool> volPredicate = volunteer => volunteer.VolAddress == volAddress;
        Func<DO.Call, bool> callPredicate = call => call.CallAddress == callAddress;
        double volLatitude = (double)(s_dal.Volunteer.Read(volPredicate)).Latitude;
        double volLongitude = (double)s_dal.Volunteer.Read(volPredicate).Longitude;
        double callLatitude = (double)s_dal.Call.Read(callPredicate).Latitude;
        double callLongitude = (double)s_dal.Call.Read(callPredicate).Longitude;

        const double R = 6371; // רדיוס כדור הארץ בקילומטרים

        // המרת מעלות לרדיאנים
        double ToRadians(double angle) => Math.PI * angle / 180.0;

        double phi1 = ToRadians(volLatitude);
        double phi2 = ToRadians(callLatitude);

        double deltaLat = ToRadians(callLatitude - volLatitude);
        double deltaLon = ToRadians(callLongitude - volLongitude);

        // חישוב המרחק בקירוב לשטח מישורי
        double x = deltaLon * Math.Cos((phi1 + phi2) / 2);
        double y = deltaLat;

        double distance = R * Math.Sqrt(x * x + y * y); // נוסחת פיתגורס
        return distance;
    }


}