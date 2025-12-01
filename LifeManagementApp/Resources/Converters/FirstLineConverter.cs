using System.Globalization;

namespace LifeManagementApp.Resources.Converters
{
    public class FirstLineConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var text = value as string ?? "";
            if (string.IsNullOrEmpty(text)) return "";
            var line = text.Split('\n').FirstOrDefault() ?? "";
            return line.Length > 50 ? line.Substring(0, 50) + "..." : line;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}