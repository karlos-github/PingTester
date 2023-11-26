namespace PingTester
{
	public class Setting
	{
		public int Duration { get; set; }
		public string[] Ips { get; set; }
		public int DefaultTimeOut { get; set; } = 300;
		public int PingFrequency { get; set; } = 100;
		public StatisticsOutputType StatisticsOutput { get; set; } = StatisticsOutputType.console;

		public Setting()
		{

		}

		public Setting(int timePeriod, string[] ips)
		{
			Duration = timePeriod;
			Ips = ips;
		}

		public Setting(int timePeriod, string[] ips, int defaultTimeOut, int pingFrequency)
		{
			Duration = timePeriod;
			Ips = ips;
			DefaultTimeOut = defaultTimeOut;
			PingFrequency = pingFrequency;
		}
	}

	public enum StatisticsOutputType
	{
		console,
		textfile,
		xmlfile
	}
}
