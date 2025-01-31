using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace PL;
public class IntervalConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return !(bool)value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
public class AddressToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null )
            return Brushes.Red; // אם הכתובת חסרה
        if(value.ToString()== "-1")
            return Brushes.Red;
        if (value.ToString() == "-2")
            return Brushes.OrangeRed; // אם הכתובת לא נמצאה

        return Brushes.Black; // כתובת תקינה
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
public class AddressToToolTipConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
            return "Address is missing"; // אם הכתובת חסרה
        if (value.ToString() == "-1")
            return "Network connection failed, please try again later.";
        if (value.ToString() == "-2")
            return "Sorry, this is outside our scope of activity, but we'll be there soon:)"; // אם הכתובת לא נמצאה

        return "Correct address"; // כתובת תקינה
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}