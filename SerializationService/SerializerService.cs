using System.Net.NetworkInformation;
using System.Xml;

namespace PingTester.Serialization
{
	internal class SerializerService : ISerializerService
	{
		public void Deserialize(out List<TestPing> testPings)
		{
			testPings = new List<TestPing>();
			string[] nameTableItems = { $"{nameof(TestPing.RoundtripTime)}", $"{nameof(TestPing.Status)}", $"{nameof(TestPing)}" };
			XmlReaderSettings settings = new() { NameTable = new NameTable() };
			try
			{
				var testPing = new TestPing();
				int counter = 0;
				using XmlReader reader = XmlReader.Create(@$"{nameof(TestPing)}.xml", settings);
				while (reader.Read())
				{
					if (reader.NodeType != XmlNodeType.Element) continue;
					if (reader.Name == nameTableItems[0])
					{
						testPing.RoundtripTime = Convert.ToInt16(reader.ReadString());
						counter++;
					}
					else if (reader.Name == nameTableItems[1])
					{
						testPing.Status = (IPStatus)Enum.Parse(typeof(IPStatus), reader.ReadString());
						counter++;
					}
					else if (reader.Name == nameTableItems[2])
					{
						reader.MoveToAttribute($"{nameof(TestPing.IP)}");
						testPing.IP = reader.GetAttribute($"{nameof(TestPing.IP)}")!;
						counter++;
					}

					if (counter == nameTableItems.Length)
					{
						testPings.Add(testPing);
						testPing = new TestPing();
						counter = 0;
					}
				}
				reader.Close();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.InnerException);
				throw;
			}
		}

		void DoSerialize(TestPing[] testPings)
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

		public void Serialize(TestPing[] testPings)
		{
			try
			{
				if (File.Exists(Path.Combine(Environment.CurrentDirectory, @$"{nameof(TestPing)}.xml")))
				{
//#if DEBUG
//					File.Copy(Path.Combine(Environment.CurrentDirectory, @$"{nameof(TestPing)}.xml"), Path.Combine(Environment.CurrentDirectory, @$"{nameof(TestPing)}_{DateTime.UtcNow.ToString("mm-ss-fff", System.Globalization.CultureInfo.InvariantCulture)}.xml"));
//#endif
					File.Move(Path.Combine(Environment.CurrentDirectory, @$"{nameof(TestPing)}.xml"), Path.Combine(Environment.CurrentDirectory, @$"{nameof(TestPing)}_.xml"));

					Append(testPings);

					if (File.Exists(Path.Combine(Environment.CurrentDirectory, @$"{nameof(TestPing)}_.xml")))
						File.Delete(Path.Combine(Environment.CurrentDirectory, @$"{nameof(TestPing)}_.xml"));
				}
				else DoSerialize(testPings);
			}
			catch (Exception) { throw; }
		}

		void Append(TestPing[] testPings)
		{
			try
			{
				int counter = 0;
				const int MAX_COUNTER = 5; //each ping test holds 5 blocks of data ..... guid,ip,timeStamp,status,roundTrip
				string GuidKey = string.Empty;
				string ipKey = string.Empty;
				string TimeStampKey = string.Empty;
				string statusKey = string.Empty;
				string roundTripKey = string.Empty;
				using XmlWriter writer = XmlWriter.Create(Path.Combine(Environment.CurrentDirectory, @$"{nameof(TestPing)}.xml"));
				using XmlReader reader = XmlReader.Create(Path.Combine(Environment.CurrentDirectory, @$"{nameof(TestPing)}_.xml"), new XmlReaderSettings() { NameTable = new NameTable() });
				while (reader.Read())
				{
					if (reader.NodeType == XmlNodeType.XmlDeclaration)
					{
						//Write declaration
						writer.WriteStartDocument();
					}
					else if (reader.NodeType == XmlNodeType.Element)
					{
						if (reader.Name == $"{nameof(TestPing)}s")
						{
							//Write start TestPings element
							writer.WriteStartElement($"{nameof(TestPing)}s");
						}
						if (reader.Name == $"{nameof(TestPing)}")
						{
							GuidKey = reader.GetAttribute($"{nameof(TestPing.Guid)}")!;
							ipKey = reader.GetAttribute($"{nameof(TestPing.IP)}")!;
							TimeStampKey = reader.GetAttribute($"{nameof(TestPing.TimeStamp)}")!;
							counter += 3;
						}
						if (reader.Name == $"{nameof(TestPing.Status)}")
						{
							statusKey = reader.ReadString();
							counter++;
						}
						if (reader.Name == $"{nameof(TestPing.RoundtripTime)}")
						{
							roundTripKey = reader.ReadString();
							counter++;
						}
					}
					else if (reader.NodeType == XmlNodeType.EndElement)
					{
						if (reader.Name == $"{nameof(TestPing)}s")
						{
							//write a new content
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
							//write ending element
							writer.WriteEndElement();//TestPings
						}
					}

					if (counter == MAX_COUNTER)
					{
						//writing the whole TestPing element just read from source
						writer.WriteStartElement($"{nameof(TestPing)}");
						writer.WriteAttributeString($"{nameof(TestPing.Guid)}", GuidKey);
						writer.WriteAttributeString($"{nameof(TestPing.IP)}", ipKey);
						writer.WriteAttributeString($"{nameof(TestPing.TimeStamp)}", TimeStampKey);
						writer.WriteStartElement($"{nameof(TestPing.Status)}");
						writer.WriteString(statusKey);
						writer.WriteEndElement();//STatus
						writer.WriteStartElement($"{nameof(TestPing.RoundtripTime)}");
						writer.WriteString(roundTripKey);
						writer.WriteEndElement();//RoundtripTime
						writer.WriteEndElement();//TestPing

						counter = 0;
						GuidKey = ipKey = TimeStampKey = roundTripKey = string.Empty;
					}
				}
				reader.Close();
				writer.Close();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.InnerException);
				throw;
			}
		}
	}
}
