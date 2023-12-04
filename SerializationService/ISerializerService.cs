namespace PingTester.Serialization
{
	internal interface ISerializerService
	{
		Task<IEnumerable<TestPing>> DeserializeAsync();
		Task SerializeAsync(TestPing[] testPings);
	}
}