using System;
using System.Runtime.Serialization;

namespace MeasurementCollectorIOT.Model
{
	[DataContract]
	public class DeviceMeasurement
	{
		[DataMember]
		public int Index { get; private set; }

		[DataMember]
		public float Value { get; private set; }

		[DataMember]
		public DateTime Taken { get; private set; }

		public static DeviceMeasurement CreateMeasurement(int index, float value)
		{
			return new DeviceMeasurement { Index = index, Value = value, Taken = DateTime.Now };
		}

		public override bool Equals(object obj) => obj?.GetType() == typeof(DeviceMeasurement) && (obj as DeviceMeasurement).Index == Index;

		public override int GetHashCode() => Index.GetHashCode();
	}
}
