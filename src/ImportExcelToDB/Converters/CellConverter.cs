using Domain.Enums;
using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ImportExcelToDB;
public class CellConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var input = (Status)value;
        switch (input)
        {
            case Status.Error:
                return Brushes.Red;
            case Status.Ready:
                return Brushes.Yellow;
            case Status.Imported:
                return Brushes.Green;
            default:
                return DependencyProperty.UnsetValue;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
