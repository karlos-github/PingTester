namespace PingTester.ArgumentsService
{
	public static class ArgumentParser
	{
		readonly static string outputFlag = "--output";

		public static Setting Parse(string[] arguments)
		{
			if (arguments.Length == 0)
				throw new ArgumentNullException("Wrong arguments");

			if (arguments.Length < 2)
				throw new ArgumentOutOfRangeException("No ip address");

			if (!int.TryParse(arguments[0], out int number))
				throw new ArgumentNullException("Wrong argument for testing duration");

			var setting = new Setting();
			if (arguments.Any(arg => arg.Equals(outputFlag)))
			{
				setting.Duration = number * 1000;

				var index = arguments.Select((v, i) => new { v, i })
					.Where(x => x.v == outputFlag)
					.Select(x => x.i)
					.FirstOrDefault();

				if (index < arguments.Length)
				{
					setting.StatisticsOutput = Enum.Parse<StatisticsOutputType>(arguments[index + 1].ToLower().Trim());
					var hosts = new List<string>();
					for (int i = 0; i < arguments.Length; i++)
						if (i != 0 && i != index && i != index + 1)
							hosts.Add(arguments[i]);
					setting.Ips = hosts.ToArray();
				}

			}
			else setting = new Setting(number * 1000, arguments.Skip(1).ToArray());

			return setting;
		}
	}
}
