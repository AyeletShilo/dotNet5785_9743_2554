

using System.Reflection;
using BO;
namespace Helpers;

internal class Tools
{
    public static string ToStringProperty<T>(this T t)
    {
        string str = "";
        foreach (PropertyInfo item in t.GetType().GetProperties())
        {
            str += "\n" + item.Name + ": " + item.GetValue(item);
        }
        return str;
        //Type Ttype = t.GetType();
        //PropertyInfo[] info = Ttype.GetProperties();

    }

    //internal static void CheckLogic<T>(this T toCheck) //where T: BO.Volunteer, BO.Call
    //{
    //    if (toCheck is Call || toCheck is Volunteer)
    //    {
    //        bool isId = checkId(toCheck.Id);

    //        bool IsAddress = checkAddress(toCheck.Address);

    //        if (isId == false || IsAddress == false)
    //            throw new BO.IntegrityOfValuesException("Error in value integrity");
    //    }
    //}

    //private static bool checkId(int id)
    //{
    //    int sum = 0;
    //    string idString = id.ToString();
    //    for (int i = 0; i < 8; i++)
    //    {
    //        int digit = idString[i] - '0'; // המרת התו למספר
    //        int multiplier = (i % 2 == 0) ? 1 : 2; // זוגי/אי-זוגי
    //        int product = digit * multiplier;
    //        sum += (product > 9) ? product - 9 : product; // סכום הספרות
    //    }
    //    if (idString[8] != (10 - (sum % 10)) % 10)
    //        return false;
    //    return true;
    //}
    //private static bool checkAddress(string address)
    //{
    //    if (address == null) return true;

    //    //??
    //    //עדכון קווי אורך רוחב
    //    return true;
    //}

    public static double CalculateDis(string volAddress, string CallAddress)
    {



        return 3.4;
    }

}
