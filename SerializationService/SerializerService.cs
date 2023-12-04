using System.Net.NetworkInformation;
using System.Xml;

namespace PingTester.Serialization
{
	internal class SerializerService : ISerializerService
	{
		public async Task<IEnumerable<TestPing>> DeserializeAsync()
		{
			const int MAX_ITEMS_READ = 3;
			var testPings = new List<TestPing>();
			//string[] nameTableItems = { $"{nameof(TestPing.RoundtripTime)}", $"{nameof(TestPing.Status)}", $"{nameof(TestPing)}" };
			XmlReaderSettings settings = new() { NameTable = new NameTable(), Async = true };
			try
			{
				var testPing = new TestPing();
				int counter = 0;
				using XmlReader reader = XmlReader.Create(@$"{nameof(TestPing)}.xml", settings);
				while (await reader.ReadAsync())
				{
					if (reader.NodeType != XmlNodeType.Element && reader.NodeType != XmlNodeType.Text) continue;

					if (reader.Name == nameof(TestPing))
					{
						reader.MoveToAttribute($"{nameof(TestPing.IP)}");
						testPing.IP = reader.GetAttribute($"{nameof(TestPing.IP)}")!;
						counter++;
					}
					else if (reader.Name == nameof(TestPing.Status))
					{
						testPing.Status = GetStatus(await reader.ReadElementContentAsStringAsync());
						counter++;
					}
					else if (reader.NodeType == XmlNodeType.Text)
					{
						testPing.RoundtripTime = GetRoundTrip(reader.Value);
						counter++;
					}

					if (counter == MAX_ITEMS_READ)
					{
						testPings.Add(testPing);
						testPing = new TestPing();
						counter = 0;
					}
				}
				return testPings;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.InnerException);
				throw;
			}
		}

		IPStatus GetStatus(string input) => Enum.TryParse(typeof(IPStatus), input, out var status) ? (IPStatus)status : IPStatus.Unknown;
		int GetRoundTrip(string input) => int.TryParse(input, out var roundTrip) ? roundTrip : 0;

		async Task DoSerializeAsync(TestPing[] testPings)
		{
			try
			{
				using XmlWriter writer = XmlWriter.Create(@$"{nameof(TestPing)}.xml", new XmlWriterSettings() { Async = true });
				await writer.WriteStartElementAsync(null, $"{nameof(TestPing)}s", null);
				for (int i = 0; i < testPings.Length; i++)
				{
					await writer.WriteStartElementAsync(null, $"{nameof(TestPing)}", null);
					await writer.WriteAttributeStringAsync(null, $"{nameof(TestPing.Guid)}", null, $"{testPings[i].Guid}");
					await writer.WriteAttributeStringAsync(null, $"{nameof(TestPing.IP)}", null, $"{testPings[i].IP}");
					await writer.WriteAttributeStringAsync(null, $"{nameof(TestPing.TimeStamp)}", null, $"{testPings[i].TimeStamp}");
					await writer.WriteElementStringAsync(null, nameof(TestPing.Status), null, $"{testPings[i].Status}");
					await writer.WriteElementStringAsync(null, $"{nameof(TestPing.RoundtripTime)}", null, $"{testPings[i].RoundtripTime}");
					await writer.WriteEndElementAsync();//TestPing
				}
				await writer.WriteEndElementAsync();//TestPings
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

		public async Task SerializeAsync(TestPing[] testPings)
		{
			try
			{
				if (File.Exists(Path.Combine(Environment.CurrentDirectory, @$"{nameof(TestPing)}.xml")))
				{
#if DEBUG
					File.Copy(
						Path.Combine(Environment.CurrentDirectory, @$"{nameof(TestPing)}.xml"), 
						Path.Combine(Environment.CurrentDirectory, @$"{nameof(TestPing)}_{DateTime.UtcNow.ToString("mm-ss-fff", System.Globalization.CultureInfo.InvariantCulture)}.xml")
						);
#endif
					File.Move(Path.Combine(Environment.CurrentDirectory, @$"{nameof(TestPing)}.xml"), Path.Combine(Environment.CurrentDirectory, @$"{nameof(TestPing)}_.xml"));

					await AppendAsync(testPings);

					if (File.Exists(Path.Combine(Environment.CurrentDirectory, @$"{nameof(TestPing)}_.xml")))
						File.Delete(Path.Combine(Environment.CurrentDirectory, @$"{nameof(TestPing)}_.xml"));
				}
				else await DoSerializeAsync(testPings);
			}
			catch (Exception ex) { Console.WriteLine(ex.InnerException); throw; }
		}

		async Task AppendAsync(TestPing[] testPings)
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
				using XmlWriter writer = XmlWriter.Create(Path.Combine(Environment.CurrentDirectory, @$"{nameof(TestPing)}.xml"), new XmlWriterSettings() { Async = true });
				using XmlReader reader = XmlReader.Create(Path.Combine(Environment.CurrentDirectory, @$"{nameof(TestPing)}_.xml"), new XmlReaderSettings() { NameTable = new NameTable(), Async = true });
				while (await reader.ReadAsync())
				{
					if (reader.NodeType == XmlNodeType.XmlDeclaration)
					{
						//Write declaration
						await writer.WriteStartDocumentAsync();
					}
					else if (reader.NodeType == XmlNodeType.Element)
					{
						if (reader.Name == $"{nameof(TestPing)}s")
						{
							//Write start TestPings element
							await writer.WriteStartElementAsync(null, $"{nameof(TestPing)}s", null);
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
							statusKey = await reader.ReadElementContentAsStringAsync();
							counter++;
						}
						if (reader.Name == $"{nameof(TestPing.RoundtripTime)}")
						{
							roundTripKey = reader.Value;
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
								await writer.WriteStartElementAsync(null, $"{nameof(TestPing)}", null);
								await writer.WriteAttributeStringAsync(null, $"{nameof(TestPing.Guid)}", null, $"{testPings[i].Guid}");
								await writer.WriteAttributeStringAsync(null, $"{nameof(TestPing.IP)}", null, $"{testPings[i].IP}");
								await writer.WriteAttributeStringAsync(null, $"{nameof(TestPing.TimeStamp)}", null, $"{testPings[i].TimeStamp}");
								await writer.WriteElementStringAsync(null, nameof(TestPing.Status), null, $"{testPings[i].Status}");
								await writer.WriteElementStringAsync(null, $"{nameof(TestPing.RoundtripTime)}", null, $"{testPings[i].RoundtripTime}");
								await writer.WriteEndElementAsync();//TestPing
							}
							//write ending element
							await writer.WriteEndElementAsync();//TestPings
						}
					}

					if (counter == MAX_COUNTER)
					{
						//writing the whole TestPing element just read from source
						await writer.WriteStartElementAsync(null, $"{nameof(TestPing)}", null);
						await writer.WriteAttributeStringAsync(null, $"{nameof(TestPing.Guid)}", null, GuidKey);
						await writer.WriteAttributeStringAsync(null, $"{nameof(TestPing.IP)}", null, ipKey);
						await writer.WriteAttributeStringAsync(null, $"{nameof(TestPing.TimeStamp)}", null, TimeStampKey);
						await writer.WriteElementStringAsync(null, nameof(TestPing.Status), null, statusKey);
						await writer.WriteElementStringAsync(null, $"{nameof(TestPing.RoundtripTime)}", null, roundTripKey);
						await writer.WriteEndElementAsync();//TestPing

						counter = 0;
						GuidKey = ipKey = TimeStampKey = roundTripKey = string.Empty;
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.InnerException);
				throw;
			}
		}
	}
}
