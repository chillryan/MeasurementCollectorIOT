using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MeasurementCollectorIOT.Helpers;
using MeasurementCollectorIOT.Model;
using Prism.Commands;
using Prism.Windows.Mvvm;
using Windows.Storage;
using Windows.UI.Xaml;

namespace MeasurementCollectorIOT.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private MeasurementFileSaver _fileSaver = new MeasurementFileSaver();
        private DispatcherTimer _timer;
        private IList<StorageFile> _selectedFiles = new List<StorageFile>();

        public MainPageViewModel()
        {
            MeasurementsTaken = new ObservableCollection<DeviceMeasurement>();
            AcceptMeasurementEntryCommand = new DelegateCommand<string>(AcceptMeasurement);
            DeleteFilesCommand = new DelegateCommand(DeleteFiles);
            ExportToCsvCommand = new DelegateCommand(ExportToCsv);
            UpdateSelectedFileListCommand = new DelegateCommand<IEnumerable<StorageFile>>(UpdateSelectedFileList);

            _timer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 30) };
            _timer.Tick += AutoFileSaveElapsed;

            ClearUI();
        }

        public ObservableCollection<DeviceMeasurement> MeasurementsTaken { get; private set; }

        public ObservableCollection<StorageFile> Files { get; private set; }

        public ICommand AcceptMeasurementEntryCommand { get; private set; }

        public ICommand DeleteFilesCommand { get; private set; }

        public ICommand ExportToCsvCommand { get; private set; }

        public ICommand UpdateSelectedFileListCommand { get; private set; }

        public string Measurement { get; set; }

        private void AcceptMeasurement(string entry)
        {
            _timer.Stop();

            float value;
            if (float.TryParse(entry, out value))
            {
                int nextIndex = MeasurementsTaken.Count + 1;
                var measurement = DeviceMeasurement.CreateMeasurement(nextIndex, value);
                MeasurementsTaken.Add(measurement);
            }

            _timer.Start();
        }

        private void AutoFileSaveElapsed(object sender, object e)
        {
            _fileSaver.CreateFileContents(MeasurementsTaken.ToList());
            ClearUI();
        }

        private async void ClearUI()
        {
            MeasurementsTaken.Clear();
            Files = new ObservableCollection<StorageFile>(await _fileSaver.GetLocalFolderContents());
            OnPropertyChanged(() => MeasurementsTaken);
            OnPropertyChanged(() => Files);
        }

        private async void DeleteFiles()
        {
            foreach (StorageFile file in _selectedFiles)
            {
                await file.DeleteAsync();
                Files.Remove(file);
            }
            _selectedFiles.Clear();
            OnPropertyChanged(() => Files);
        }

        private async void ExportToCsv()
        {
            await _fileSaver.ExportToCsv(_selectedFiles);
        }

        private void UpdateSelectedFileList(IEnumerable<StorageFile> selectedFiles)
        {
            _selectedFiles = new List<StorageFile>(selectedFiles);
        }
    }
}
