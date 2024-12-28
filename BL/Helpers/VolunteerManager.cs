using BO;
using DalApi;


namespace Helpers;

internal static class VolunteerManager
{
    private static IDal s_dal = Factory.Get; //stage 4
    internal static ObserverManager Observers = new(); //stage 5 

    /// <summary>
    /// Checks that all values ​​are correctly formatted.
    /// </summary>
    /// <param name="volToCheck">The volunteer whose values ​​are being tested</param>
    /// <exception cref="BO.BlIntegrityOfValuesException">Throws an exception if there is a problem with the format of one of the values.</exception>
    internal static void CheckFormat(BO.Volunteer volToCheck)
    {
        bool? isEmail = checkEmail(volToCheck.Email) ?? throw new BO.BlIntegrityOfValuesException("Error in email format");
        if (!int.TryParse(volToCheck.PhoneNumber, out int number))
            throw new BO.BlIntegrityOfValuesException("Error in PhoneNumber format");
        if (volToCheck.PhoneNumber.Length != 9 || volToCheck.PhoneNumber[0] != '0' || volToCheck.PhoneNumber[1] != '5')
            throw new BO.BlIntegrityOfValuesException("Error in PhoneNumber format");
        if (volToCheck.MaxDis < 0/* || volToCheck.MaxDis == null*/)
            throw new BO.BlIntegrityOfValuesException("Error in Max Distance format");
        if (volToCheck.Id < 10000000 || volToCheck.Id > 999999999)
            throw new BO.BlIntegrityOfValuesException("Error in ID format");
    }

    /// <summary>
    /// Check that the email address received is correct
    /// </summary>
    /// <param name="email">The email address received</param>
    /// <returns>A boolean variable that evaluates to true when the email is correct.</returns>
    private static bool? checkEmail(string email)
    {
        if (email.Contains("@gmail.com") == false && email.Contains("@walla.co.il") == false && email.Contains("@g.jct.ac.il") == false)
            return null;
        if (email[0] == '.' || email[email.Length - 1] == '.')
            return null;
        if (email.Contains("..") == true || email.Contains(" ") == true)
            return null;

        return true;
    }


    /// <summary>
    /// Checking that the volunteer's values ​​are logically correct
    /// </summary>
    /// <param name="volToCheck">The volunteer whose values ​​are being tested</param>
    /// <exception cref="BO.BlIntegrityOfValuesException">>Throws an exception if there is a problem with the logic of one of the values</exception>
    internal static void CheckLogic(BO.Volunteer volToCheck)
    {
        if (checkId(volToCheck.Id) == false)
            throw new BO.BlIntegrityOfValuesException("Error in ID integrity");
        if (volToCheck.Address != null)
            CallManager.GetCoordinates(volToCheck.Address);
    }

    /// <summary>
    /// Check that the ID received is correct
    /// </summary>
    /// <param name="id">id for checking</param>
    /// <returns>A boolean variable that evaluates to true when the ID is correct.</returns>
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
        if ((id % 10 != ((10 - (sum % 10)) % 10)))
            return false;
        return true;
    }

    // <summary>
    /// Gets a volunteer from the database and returns the call he handle.
    /// </summary>
    /// <param name="id"> volunteer id</param>
    /// <param name="address">volunteer Address</param>
    /// <returns>returns the call the volunteer handle</returns>
    /// <exception cref="BO.BlNullPropertyException">Throws an exception when the  call does not exist </exception>
    internal static BO.CallInProgress? VolCall(int id, string address)
    {
        Func<DO.Assignment, bool> func = item => item.VolunteerId == id;
        DO.Assignment? CurrentAss = s_dal.Assignment.Read(func);
        if (CurrentAss is not null)/*?? throw new BO.BlNullPropertyException($"Assignment does not exist");*/
        {
            DO.Call CurrentCall = s_dal.Call.Read(CurrentAss.CallId) ?? throw new BO.BlNullPropertyException($"Call with ID: {CurrentAss.CallId} does not exist");
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
                status = (CurrentCall.MaxTime is null || (CurrentCall.MaxTime - AdminManager.Now) < s_dal.Config.RiskRange) ? Status.InTreatment : Status.InRiskTreatment
            };
        }
        return null;
    }


    /// <summary>
    /// Gets a volunteer from the database and returns it as a data entity of type volunteer in list
    /// </summary>
    /// <param name="OldVolunteer">volunteer from the database</param>
    /// <returns> returns it as a data entity of type volunteer in list</returns>
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
                FullName = item.FullName,
                IsActive = item.Active,
                HandleCalls = oldAssignments.Count(assignment => (assignment.VolunteerId == item.Id) && (assignment.EndTreatment == DO.AssignmentEnum.TakenCare)),
                ExpiredCalls = oldAssignments.Count(assignment => (assignment.VolunteerId == item.Id) && (assignment.EndTreatment == DO.AssignmentEnum.CancelExpired)),
                CancelCalls = oldAssignments.Count(assignment => (assignment.VolunteerId == item.Id) && (assignment.EndTreatment == DO.AssignmentEnum.SelfCancel || assignment.EndTreatment == DO.AssignmentEnum.CancelAdmin)),
                CallId = VolunteerAssignment != null ? VolunteerAssignment.CallId : null,
                InTreatment = VolunteerAssignment != null ? (BO.CallInTreatment)s_dal.Call.Read(VolunteerAssignment.CallId)!.CallType : BO.CallInTreatment.None
            });
        };
        return volunteerInLists.AsEnumerable();
    }

    /// <summary>
    /// Calculates the distance between the 2 given addresses
    /// </summary>
    /// <param name="volAddress"> volunteer address</param>
    /// <param name="callAddress"> call addresss</param>
    /// <returns></returns>
    public static double CalculateDis(string? volAddress, string callAddress)
    {
        if (volAddress is null || callAddress is null) return 0.0;
        Func<DO.Volunteer, bool> volPredicate = volunteer => volunteer.VolAddress == volAddress;
        Func<DO.Call, bool> callPredicate = call => call.CallAddress == callAddress;
        double volLatitude = (double)s_dal.Volunteer.Read(volPredicate).Latitude;
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


