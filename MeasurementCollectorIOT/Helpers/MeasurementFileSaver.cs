using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using MeasurementCollectorIOT.Model;
using Windows.Storage;

namespace MeasurementCollectorIOT.Helpers
{
    internal class MeasurementFileSaver
    {
        public const int FILESAVE_INTERVAL = 60000; // 1 minute interval
        public const string EXPORT_FILENAME = "export.csv";

        public static string TemplateFileName => $"MEAS_COLLECT_{DateTime.Now.ToString("yyyyMMdd_HHmm")}.dat";

        private StorageFolder Folder => ApplicationData.Current.LocalFolder;
        private readonly DataContractSerializer _fileSerializer = new DataContractSerializer(typeof(List<DeviceMeasurement>));

        public async void CreateFileContents(List<DeviceMeasurement> measurementData)
        {
            if (measurementData?.Count() > 0)
            {
                StorageFile file = await Folder.CreateFileAsync(TemplateFileName, CreationCollisionOption.ReplaceExisting);
                using (Stream stream = await file.OpenStreamForWriteAsync())
                {
                    _fileSerializer.WriteObject(stream, measurementData);
                    await stream.FlushAsync();
                }
            }
        }

        public async Task ExportToCsv(IEnumerable<IStorageFile> files)
        {
            if (files == null)
                return;

            List<DeviceMeasurement> measurementData = new List<DeviceMeasurement>();

            foreach (IStorageFile file in files)
            {
                using (Stream stream = await file.OpenStreamForReadAsync())
                {
                    var fileStreamData = (List<DeviceMeasurement>)_fileSerializer.ReadObject(stream);
                    measurementData.AddRange(fileStreamData);
                }
            }

            StorageFile csvFile = await Folder.CreateFileAsync(EXPORT_FILENAME, CreationCollisionOption.ReplaceExisting);
            IList<string> csvData = measurementData.Select(d => $"{d.Index},{d.Value},{d.Taken.ToString("yyyy-MM-dd HH:mm:ss")}").ToList();
            string csv = string.Join(Environment.NewLine, csvData);
            await FileIO.WriteTextAsync(csvFile, csv);
        }

        public async Task<IEnumerable<StorageFile>> GetLocalFolderContents() => await Folder.GetFilesAsync();
    }
}
