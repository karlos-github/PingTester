using PingTester.ArgumentService;
using PingTester.OutputService;
using PingTester.Serialization;

namespace PingTester.Statistics
{
	internal class StatisticService : IStatisticService
	{
		ISerializerService _serializer;
		IOutputStrategyManager _outputStrategyManager;
		IDictionary<string, PingStatistic> _statistics = new Dictionary<string, PingStatistic>();

		public StatisticService(ISerializerService serializer, IOutputStrategyManager outputStrategyManager)
		{
			_serializer = serializer;
			_outputStrategyManager = outputStrategyManager;
		}

		public async Task OutputStatistics(StatisticsOutputType outputtype)
		{
			ProcessData(await _serializer.Deserialize());
			TryClearPreviousOutputs();

			IOutputStrategy outputStrategy = _outputStrategyManager.CreateOutputStrategy(outputtype);
			outputStrategy.Output(_statistics);
		}

		void TryClearPreviousOutputs()
		{
			//Trying to delete serialized data from any previous run
			if (File.Exists(Path.Combine(Environment.CurrentDirectory, @$"{nameof(PingStatistic)}.xml")))
				File.Delete(Path.Combine(Environment.CurrentDirectory, @$"{nameof(PingStatistic)}.xml"));

			if (File.Exists(Path.Combine(Environment.CurrentDirectory, @$"{nameof(PingStatistic)}.txt")))
				File.Delete(Path.Combine(Environment.CurrentDirectory, @$"{nameof(PingStatistic)}.txt"));
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

					_statistics[testPing.IP].AvarageRoundTrip = (int)((_statistics[testPing.IP].MaximumRoundTrip + _statistics[testPing.IP].MinimumRoundTrip) / 2);
					_statistics[testPing.IP].Availability = (double)_statistics[testPing.IP].SuccessStatus / _statistics[testPing.IP].Sent * 100;
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
