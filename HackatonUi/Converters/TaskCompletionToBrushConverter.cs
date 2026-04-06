using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Media;
using Avalonia.Data.Converters;
using HackatonUi.Models;

namespace HackatonUi.Converters
{
    public class TaskCompletionToBrushConverter : IValueConverter
    {
        ///<summary>
        /// Если у задачи нет решений – возвращается Red.
        /// Если все решения имеют статус 3 ("Завершен") – возвращается Green.
        /// Если хотя бы одно решение имеет статус 3, но не все – возвращается Yellow.
        /// Иначе возвращается Red.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IList<TaskDecision> decisions)
            {
                if (decisions.Count == 0)
                    return Brushes.Red;

                /// Если все решения имеют статус "Завершен" (предположим id = 3)
                if (decisions.All(d => d.StatusId == 3))
                    return Brushes.Green;

                // Если хоть одно решение "Завершен"
                if (decisions.Any(d => d.StatusId == 3))
                    return Brushes.Yellow;
                
                return Brushes.Red;
            }
            return Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}