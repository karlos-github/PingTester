namespace PingTester.ArgumentsService
{
	public static class ArgumentParser
	{
		const string outputFlag = "--output";

		public static Setting Parse(string[] arguments)
		{
			if (arguments.Length == 0)
				throw new ArgumentNullException("Wrong arguments : \nRight arguments are : 60 seznam.cz --output console\nPlease check documentation (section Command Line Arguments)!");

			if (!int.TryParse(arguments[0], out int number))
				throw new ArgumentNullException("Wrong argument for testing duration");

			var setting = new Setting();
			setting.Duration = number * 1000;
			var cdx = Array.IndexOf(arguments, outputFlag);
			if (cdx < 0)
				cdx = arguments.Length;
			else if (cdx < arguments.Length - 1)
				setting.StatisticsOutput = Enum.TryParse<StatisticsOutputType>(arguments[cdx + 1].Trim(), true, out var outputType) ? outputType : StatisticsOutputType.console;
			setting.Ips = cdx == 1 ? arguments[3..] : arguments[1..cdx];

			return setting;
		}
	}
}
