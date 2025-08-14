using Microsoft.Maui.Controls;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SprintTrack.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Colors.Green : Colors.Red;
            }
            return Colors.Gray;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToBorderColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Colors.DarkGreen : Colors.DarkRed;
            }
            return Colors.Gray;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToFontAttributesConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue && boolValue)
            {
                return FontAttributes.Bold;
            }
            return FontAttributes.None;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToTextColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                // When IsCurrentMonth is true, use dark text for good readability
                // When IsCurrentMonth is false, use lighter gray for days from other months
                return boolValue ? Color.FromArgb("#242424") : Color.FromArgb("#919191");
            }
            return Color.FromArgb("#919191");
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CalendarDayTextColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isCurrentMonth)
            {
                // Use dark text for current month days, lighter gray for other months
                return isCurrentMonth ? Color.FromArgb("#1f1f1f") : Color.FromArgb("#ACACAC");
            }
            return Color.FromArgb("#ACACAC");
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToWarmupColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isWarmup)
            {
                // Orange color for warmup sets, default dark color for normal sets
                return isWarmup ? Color.FromArgb("#FF8C00") : Color.FromArgb("#333333");
            }
            return Color.FromArgb("#333333");
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CountToBoolConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int count)
            {
                return count > 0;
            }
            return false;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StringToBoolConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return !string.IsNullOrWhiteSpace(value?.ToString());
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class InvertedBoolConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return false;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return false;
        }
    }

    public class InvertedStringToBoolConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return string.IsNullOrWhiteSpace(value?.ToString());
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToSelectedBackgroundConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isSelected && isSelected)
            {
                return Color.FromArgb("#E3F2FD"); // Light blue background
            }
            return Colors.Transparent;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ExerciseTypeToVisibilityConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is SprintTrack.Models.ExerciseType exerciseType && parameter is string targetTypeString)
            {
                return exerciseType.ToString() == targetTypeString;
            }
            return false;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class OrConverter : IMultiValueConverter
    {
        public object? Convert(object[]? values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values == null)
                return false;

            foreach (var value in values)
            {
                if (value is bool boolValue && boolValue)
                {
                    return true;
                }
            }
            return false;
        }

        public object[]? ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Numeric input validation behaviors
    public class NumericValidationBehavior : Behavior<Entry>
    {
        public static readonly BindableProperty AllowDecimalProperty = 
            BindableProperty.Create(nameof(AllowDecimal), typeof(bool), typeof(NumericValidationBehavior), false);

        public static readonly BindableProperty MaxDecimalPlacesProperty = 
            BindableProperty.Create(nameof(MaxDecimalPlaces), typeof(int), typeof(NumericValidationBehavior), 2);

        public bool AllowDecimal
        {
            get => (bool)GetValue(AllowDecimalProperty);
            set => SetValue(AllowDecimalProperty, value);
        }

        public int MaxDecimalPlaces
        {
            get => (int)GetValue(MaxDecimalPlacesProperty);
            set => SetValue(MaxDecimalPlacesProperty, value);
        }

        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(entry);
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(entry);
        }

        private void OnEntryTextChanged(object? sender, TextChangedEventArgs e)
        {
            if (sender is not Entry entry)
                return;

            var newText = e.NewTextValue;

            if (string.IsNullOrEmpty(newText))
                return;

            // Create regex pattern based on AllowDecimal setting
            string pattern;
            if (AllowDecimal)
            {
                // Allow digits, comma, and period for decimal input
                // Only allow one decimal separator and limit decimal places
                pattern = MaxDecimalPlaces > 0 
                    ? $@"^\d*[,.]?\d{{0,{MaxDecimalPlaces}}}$"
                    : @"^\d*[,.]?\d*$";
            }
            else
            {
                // Only allow digits for integer input
                pattern = @"^\d*$";
            }

            if (!Regex.IsMatch(newText, pattern))
            {
                // Remove the last character that made it invalid
                entry.Text = e.OldTextValue;
            }
            else if (AllowDecimal && newText.Contains(","))
            {
                // Replace comma with period for internal consistency
                var correctedText = newText.Replace(",", ".");
                if (correctedText != newText)
                {
                    entry.Text = correctedText;
                }
            }
        }
    }

    public class IntegerOnlyBehavior : Behavior<Entry>
    {
        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(entry);
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(entry);
        }

        private void OnEntryTextChanged(object? sender, TextChangedEventArgs e)
        {
            if (sender is not Entry entry)
                return;

            var newText = e.NewTextValue;

            if (string.IsNullOrEmpty(newText))
                return;

            // Only allow digits
            if (!Regex.IsMatch(newText, @"^\d*$"))
            {
                entry.Text = e.OldTextValue;
            }
        }
    }

    public class DecimalInputBehavior : Behavior<Entry>
    {
        public static readonly BindableProperty MaxDecimalPlacesProperty = 
            BindableProperty.Create(nameof(MaxDecimalPlaces), typeof(int), typeof(DecimalInputBehavior), 2);

        public int MaxDecimalPlaces
        {
            get => (int)GetValue(MaxDecimalPlacesProperty);
            set => SetValue(MaxDecimalPlacesProperty, value);
        }

        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(entry);
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(entry);
        }

        private void OnEntryTextChanged(object? sender, TextChangedEventArgs e)
        {
            if (sender is not Entry entry)
                return;

            var newText = e.NewTextValue;

            if (string.IsNullOrEmpty(newText))
                return;

            // Allow digits, comma, and period for decimal input
            // Only allow one decimal separator and limit decimal places
            string pattern = MaxDecimalPlaces > 0 
                ? $@"^\d*[,.]?\d{{0,{MaxDecimalPlaces}}}$"
                : @"^\d*[,.]?\d*$";

            if (!Regex.IsMatch(newText, pattern))
            {
                entry.Text = e.OldTextValue;
            }
            else if (newText.Contains(","))
            {
                // Replace comma with period for internal consistency
                var correctedText = newText.Replace(",", ".");
                if (correctedText != newText)
                {
                    entry.Text = correctedText;
                }
            }
        }
    }

    public class InvertedCountToBoolConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int count)
            {
                return count == 0; // Return true when count is 0 (no exercises)
            }
            return true; // Default to showing the "no exercises" message
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}