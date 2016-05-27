using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using MeasurementCollectorIOT.Model;
using Windows.Storage;

namespace MeasurementCollectorIOT.Helpers
{
	internal class MeasurementFileSaver
	{
		public static string CreateFileName => DateTime.Now.ToString("MEAS_yyyyMMdd-HH.dat");

		public void CreateFileContents(IEnumerable<DeviceMeasurement> measurementData)
		{
			using (var data = new MemoryStream())
			{
				var serializer = new DataContractSerializer(typeof(IEnumerable<DeviceMeasurement>));
				serializer.WriteObject(data, measurementData);

				SaveMeasurementsToFileAsync(data);
			}
		}

		private async void SaveMeasurementsToFileAsync(MemoryStream memoryStream)
		{
			StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
			StorageFile measurementFile = await storageFolder.CreateFileAsync(CreateFileName, CreationCollisionOption.ReplaceExisting);

			using (var fileStream = await measurementFile.OpenStreamForWriteAsync())
			{
				memoryStream.Seek(0, SeekOrigin.Begin);
				await fileStream.CopyToAsync(memoryStream);
			}
		}
	}
}
