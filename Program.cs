using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PingTester.Serialization;
using PingTester.Statistics;

namespace PingTester
{
	internal class Program
	{
		static ISerializerService _serializer;
		static IPingTester _pingTester;
		static IStatisticService _statisticService;

		public int[] _register = new int[10];

		static async Task Main(string[] args)
		{
			Init();

			#region T#1 input args
			//TODO- 0#1 additional options for output, serialization, (factory????)
			if (args.Length == 0)
				throw new ArgumentNullException("Wrong arguments");

			if (args.Length < 2)
				throw new ArgumentOutOfRangeException("No ip address");

			if (!int.TryParse(args[0], out int number))
				throw new ArgumentNullException("Wrong argument for testing time period");

			var setting = new Setting(number * 1000, args.Skip(1).ToArray());
			#endregion

			await _pingTester.Run(setting);

			_statisticService.OutputStatistics();

#if DEBUG
			Console.WriteLine($"Debug mode info :  ");
			Console.WriteLine($"TimePeriod={setting.Duration}");
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
					service.AddSingleton<ISerializerService, SerializerService>();
					service.AddSingleton<IPingTester, PingTester>();
					service.AddSingleton<IStatisticService, StatisticService>();
				}).Build();

			_serializer = _host.Services.GetRequiredService<ISerializerService>();
			_pingTester = _host.Services.GetRequiredService<IPingTester>();
			_statisticService = _host.Services.GetRequiredService<IStatisticService>();
		}
	}
}