namespace PingTester
{
	internal interface IPingTester
	{
		Task Run(IEnumerable<string> hosts);
	}
}
