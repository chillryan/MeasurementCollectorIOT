using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace MeasurementCollectorIOT.Helpers
{
	internal class SelectionChangeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var listView = parameter as ListView;

			return listView?.SelectedItems;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
