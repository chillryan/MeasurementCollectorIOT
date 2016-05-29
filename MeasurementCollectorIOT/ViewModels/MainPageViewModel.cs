using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Input;
using MeasurementCollectorIOT.Helpers;
using MeasurementCollectorIOT.Model;
using Prism.Commands;
using Prism.Windows.Mvvm;
using Windows.Storage;

namespace MeasurementCollectorIOT.ViewModels
{
	public class MainPageViewModel : ViewModelBase
	{
		private MeasurementFileSaver _fileSaver = new MeasurementFileSaver();
		private Timer _autoFileSaveTimer;

		public MainPageViewModel()
		{
			MeasurementsTaken = new ObservableCollection<DeviceMeasurement>();
			AcceptMeasurementEntryCommand = new DelegateCommand<string>(AcceptMeasurement);
			ExportEntriesToCsvCommand = new DelegateCommand<IEnumerable<IStorageFile>>(ExportEntriesToCsv);

			_fileSaver.FileSaved += fileSaver_FileSaved;
			_autoFileSaveTimer = new Timer(_fileSaver.CreateFileContents, MeasurementsTaken, MeasurementFileSaver.FILESAVE_INTERVAL, MeasurementFileSaver.FILESAVE_INTERVAL);
		}

		public ObservableCollection<DeviceMeasurement> MeasurementsTaken { get; private set; }

		public ObservableCollection<StorageFile> Files { get; private set; }

		public ICommand AcceptMeasurementEntryCommand { get; private set; }

		public ICommand ExportEntriesToCsvCommand { get; private set; }

		public string Measurement { get; set; }

		private void AcceptMeasurement(string entry)
		{
			float value;
			if (float.TryParse(entry, out value))
			{
				int nextIndex = MeasurementsTaken.Count + 1;
				var measurement = DeviceMeasurement.CreateMeasurement(nextIndex, value);
				MeasurementsTaken.Add(measurement);

				_autoFileSaveTimer.Change(MeasurementFileSaver.FILESAVE_INTERVAL, MeasurementFileSaver.FILESAVE_INTERVAL);
			}
		}

		private async void fileSaver_FileSaved(object sender, EventArgs e)
		{
			MeasurementsTaken.Clear();
			Files = new ObservableCollection<StorageFile>(await _fileSaver.GetLocalFolderContents());

			OnPropertyChanged(() => MeasurementsTaken);
			OnPropertyChanged(() => Files);
		}

		private async void ExportEntriesToCsv(IEnumerable<IStorageFile> selectedFiles)
		{
			await _fileSaver.ExportToCsv(selectedFiles);
		}
	}
}
