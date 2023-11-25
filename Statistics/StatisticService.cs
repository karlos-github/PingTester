using PingTester.Serialization;

namespace PingTester.Statistics
{
	internal class StatisticService : IStatisticService
	{
		ISerializerService _serializer;
		IDictionary<string, PingStatistic> _statistics = new Dictionary<string, PingStatistic>();

		public StatisticService(ISerializerService serializer) => _serializer = serializer;

		public void OutputStatistics()
		{
			GetData();
			Console.Clear();
			foreach (var key in _statistics.Keys)
			{
				if (_statistics.TryGetValue(key, out var statistic))
				{
					Console.WriteLine($"Pinging [{key}] statistics:");
					Console.WriteLine($"Availability = {(double)statistic.SuccessStatus / statistic.Sent * 100:#.000} %, Sent = {statistic.Sent} packets");
					Console.WriteLine($"Approximate round trip times in milli - seconds:");
					Console.WriteLine($"Minimum = {statistic.MinimumRoundTrip}ms, Maximum = {statistic.MaximumRoundTrip}ms, Average = {statistic.AvarageRoundTrip}ms");
					Console.WriteLine();
				}
			}
		}

		void GetData()
		{
			_serializer.Deserialize(out List<TestPing> testPings);

			ProcessData(testPings);
		}

		void ProcessData(IEnumerable<TestPing> testPings)
		{
			foreach (var testPing in testPings)
			{
				if (_statistics.ContainsKey(testPing.IP))
				{
					_statistics[testPing.IP].Sent++;
					if (testPing.Status == System.Net.NetworkInformation.IPStatus.Success)
						_statistics[testPing.IP].SuccessStatus++;
					if (testPing.RoundtripTime > _statistics[testPing.IP].MaximumRoundTrip)
						_statistics[testPing.IP].MaximumRoundTrip = testPing.RoundtripTime;
					if (testPing.RoundtripTime < _statistics[testPing.IP].MinimumRoundTrip)
						_statistics[testPing.IP].MinimumRoundTrip = testPing.RoundtripTime;

					//TODO- calculate average round trip!!!!!! RoundTripFrequencies
					_statistics[testPing.IP].AvarageRoundTrip = (int)((_statistics[testPing.IP].MaximumRoundTrip + _statistics[testPing.IP].MinimumRoundTrip) / 2);
				}
				else
				{
					_statistics.Add(testPing.IP, new PingStatistic()
					{
						MaximumRoundTrip = int.MinValue,
						MinimumRoundTrip = int.MaxValue,
						Sent = 1,
						SuccessStatus = 1,
					});
				}
			}
		}
	}
}
