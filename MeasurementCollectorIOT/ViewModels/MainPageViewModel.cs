using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MeasurementCollectorIOT.Helpers;
using MeasurementCollectorIOT.Model;
using Prism.Commands;
using Prism.Windows.Mvvm;

namespace MeasurementCollectorIOT.ViewModels
{
	public class MainPageViewModel : ViewModelBase
	{
		private MeasurementFileSaver fileSaver = new MeasurementFileSaver();

		public MainPageViewModel()
		{
			MeasurementsTaken = new ObservableCollection<DeviceMeasurement>();

			AcceptMeasurementEntry = new DelegateCommand<string>(AcceptMeasurement);
		}

		public ObservableCollection<DeviceMeasurement> MeasurementsTaken { get; private set; }

		public ICommand AcceptMeasurementEntry { get; private set; }

		public string Measurement { get; set; }

		private void AcceptMeasurement(string entry)
		{
			float value;
			if (float.TryParse(entry, out value))
			{
				int nextIndex = MeasurementsTaken.Count + 1;
				var measurement = DeviceMeasurement.CreateMeasurement(nextIndex, value);
				MeasurementsTaken.Add(measurement);

				fileSaver.CreateFileContents(MeasurementsTaken.ToList());
			}
		}
	}
}
