namespace PingTester.Statistics
{
	public class PingStatistic
	{
		public double Availability { get; set; }
		public int Sent { get; set; }
		public int SuccessStatus { get; set; }
		public int MinimumRoundTrip { get; set; }
		public int MaximumRoundTrip { get; set; }
		public int AvarageRoundTrip { get; set; }
	}
}
