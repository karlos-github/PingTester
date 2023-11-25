using System.Net.NetworkInformation;

namespace PingTester.Serialization
{
	//T#4 --- structure to write to xml file

	/// <summary>
	/// Result of some pings called to donated IP address for the some period of time
	/// </summary>
	internal class TestPing
	{
		public Guid Guid { get; set; } = Guid.NewGuid();
		public string IP { get; set; }
		public int RoundtripTime { get; set; }
		public IPStatus Status { get; set; }
		public string TimeStamp { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);

		public override string ToString() => $"{IP} {TimeStamp} {Status} {RoundtripTime} {Guid}";
	}
}
