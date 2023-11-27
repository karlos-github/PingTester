using PingTester.Serialization;
using PingTester.Statistics;
using System.Xml;

namespace PingTester.ArgumentService
{
	internal class XmlFileOutput : IOutputStrategy
	{
		public void Output(IDictionary<string, PingStatistic> statistics)
		{
			try
			{
				if (File.Exists(Path.Combine(Environment.CurrentDirectory, @$"{nameof(PingStatistic)}.xml")))
					File.Delete(Path.Combine(Environment.CurrentDirectory, @$"{nameof(PingStatistic)}.xml"));

				Console.Clear();
				Console.WriteLine($@"Output to file : {Environment.CurrentDirectory}\{nameof(PingStatistic)}.xml");
				Console.WriteLine();

				using XmlWriter writer = XmlWriter.Create(@$"{nameof(PingStatistic)}.xml");
				writer.WriteStartElement($"{nameof(PingStatistic)}s");
				foreach (var key in statistics.Keys)
				{
					var statistic = statistics[key];
					writer.WriteStartElement($"{nameof(PingStatistic)}");
					writer.WriteAttributeString($"IP", $"{key}");
					writer.WriteStartElement($"{nameof(PingStatistic.Availability)}");
					writer.WriteString($"{statistic.Availability}");
					writer.WriteEndElement();//Availability
					writer.WriteStartElement($"{nameof(PingStatistic.Sent)}");
					writer.WriteString($"{statistic.Sent}");
					writer.WriteEndElement();//Sent
					writer.WriteStartElement($"{nameof(PingStatistic.MinimumRoundTrip)}");
					writer.WriteString($"{statistic.MinimumRoundTrip}");
					writer.WriteEndElement();//MinimumRoundTrip
					writer.WriteStartElement($"{nameof(PingStatistic.MaximumRoundTrip)}");
					writer.WriteString($"{statistic.MaximumRoundTrip}");
					writer.WriteEndElement();//MaximumRoundTrip
					writer.WriteStartElement($"{nameof(PingStatistic.AvarageRoundTrip)}");
					writer.WriteString($"{statistic.AvarageRoundTrip}");
					writer.WriteEndElement();//AvarageRoundTrip
					writer.WriteEndElement();//PingStatistic
				}
				writer.WriteEndElement();//PingStatistics
				writer.Close();
			}
			catch (XmlException xmlEx)
			{
				throw new XmlException(xmlEx.Message, xmlEx.InnerException);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message, ex.InnerException);
			}
		}
	}
}
