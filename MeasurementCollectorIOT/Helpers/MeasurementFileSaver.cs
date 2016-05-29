using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MeasurementCollectorIOT.Model;
using Windows.Storage;

namespace MeasurementCollectorIOT.Helpers
{
	internal class MeasurementFileSaver
	{
		public const int FILESAVE_INTERVAL = 60000; // 1 minute interval
		public const string EXPORT_FILENAME = "export.csv";

		public event EventHandler FileSaved;

		public static string CreateFileName => DateTime.Now.ToString("MEAS_COLLECT_yyyyMMdd-HHmm.dat");

		public void CreateFileContents(object state) => CreateFileContents((IEnumerable<DeviceMeasurement>)state);

		public void CreateFileContents(IEnumerable<DeviceMeasurement> measurementData)
		{
			if (measurementData?.Count() > 0)
				using (var fileStreamData = new MemoryStream())
				{
					var serializer = new DataContractSerializer(typeof(IEnumerable<DeviceMeasurement>));
					serializer.WriteObject(fileStreamData, measurementData);

					ArraySegment<byte> buffer;
					fileStreamData.TryGetBuffer(out buffer);
					SaveMeasurementsToFileAsync(buffer.Array, CreateFileName);
				}
		}

		public async Task ExportToCsv(IEnumerable<IStorageFile> files)
		{
			List<DeviceMeasurement> measurementData = new List<DeviceMeasurement>();
			var serializer = new DataContractSerializer(typeof(IEnumerable<DeviceMeasurement>));

			foreach (IStorageFile file in files)
			{
				using (var stream = await file.OpenStreamForReadAsync())
				{
					var fileStreamData = serializer.ReadObject(stream) as IEnumerable<DeviceMeasurement>;
					measurementData.AddRange(fileStreamData);
				}
			}

			IList<string> csvData = measurementData.Select(d => $"{d.Index},{d.Value},{d.Taken}").ToList();
			string csv = string.Join(Environment.NewLine, csvData);
			var bytes = Encoding.ASCII.GetBytes(csv);
			SaveMeasurementsToFileAsync(bytes, EXPORT_FILENAME);
		}

		public async Task<IEnumerable<StorageFile>> GetLocalFolderContents()
		{
			IList<string> files = new List<string>();

			StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
			return await storageFolder.GetFilesAsync();
		}

		private void OnFileSaved()
		{
			FileSaved?.Invoke(this, EventArgs.Empty);
		}

		private async void SaveMeasurementsToFileAsync(byte[] buffer, string fileName)
		{
			StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
			StorageFile measurementFile = await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

			using (var stream = await measurementFile.OpenStreamForWriteAsync())
			{
				try
				{
					stream.Write(buffer, 0, buffer.Length);
					OnFileSaved();
				}
				catch (Exception)
				{
				}
			}
		}
	}
}
