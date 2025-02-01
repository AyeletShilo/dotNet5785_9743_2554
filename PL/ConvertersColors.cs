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

//public class SimulatorChengeConverter : IValueConverter
//{
//    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
//    {
//        if (value is TimeSpan timeSpan)
//        {
//            return timeSpan.TotalDays.ToString("0");
//        }
//        return "0";
//    }

//    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
//    {

//        if (double.TryParse(value.ToString(), out double days))
//        {
//            return TimeSpan.FromDays(days);
//        }
//        return TimeSpan.Zero;
//    }
//}
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


/// <summary>
/// Changes the background of call in list according to the call type
/// </summary>
class ConvertCallTypeToColor : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        BO.CallType callType = (BO.CallType)value;
        switch (callType)
        {
            case BO.CallType.Repairing:
                return (SolidColorBrush)Application.Current.FindResource("Color1");
            case BO.CallType.Talking:
                return (SolidColorBrush)Application.Current.FindResource("Color2");
            case BO.CallType.Cleaning:
                return (SolidColorBrush)Application.Current.FindResource("Color3");
            case BO.CallType.TechnologyHelp:
                return (SolidColorBrush)Application.Current.FindResource("Color4");
            case BO.CallType.Shopping:
                return (SolidColorBrush)Application.Current.FindResource("Color5");
            default:
                return Brushes.White;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Changes the font of call in the list according to the call type
/// </summary>
class ConvertFontColor : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        BO.CallType callType = (BO.CallType)value;
        switch (callType)
        {
            case BO.CallType.Repairing:
            case BO.CallType.Talking:
                return (SolidColorBrush)Application.Current.FindResource("AyeletHardColor");
            case BO.CallType.Cleaning:
            case BO.CallType.TechnologyHelp:
            case BO.CallType.Shopping:
                return (SolidColorBrush)Application.Current.FindResource("CreamColor");
            default:
                return (SolidColorBrush)Application.Current.FindResource("AyeletHardColor");
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}


///// <summary>
/////  Changes the background of call in list according to the call type
///// </summary>
//class ConvertVolTypeToColor : IValueConverter
//{
//    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
//    {
//        BO.CallInTreatment callType = (BO.CallInTreatment)value;
//        switch (callType)
//        {
//            case BO.CallInTreatment.Repairing:
//                return Brushes.Yellow;
//            case BO.CallInTreatment.Talking:
//                return Brushes.Orange;
//            case BO.CallInTreatment.Cleaning:
//                return Brushes.Green;
//            case BO.CallInTreatment.TechnologyHelp:
//                return Brushes.PaleVioletRed;
//            case BO.CallInTreatment.Shopping:
//                return Brushes.Purple;
//            default:
//                return Brushes.White;
//        }
//    }

//    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
//    {
//        throw new NotImplementedException();
//    }
//}


public class ConvertStatusToColor : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        BO.CallListStatus callType = (BO.CallListStatus)value;
        switch (callType)
        {
            case BO.CallListStatus.Opened:
                return (SolidColorBrush)Application.Current.FindResource("status1");
            case BO.CallListStatus.InTreatment:
                return (SolidColorBrush)Application.Current.FindResource("status3");
            case BO.CallListStatus.InTreatmentInRisk:
                return (SolidColorBrush)Application.Current.FindResource("status4");
            case BO.CallListStatus.Expired:
                return (SolidColorBrush)Application.Current.FindResource("Color1");
            case BO.CallListStatus.Closed:
                return (SolidColorBrush)Application.Current.FindResource("status5");
            case BO.CallListStatus.OpenInRisk:
                return (SolidColorBrush)Application.Current.FindResource("status2");
            default:
                return Brushes.White;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException("ConvertBack is not implemented");
    }
}

public class ConvertStatusToFontColor : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        BO.CallListStatus callType = (BO.CallListStatus)value;
        switch (callType)
        {
            case BO.CallListStatus.Opened:
            case BO.CallListStatus.OpenInRisk:
            case BO.CallListStatus.InTreatment:
            case BO.CallListStatus.Expired:
                return (SolidColorBrush)Application.Current.FindResource("AyeletHardColor");
            case BO.CallListStatus.InTreatmentInRisk:
            case BO.CallListStatus.Closed:
                return (SolidColorBrush)Application.Current.FindResource("CreamColor");

            default:
                return Brushes.Black;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException("ConvertBack is not implemented");
    }
}

class ConvertCallTypeToPitcher : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        BO.CallType callType = (BO.CallType)value;
        switch (callType)
        {
            case BO.CallType.Repairing:
                return new BitmapImage(new Uri("pack://application:,,,/Image/repairing.png"));
            case BO.CallType.Talking:
                return new BitmapImage(new Uri("pack://application:,,,/Image/talking.png"));
            case BO.CallType.Cleaning:
                return new BitmapImage(new Uri("pack://application:,,,/Image/cleaning.png"));
            case BO.CallType.TechnologyHelp:
                return new BitmapImage(new Uri("pack://application:,,,/Image/tech.png"));
            case BO.CallType.Shopping:
                return new BitmapImage(new Uri("pack://application:,,,/Image/Shopping.png"));
            default:
                return null;
        }
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