using PingTester.Serialization;
using System.Collections.Concurrent;
using System.Net.NetworkInformation;

namespace PingTester
{
	internal class PingTester : IPingTester
	{
		ConcurrentQueue<TestPing> _pingQueue = new ConcurrentQueue<TestPing>();

		async Task PingHostAsync(string host, CancellationToken cancellationToken)
		{
			try
			{
				while (!cancellationToken.IsCancellationRequested)
				{
					var delayTask = Task.Delay(new Setting().PingFrequency);
					using Ping pingSender = new();
					if (!cancellationToken.IsCancellationRequested)
					{
						PingReply reply = pingSender.Send(host, new Setting().DefaultTimeOut);
						_pingQueue.Enqueue(new TestPing() { IP = host, Status = reply.Status, RoundtripTime = Convert.ToInt16(reply.RoundtripTime) });
					}
					await delayTask;
				};
			}
			catch (OperationCanceledException ex)
			{
				Console.WriteLine($"{host} exception {ex.InnerException}.");
			}
		}

		void SavingItems(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				if (_pingQueue.Count > 100)
				{
					TestPing[] arrTestPing = new TestPing[100];
					int counter = 0;
					while (counter < 100)
					{
						if (_pingQueue.TryDequeue(out var item))
						{
							arrTestPing[counter] = item;
							counter++;
						}
					}
					new SerializerService().Serialize(arrTestPing);
					Thread.Sleep(50);
				}
			}
		}

		public async Task Run(Setting setting)
		{
			CancellationTokenSource _cts = new();
			CancellationToken cancellationToken = _cts.Token;
			var tasks = new List<Task>();
			for (int i = 0; i < setting.Ips.Length; i++)
			{
				tasks.Add(PingHostAsync(setting.Ips[i], cancellationToken));
			}
			tasks.Add(Task.Run(() => ProgressBarUtility.WriteProgress(cancellationToken)));
			tasks.Add(Task.Run(() => SavingItems(cancellationToken)));
			try
			{
				Task.WhenAll(tasks.ToArray()).Wait(setting.Duration, cancellationToken);

				new SerializerService().Serialize(_pingQueue.ToArray());
			}
			catch (AggregateException ex)
			{
				foreach (Exception innerException in ex.InnerExceptions)
				{
					Console.WriteLine($"Task error: {innerException.Message}");
				}
			}
			finally
			{
				_cts.Dispose();
			}
		}
	}
}
