using BO;
using DO;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Channels;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
        if ((bool)value == false)
            return Brushes.Red; // Wrong address
        else
            return (SolidColorBrush)Application.Current.FindResource("HardGreenColor");// Correct
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
//public class AddressToToolTipConverter : IValueConverter
//{
//    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
//    {
//        if (value == null)
//            return "Address is missing"; // אם הכתובת חסרה
//        if (value.ToString() == "-1")
//            return "Network connection failed, please try again later.";
//        if (value.ToString() == "-2")
//            return "Sorry, this is outside our scope of activity, but we'll be there soon:)"; // אם הכתובת לא נמצאה

//        return "Correct address"; // כתובת תקינה
//    }

//    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
//    {
//        throw new NotImplementedException();
//    }
//}