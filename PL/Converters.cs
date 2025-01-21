using BO;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace PL;

/// <summary>
/// 
/// </summary>
public class ConvertUpdateToTrue : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString() == "Update";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 
/// </summary>
public class ConvertUpdateToVisible : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string text && text.Equals("Update", StringComparison.OrdinalIgnoreCase))
        {
            return Visibility.Visible;
        }

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException("ConvertBack is not implemented");
    }
}

/// <summary>
/// 
/// </summary>
public class NullToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value == null ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 
/// </summary>
public class ConvertUpdateDetails : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        BO.CallStatus status = (BO.CallStatus)value;
        switch (status)
        {
            case BO.CallStatus.Opened:
                return false;
            case BO.CallStatus.OpenInRisk:
                return false;
            default: return true;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException("ConvertBack is not implemented");
    }
}

/// <summary>
/// 
/// </summary>
public class ConvertUpdateType : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        BO.CallStatus status = (BO.CallStatus)value;
        switch (status)
        {
            case BO.CallStatus.Opened:
                return true;
            case BO.CallStatus.OpenInRisk:
                return true;
            default: return false;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException("ConvertBack is not implemented");
    }
}

/// <summary>
/// 
/// </summary>
public class ConvertUpdateMaxTime : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        BO.CallStatus status = (BO.CallStatus)value;
        switch (status)
        {
            case BO.CallStatus.Closed:
                return true;
            case BO.CallStatus.Expired:
                return true;
            default: return false;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException("ConvertBack is not implemented");
    }
}

/// <summary>
/// 
/// </summary>
public class ConvertActiveIsEnable : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (BO.CallInProgress?)value == null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException("ConvertBack is not implemented");
    }
}

/// <summary>
/// 
/// </summary>
public class MultiToIsEnabledConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return (values[0] == null || (string)values[1] == "Add") ? Visibility.Hidden : Visibility.Visible;

        //if (values[0] == null || values[1] == null)
        //    Debug.WriteLine("One of the bindings is null.");

        //if (values[1] != null && !(values[1] is bool))
        //    Debug.WriteLine($"Unexpected type for isActive: {values[1].GetType()}");
        //return false;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 
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

/// <summary>
/// 
/// </summary>
class ConvertStatusToColor : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        BO.CallListStatus status = (BO.CallListStatus)value;
        switch (status)
        {
            case BO.CallListStatus.Opened:
                return Brushes.Yellow;
            case BO.CallListStatus.Closed:
                return Brushes.Orange;
            case BO.CallListStatus.InTreatment:
                return Brushes.Green;
            case BO.CallListStatus.Expired:
                return Brushes.PaleVioletRed;
            case BO.CallListStatus.OpenInRisk:
                return Brushes.Purple;
            case BO.CallListStatus.InTreatmentInRisk:
                return Brushes.Silver;
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
/// 
/// </summary>
class ConvertVolTypeToColor : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        BO.CallInTreatment callType = (BO.CallInTreatment)value;
        switch (callType)
        {
            case BO.CallInTreatment.Repairing:
                return Brushes.Yellow;
            case BO.CallInTreatment.Talking:
                return Brushes.Orange;
            case BO.CallInTreatment.Cleaning:
                return Brushes.Green;
            case BO.CallInTreatment.TechnologyHelp:
                return Brushes.PaleVioletRed;
            case BO.CallInTreatment.Shopping:
                return Brushes.Purple;
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
/// 
/// </summary>
public class ConvertStatusToVisible : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isFilter = (bool)value;
        if (isFilter == true)
        {
            return Visibility.Visible;
        }

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException("ConvertBack is not implemented");
    }
}

/// <summary>
/// In call list window, hides the delete button from the call row when the call cannot be deleted
/// </summary>
public class ConvertDeleteToVisible : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is BO.CallInList call)
        {
            if ((call.Status == BO.CallListStatus.Opened || call.Status == BO.CallListStatus.OpenInRisk) && call.TotalAssignments == 0)
                return Visibility.Visible;

        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException("ConvertBack is not implemented");
    }
}

public class FontSizeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        double height = (double)value;
        return height * 0.4; //Change according to height
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}


public class TimeSpanToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {

        if (value is TimeSpan timeSpan)
        {
            if (timeSpan.TotalHours >= 24)
                return $"{(int)timeSpan.TotalDays}d {timeSpan.Hours:00}:{timeSpan.Minutes:00}";
            else
                return $"{timeSpan.Hours:00}:{timeSpan.Minutes:00}";
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class TimeSpanToDateConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {

        if (value is TimeSpan timeSpan)
        {

            DateTime baseDate = DateTime.Today;

            DateTime resultDate = baseDate.Add(timeSpan);

            return resultDate.ToString("dd/MM/yy HH:mm", culture);
           
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
public class MaxTimeForEndTimeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DateTime? && value !=null)
        {
            DateTime dateTime= (DateTime)value;
            return dateTime.AddMonths(3);
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

