

using System.Reflection;
using BO;
namespace Helpers;

internal static class Tools
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
}
