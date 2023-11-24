using System.Xml;

namespace PingTester.Serialization
{
	internal class Serializer : ISerializer
	{
		public void Serialize(TestPing[] testPings)
		{
			try
			{
				using XmlWriter writer = XmlWriter.Create(@$"{nameof(TestPing)}.xml");
				writer.WriteStartElement($"{nameof(TestPing)}s");
				for (int i = 0; i < testPings.Length; i++)
				{
					writer.WriteStartElement($"{nameof(TestPing)}");
					writer.WriteAttributeString($"{nameof(TestPing.Guid)}", $"{testPings[i].Guid}");
					writer.WriteAttributeString($"{nameof(TestPing.IP)}", $"{testPings[i].IP}");
					writer.WriteAttributeString($"{nameof(TestPing.TimeStamp)}", $"{testPings[i].TimeStamp}");
					writer.WriteStartElement($"{nameof(TestPing.Status)}");
					writer.WriteString($"{testPings[i].Status}");
					writer.WriteEndElement();//STatus
					writer.WriteStartElement($"{nameof(TestPing.RoundtripTime)}");
					writer.WriteString($"{testPings[i].RoundtripTime}");
					writer.WriteEndElement();//RoundtripTime
					writer.WriteEndElement();//TestPing
				}
				writer.WriteEndElement();//TestPings
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

		#region Obsolate
		//public void CreateOutputFile(IEnumerable<TestPing> testPings)
		//{
		//	try
		//	{
		//		XmlDocument doc = new();
		//		XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
		//		XmlElement root = doc.DocumentElement;
		//		doc.InsertBefore(xmlDeclaration, root);
		//		XmlElement elTestPings = doc.CreateElement(string.Empty, $"{nameof(TestPing)}s", string.Empty);
		//		doc.AppendChild(elTestPings);
		//		foreach (var testPing in testPings)
		//		{
		//			XmlElement elTestPing = doc.CreateElement(string.Empty, $"{nameof(TestPing)}", string.Empty);
		//			elTestPings.AppendChild(elTestPing);

		//			XmlElement elIP = doc.CreateElement(string.Empty, $"{nameof(TestPing.IP)}", string.Empty);
		//			XmlText text1 = doc.CreateTextNode(testPing.IP);
		//			elIP.AppendChild(text1);
		//			elTestPing.AppendChild(elIP);

		//			XmlElement elTestingResults = doc.CreateElement(string.Empty, $"{nameof(TestingResult)}s", string.Empty);
		//			XmlText text2 = doc.CreateTextNode(string.Empty);
		//			elTestingResults.AppendChild(text2);
		//			elTestPing.AppendChild(elTestingResults);
		//		}
		//		doc.Save(@$"{nameof(TestPing)}s.xml");
		//	}
		//	catch (XmlException xmlEx) { throw new XmlException(xmlEx.Message, xmlEx.InnerException); }
		//	catch (Exception ex) { throw new Exception(ex.Message, ex.InnerException); }
		//}
		#endregion
	}
}
