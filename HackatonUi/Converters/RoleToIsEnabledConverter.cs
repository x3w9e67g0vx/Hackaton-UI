using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace HackatonUi.Converters
{
    public class RoleToIsEnabledConverter : IValueConverter
    {
        // Если роль равна "Admin" или "Expert" – true, иначе false.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string role)
            {
                return role.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                       role.Equals("Expert", StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}