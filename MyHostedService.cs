using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PingTester
{
	public class MyHostedService : IHostedService, IDisposable
	{
		private Timer _timer;
		static IPingTester _pingTester;

		public Task StartAsync(CancellationToken cancellationToken)
		{
			Console.WriteLine("MyHostedService is starting.");

			// Set up a timer to run the DoWork method every 2 seconds
			_timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(2));

			return Task.CompletedTask;
		}

		private void DoWork(object state)
		{
			new PingTester().Run(new List<string>() { "seznam.cz", "google.com"});

			Console.WriteLine($"Doing some work at {DateTime.Now}");
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			Console.WriteLine("MyHostedService is stopping.");

			// Stop the timer
			_timer?.Change(Timeout.Infinite, 0);

			return Task.CompletedTask;
		}

		public void Dispose()
		{
			// Clean up resources, if needed
			_timer?.Dispose();
		}
	}
}
