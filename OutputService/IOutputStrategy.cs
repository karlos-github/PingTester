using PingTester.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingTester.ArgumentService
{
	internal interface IOutputStrategy
	{
		void Output(IDictionary<string, PingStatistic> statistics);
	}
}
