using PingTester.Serialization;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace PingTester
{
	internal class Program
	{
		static ISerializer _serializer;
		static IPingTester _pingTester;

		public int[] _register = new int[10];

		static async Task Main(string[] args)
		{
			//await InitServiceAsync();
			Init();

			#region T#1 input args
			//TODO- 0#1 additional options for output, serialization, (factory????)
			if (args.Length == 0)
				throw new ArgumentNullException("Wrong arguments");

			if (args.Length < 2)
				throw new ArgumentOutOfRangeException("No ip address");

			if (!int.TryParse(args[0], out int number))
				throw new ArgumentNullException("Wrong argument for testing time period");

			var setting = new Setting(number, args.Skip(1));
			#endregion

			#region PingTesting

			await _pingTester.Run(setting.Ips);

			#endregion

#if DEBUG
			Console.WriteLine($"TimePeriod={setting.TimePeriod}");
			Console.WriteLine($"Ip addresses: ");
			foreach (var ip in setting.Ips)
				Console.WriteLine($"{ip}");
#endif
		}

		static void Init()
		{
			IHost _host = Host.CreateDefaultBuilder().ConfigureServices(
				service =>
				{
					service.AddSingleton<ISerializer, Serializer>();
					service.AddSingleton<IPingTester, PingTester>();
					//service.AddHostedService<MyHostedService>();
				}).Build();

			_serializer = _host.Services.GetRequiredService<ISerializer>();
			_pingTester = _host.Services.GetRequiredService<IPingTester>();
		}

		//static async Task InitServiceAsync()
		//{
		//	await new HostBuilder()
		//.ConfigureServices((hostContext, services) =>
		//{
		//	// Add the hosted service
		//	services.AddHostedService<MyHostedService>();
		//})
		//.RunConsoleAsync();
		//}
	}
}