using BO;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

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

public class MultiToIsEnabledConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return (values[0] == null && (bool)values[1] == true);

        if (values[0] == null || values[1] == null)
            Debug.WriteLine("One of the bindings is null.");

        if (values[1] != null && !(values[1] is bool))
            Debug.WriteLine($"Unexpected type for isActive: {values[1].GetType()}");
        return false;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}


class ConvertCallTypeToColor : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        BO.CallType callType = (BO.CallType)value;
        switch (callType)
        {
            case BO.CallType.Repairing:
                return Brushes.Yellow;
            case BO.CallType.Talking:
                return Brushes.Orange;
            case BO.CallType.Cleaning:
                return Brushes.Green;
            case BO.CallType.TechnologyHelp:
                return Brushes.PaleVioletRed;
            case BO.CallType.Shopping:
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