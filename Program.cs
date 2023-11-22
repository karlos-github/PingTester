using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace PingTester
{
	internal class Program
	{
		static void Main(string[] args)
		{
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

#if DEBUG
			Console.WriteLine($"TimePeriod={setting.TimePeriod}");
			Console.WriteLine($"Ip addresses: ");
			foreach (var ip in setting.Ips)
				Console.WriteLine($"{ip}");
#endif
		}
	}
}