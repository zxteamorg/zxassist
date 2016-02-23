namespace org.zxteam.zxassist
{
	using System;
	using System.Globalization;
	using System.Windows;
	using System.Windows.Data;

	/// <summary>
	/// Value converter that translates true to <see cref="Visibility.Visible"/> and false to
	/// <see cref="Visibility.Collapsed"/>.
	/// </summary>
	public sealed class BooleanToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool requestValue = (value is bool && (bool)value);

			if (parameter != null && "!".Equals(parameter))
			{
				requestValue = !requestValue;
			}

			return requestValue ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool result = value is Visibility && (Visibility)value == Visibility.Visible;
			if (parameter != null && "!".Equals(parameter))
			{
				result = !result;
			}
			return result;
		}
	}
}
