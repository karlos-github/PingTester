using Microsoft.Extensions.Hosting;
using PingTester.Serialization;
using System.Collections.Concurrent;
using System.Net.NetworkInformation;

namespace PingTester
{
	internal class PingTester : IPingTester
	{
		ISerializerService _serializerService;
		ConcurrentQueue<TestPing> _pingQueue = new ConcurrentQueue<TestPing>();
		const int PING_FREQUENCY = 100;
		const int DEFAULT_TIMEOUT = 300;//in ms
		const int ITEMS_SERIALIZE = 200;
		const int TIMEOUT_SERIALIZE = 3000;//in ms

		public PingTester(ISerializerService serializerService)
		{
			_serializerService = serializerService;
		}

		async Task PingHostAsync(string host, CancellationToken cancellationToken)
		{
			try
			{
				while (!cancellationToken.IsCancellationRequested)
				{
					var delayTask = Task.Delay(PING_FREQUENCY, CancellationToken.None);
					using Ping pingSender = new();
					if (!cancellationToken.IsCancellationRequested)
					{
						PingReply reply = pingSender.Send(host, DEFAULT_TIMEOUT);
						_pingQueue.Enqueue(new TestPing() { IP = host, Status = reply.Status, RoundtripTime = Convert.ToInt32(reply.RoundtripTime) });
					}
					await delayTask;
				};
			}
			catch (Exception ex) when (ex is not OperationCanceledException)
			{
				Console.WriteLine($"{host} exception {ex.InnerException}.");
			}
		}

		async void SavingItems(CancellationToken cancellationToken)
		{
			try
			{
				while (!cancellationToken.IsCancellationRequested)
				{
					if (_pingQueue.Count > ITEMS_SERIALIZE)
					{
						TestPing[] arrTestPing = new TestPing[ITEMS_SERIALIZE];
						int counter = 0;
						while (counter < ITEMS_SERIALIZE)
						{
							if (_pingQueue.TryDequeue(out var item))
							{
								arrTestPing[counter] = item;
								counter++;
							}
						}
						_serializerService.Serialize(arrTestPing);
						//Thread.Sleep(50);
					}
					await Task.Delay(TIMEOUT_SERIALIZE, CancellationToken.None);
				}
			}
			catch (Exception ex) when (ex is not OperationCanceledException)
			{
				Console.WriteLine($"Exception {ex.InnerException}.");
			}
		}

		public async Task Run(Setting setting)
		{
			using CancellationTokenSource _cts = new(TimeSpan.FromMilliseconds(setting.Duration));
			CancellationToken cancellationToken = _cts.Token;
			var tasks = new List<Task>();
			foreach (var ip in setting.Ips)
			{
				tasks.Add(PingHostAsync(ip, cancellationToken));
			}
			tasks.Add(Task.Run(() => ProgressBarUtility.WriteProgress(cancellationToken)));
			tasks.Add(Task.Run(() => SavingItems(cancellationToken)));
			try
			{
				await Task.WhenAll(tasks.ToArray());

				_serializerService.Serialize(_pingQueue.ToArray());
			}
			catch (AggregateException ex)
			{
				foreach (Exception innerException in ex.InnerExceptions)
				{
					Console.WriteLine($"Task error: {innerException.Message}");
				}
			}
		}
	}
}
