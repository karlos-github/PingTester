namespace PingTester.Statistics
{
	internal interface IStatisticService
	{
		Task OutputStatistics(StatisticsOutputType outputtype);
	}
}
