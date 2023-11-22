using PingTester.Serialization;
using System.Collections.Concurrent;
using System.Net.NetworkInformation;

namespace PingTester
{
	internal class PingTester
	{
		static readonly CancellationTokenSource _cts = new();
		static readonly ConcurrentDictionary<string, List<TestingResult>> _testingResults = new();

		public static async Task DoSomethingEveryTenSeconds(string host)
		{
			while (true && !_cts.Token.IsCancellationRequested)
			{
				var delayTask = Task.Delay(1000);
				PingHostAsync(host);
				await delayTask;
			}
		}

		static void PingHostAsync(string nameOrAddress)
		{
			while (!_cts.Token.IsCancellationRequested)
			{
				using Ping pingSender = new();
				PingReply reply = pingSender.Send(nameOrAddress, 300);
				if (reply is { Status: IPStatus.Success })
				{
					Console.WriteLine($"_______________________________running {DateTime.Now.Second}_________________________.");
					Console.WriteLine($"\n{_cts.Token.IsCancellationRequested}\n");
					Console.WriteLine("www: {0}", nameOrAddress);
					Console.WriteLine("Status: {0}", reply.Status);
					Console.WriteLine("Address: {0}", reply.Address.ToString());
					Console.WriteLine("RoundTrip time: {0}", reply.RoundtripTime);

					if (_testingResults.ContainsKey(nameOrAddress))
					{
						if (_testingResults.TryGetValue(nameOrAddress, out var value))
							value.Add(new TestingResult(reply.Status, reply.RoundtripTime));
					}
					else _testingResults.TryAdd(nameOrAddress, new List<TestingResult>() { new TestingResult(reply.Status, reply.RoundtripTime) });
				}
			};
		}

		public static async Task Run(IEnumerable<string> hosts)
		{
			List<Task> tasks = new();
			try
			{
				_cts.CancelAfter(/*3500*/6000);
				hosts.ToList().ForEach(host => tasks.Add(Task.Run(() => DoSomethingEveryTenSeconds(host), _cts.Token)));
				await Task.WhenAll(tasks);
			}
			catch (Exception)
			{
				foreach (var task in tasks.Where(x => x.IsFaulted))
					foreach (var exception in task.Exception!.InnerExceptions)
						Console.WriteLine($"\nT{exception.InnerException}\n");
			}
			finally
			{
				Console.WriteLine($"\n{_cts.Token.IsCancellationRequested}\n");
				_cts.Dispose();
			}
		}
	}
}
