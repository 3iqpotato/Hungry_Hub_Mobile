using System.Globalization;

namespace Hungry_Hub_Mobile.Converters;

public class FilterTextColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string selectedFilter && parameter is string filterValue)
        {
            return selectedFilter == filterValue ? Colors.White : Colors.Black;
        }
        return Colors.Black;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}