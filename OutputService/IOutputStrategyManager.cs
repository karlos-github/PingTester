using PingTester.ArgumentService;

namespace PingTester.OutputService
{
	internal interface IOutputStrategyManager
	{
		IOutputStrategy CreateOutputStrategy(StatisticsOutputType outputType);
	}
}
