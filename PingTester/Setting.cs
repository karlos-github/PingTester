namespace PingTester
{
	public class Setting
	{
		public int Duration { get; set; }
		public IEnumerable<string> Ips { get; set; } = new List<string>();
		public StatisticsOutputType StatisticsOutput { get; set; } = StatisticsOutputType.console;
	}
}
