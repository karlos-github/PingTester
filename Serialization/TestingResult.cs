using System.Net.NetworkInformation;

namespace PingTester.Serialization
{
	//T#4 --- structure to write to xml file, single result????

	/// <summary>
	/// Result of one single Ping to donated IP address or URL
	/// </summary>
	internal class TestingResult
    {
        public long RoundtripTime { get; }
		public IPStatus IpStatus { get; }
	}
}
