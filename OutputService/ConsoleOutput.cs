using PingTester.Statistics;

namespace PingTester.ArgumentService
{
	internal class ConsoleOutput : IOutputStrategy
	{
		public void Output(IDictionary<string, PingStatistic> statistics)
		{
			Console.Clear();
			foreach (var key in statistics.Keys)
			{
				if (statistics.TryGetValue(key, out var statistic))
				{
					Console.WriteLine($"Pinging [{key}] statistics:");
					Console.WriteLine($"Availability = {statistic.Availability:#.000} %, Sent = {statistic.Sent} packets");
					Console.WriteLine($"Approximate round trip times in milli - seconds:");
					Console.WriteLine($"Minimum = {statistic.MinimumRoundTrip}ms, Maximum = {statistic.MaximumRoundTrip}ms, Average = {statistic.AvarageRoundTrip}ms");
					Console.WriteLine();
				}
			}
		}
	}
}
