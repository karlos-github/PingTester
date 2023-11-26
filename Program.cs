using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PingTester.ArgumentService;
using PingTester.ArgumentsService;
using PingTester.OutputService;
using PingTester.Serialization;
using PingTester.Statistics;

namespace PingTester
{
	internal class Program
	{
		static ISerializerService _serializer;
		static IPingTester _pingTester;
		static IStatisticService _statisticService;
		static IOutputStrategyManager _outputStrategyManager;

		public int[] _register = new int[10];

		static async Task Main(string[] args)
		{
			Init();

			var setting = ArgumentParser.Parse(args);

			//Trying to delete serialized data from any previous run
			if (File.Exists(Path.Combine(Environment.CurrentDirectory, @$"{nameof(TestPing)}.xml")))
				File.Delete(Path.Combine(Environment.CurrentDirectory, @$"{nameof(TestPing)}.xml"));

			await _pingTester.Run(setting);

			_statisticService.OutputStatistics(setting.StatisticsOutput);

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
					service.AddSingleton<IOutputStrategyManager, OutputStrategyManager>();
				}).Build();

			_serializer = _host.Services.GetRequiredService<ISerializerService>();
			_pingTester = _host.Services.GetRequiredService<IPingTester>();
			_statisticService = _host.Services.GetRequiredService<IStatisticService>();
			_outputStrategyManager = _host.Services.GetRequiredService<IOutputStrategyManager>();
		}
	}
}