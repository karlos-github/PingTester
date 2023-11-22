namespace PingTester
{
	public class Setting
	{
		public int TimePeriod { get; }
		public IEnumerable<string> Ips { get; }

		public Setting(int timePeriod, IEnumerable<string> ips)
		{
			TimePeriod = timePeriod;
			Ips = ips;
		}
	}
}
