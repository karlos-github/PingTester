namespace PingTester.Statistics
{
	internal interface IStatisticService
	{
		Task OutputStatisticsAsync(StatisticsOutputType outputtype);
	}
}
