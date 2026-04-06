using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace HackatonUi.Converters
{
    public class DecisionStatusToBrushConverter : IValueConverter
    {
        /// <summary>
        /// 1 ("Не начато") – Gray,
        /// 2 ("В процесе")  – Orange,
        /// 3 ("Завершен")   – Green,
        /// 4 ("Просроченно") – Red.
        /// Для остальных значений – Gray.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int status)
            {
                switch (status)
                {
                    case 1:
                        return Brushes.Gray;
                    case 2:
                        return Brushes.Orange;
                    case 3:
                        return Brushes.Green;
                    case 4:
                        return Brushes.Red;
                    default:
                        return Brushes.Gray;
                }
            }
            return Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}