﻿using PingTester.OutputService;

namespace PingTester.ArgumentService
{
	internal class OutputStrategyManager : IOutputStrategyManager
	{
		public IOutputStrategy CreateOutputStrategy(StatisticsOutputType outputType) =>
					outputType switch
					{
						StatisticsOutputType.console => new ConsoleOutput(),
						StatisticsOutputType.textfile => new TextFileOutput(),
						StatisticsOutputType.xmlfile => new XmlFileOutput(),
						_ => throw new ArgumentException("Invalid output type"),
					};

	}
}
