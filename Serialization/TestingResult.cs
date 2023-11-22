using System.Net.NetworkInformation;

namespace PingTester.Serialization
{
	//T#4 --- structure to write to xml file, single result????

	/// <summary>
	/// Result of one single Ping to donated IP address or URL
	/// </summary>
	internal class TestingResult
    {
		public TestingResult(IPStatus status, long roundtripTime)
		{
			Status = status;
			RoundtripTime = roundtripTime;
		}

		public long RoundtripTime { get; }
		public IPStatus IpStatus { get; }
		public IPStatus Status { get; }
	}
}
