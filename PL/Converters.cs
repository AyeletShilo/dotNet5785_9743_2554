using BO;
using DO;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PL;


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
/// Removing the ability to update some call details based on call status
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
/// Removing the ability to update some call details based on call status
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
/// Removing the ability to update some call details based on call status
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
/// Does not allow updating a volunteer as inactive when there is a call in his care
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

public class CallMultiConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {

        if (values[0] != null || (bool)values[1] == false)
            return false;
        return true;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

#region visibility
/// <summary>
/// Hides the call in the volunteer's care details when there is no call or when it is for adding 
/// </summary>
public class MultiToIsEnabledConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return (values[0] == null ||(string)values[1] == "Add") ? Visibility.Hidden : Visibility.Visible;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}


/// <summary>
/// Does not allow changing the attribute when the window is used for adding rather than updating
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
/// Does not allow entry to the call selection screen when there is a call in the volunteer's care
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
/// Hiding the call filtering button when they are opened already filtered
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
#endregion

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

#region Time space
/// <summary>
/// convert timeSpan tobe on string format
/// </summary>
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

/// <summary>
/// convert imeSpan to be on string format
/// </summary>
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

/// <summary>
/// convert risk range to show only days
/// </summary
public class TimeSpanToDaysConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TimeSpan timeSpan)
        {
            return timeSpan.TotalDays.ToString("0");
        }
        return "0";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {

        if (double.TryParse(value.ToString(), out double days))
        {
            return TimeSpan.FromDays(days);
        }
        return TimeSpan.Zero;
    }
}

public class DateTimeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yy HH:mm");
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string input = value as string;
        DateTime parsedDate;

        if (string.IsNullOrWhiteSpace(input))
            return DependencyProperty.UnsetValue;

        if (Regex.IsMatch(input, @"^\d{1,2}/\d{1,2}/\d{2,4}$"))
        {
            parsedDate = DateTime.ParseExact(input, "dd/MM/yy", CultureInfo.InvariantCulture);
            parsedDate = parsedDate.AddHours(8);
        }
        else if (!DateTime.TryParseExact(input, "dd/MM/yy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
        {
            return DependencyProperty.UnsetValue;
        }

        return parsedDate;
    }
}
#endregion