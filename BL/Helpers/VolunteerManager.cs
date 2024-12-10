

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
            throw new BO.IntegrityOfValuesException("Error in value integrity");

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

    internal static BO.CallInProgress VolCall(int id, string address)
    {
        DO.Call CurrentCall = s_dal.Call.Read(id);
        Func<DO.Assignment, bool> func = item => item.CallId == CurrentCall.Id;
        DO.Assignment CurrentAss = s_dal.Assignment.Read(func);
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
            VolDistance = CalculateDis(address, CurrentCall.CallAddress),
            status = (CurrentCall.MaxTime - ClockManager.Now) < s_dal.Config.RiskRange ? Status.InTreatment : Status.InRiskTreatment
        };
    }

    private static double CalculateDis(string volAddress, string CallAddress)
    {



        return 3.4;
    }
}


    
