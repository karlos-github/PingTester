using PingTester.Serialization;
using System.Collections.Concurrent;
using System.Net.NetworkInformation;

namespace PingTester
{
	internal class PingTester : IPingTester
	{
		readonly CancellationTokenSource _cts = new();
		BlockingCollection<TestPing> _pings = new();

		public async Task DoSomethingEveryTenSeconds(string host)
		{
			BlockingCollection<TestPing> backUp = new BlockingCollection<TestPing>();
			while (!_cts.Token.IsCancellationRequested)
			{
				var delayTask = Task.Delay(1000);
				PingHostAsync(host, backUp);
				if (backUp.Count > 50)
				{
					foreach (var item in backUp.GetConsumingEnumerable().Take(50))
					{
						_pings.Add(item);
					}
				}
				await delayTask;
			}

			backUp.CompleteAdding();
			foreach (var item in backUp.GetConsumingEnumerable())
			{
				_pings.Add(item);
			}
		}

		void PingHostAsync(string nameOrAddress, BlockingCollection<TestPing> backUp)
		{
			while (!_cts.Token.IsCancellationRequested)
			{
				using Ping pingSender = new();
				PingReply reply = pingSender.Send(nameOrAddress, 300);
				backUp.Add(new TestPing() { IP = nameOrAddress,  Status = reply.Status, RoundtripTime =  reply.RoundtripTime });
			};
		}

		public async Task Run(IEnumerable<string> hosts)
		{
			List<Task> tasks = new();
			try
			{
				if (hosts.Count() > 1)
				{
					_cts.CancelAfter(/*3500*/6000);
					hosts.ToList().ForEach(host => tasks.Add(Task.Run(() => DoSomethingEveryTenSeconds(host), _cts.Token)));
					await Task.WhenAll(tasks);
				}
				else
				{
					await Task.Run(() => DoSomethingEveryTenSeconds(hosts.FirstOrDefault()), _cts.Token);
				}

				new Serializer().Serialize(_pings.ToArray());
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
