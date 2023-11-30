namespace PingTester.Serialization
{
	internal interface ISerializerService
	{
		Task<IEnumerable<TestPing>> Deserialize();
		Task Serialize(TestPing[] testPings);
	}
}