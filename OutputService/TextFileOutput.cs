using PingTester.Serialization;
using PingTester.Statistics;

namespace PingTester.ArgumentService
{
	internal class TextFileOutput : IOutputStrategy
	{
		public void Output(IDictionary<string, PingStatistic> statistics)
		{
			try
			{
				if (File.Exists(Path.Combine(Environment.CurrentDirectory, @$"{nameof(PingStatistic)}.txt")))
					File.Delete(Path.Combine(Environment.CurrentDirectory, @$"{nameof(PingStatistic)}.txt"));

				Console.Clear();
				Console.WriteLine($@"Output to file : {Environment.CurrentDirectory}\{nameof(PingStatistic)}.txt");
				Console.WriteLine();

				using var sw = new StreamWriter(Path.Combine(Environment.CurrentDirectory, @$"{nameof(PingStatistic)}.txt"));
				foreach (var key in statistics.Keys)
				{
					if (statistics.TryGetValue(key, out var statistic))
					{
						sw.WriteLine($"Pinging [{key}] statistics:");
						sw.WriteLine($"Availability = {(double)statistic.SuccessStatus / statistic.Sent * 100:#.000} %, Sent = {statistic.Sent} packets");
						sw.WriteLine($"Approximate round trip times in milli - seconds:");
						sw.WriteLine($"Minimum = {statistic.MinimumRoundTrip}ms, Maximum = {statistic.MaximumRoundTrip}ms, Average = {statistic.AvarageRoundTrip}ms");
						sw.WriteLine();
					}
				}
			}
			catch (Exception) { throw; }
		}
	}
}
