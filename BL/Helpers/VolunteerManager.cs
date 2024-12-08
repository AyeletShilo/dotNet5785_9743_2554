

using DalApi;


namespace Helpers;

internal static class VolunteerManager
{
    private static IDal s_dal = Factory.Get; //stage 4


    internal static void CheckFormat(BO.Volunteer volToCheck)
    {

        bool isEmail = checkEmail(volToCheck.Email);

        bool PhoneIisNum = int.TryParse(volToCheck.PhoneNumber, out int number);

        bool PhoneIsCorrect = volToCheck.PhoneNumber.Length != 10 && volToCheck.PhoneNumber[0]== '0' && volToCheck.PhoneNumber[1] == '5';

        bool isDis = volToCheck.MaxDis > 0 || volToCheck.MaxDis == null;

        if (isEmail == false || PhoneIisNum == false || PhoneIsCorrect == false || isDis==false )
            throw  Exception;
        
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

        bool isId;

        bool IsAddress;

        
        if (isId == false || IsAddress == false)
            throw..;
       
    }
}

    
