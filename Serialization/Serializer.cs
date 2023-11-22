using System.Xml;

namespace PingTester.Serialization
{
	internal class Serializer : ISerializer
	{
		async Task TestWriter(Stream stream)
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Async = true;

			using (XmlWriter writer = XmlWriter.Create(stream, settings))
			{
				await writer.WriteStartElementAsync("pf", "root", "http://ns");
				await writer.WriteStartElementAsync(null, "sub", null);
				await writer.WriteAttributeStringAsync(null, "att", null, "val");
				await writer.WriteStringAsync("text");
				await writer.WriteEndElementAsync();
				await writer.WriteProcessingInstructionAsync("pName", "pValue");
				await writer.WriteCommentAsync("cValue");
				await writer.WriteCDataAsync("cdata value");
				await writer.WriteEndElementAsync();
				await writer.FlushAsync();
			}
		}

		public void CreateOutputFile(IEnumerable<TestPing> testPings)
		{
			try
			{
				XmlDocument doc = new();
				XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
				XmlElement root = doc.DocumentElement;
				doc.InsertBefore(xmlDeclaration, root);
				XmlElement elTestPings = doc.CreateElement(string.Empty, $"{nameof(TestPing)}s", string.Empty);
				doc.AppendChild(elTestPings);
				foreach (var testPing in testPings)
				{
					XmlElement elTestPing = doc.CreateElement(string.Empty, $"{nameof(TestPing)}", string.Empty);
					elTestPings.AppendChild(elTestPing);

					XmlElement elIP = doc.CreateElement(string.Empty, $"{nameof(TestPing.IP)}", string.Empty);
					XmlText text1 = doc.CreateTextNode(testPing.IP);
					elIP.AppendChild(text1);
					elTestPing.AppendChild(elIP);

					XmlElement elTestingResults = doc.CreateElement(string.Empty, $"{nameof(TestingResult)}s", string.Empty);
					XmlText text2 = doc.CreateTextNode(string.Empty);
					elTestingResults.AppendChild(text2);
					elTestPing.AppendChild(elTestingResults);
				}
				doc.Save(@$"{nameof(TestPing)}s.xml");
			}
			catch (XmlException xmlEx) { throw new XmlException(xmlEx.Message, xmlEx.InnerException); }
			catch (Exception ex) { throw new Exception(ex.Message, ex.InnerException); }
		}
	}
}
