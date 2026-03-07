using System.Globalization;

namespace Hungry_Hub_Mobile.Converters;

public class FilterSelectedConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string selectedFilter && parameter is string filterValue)
        {
            return selectedFilter == filterValue ? "#2196F3" : "#F0F0F0";
        }
        return "#F0F0F0";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}