namespace PingTester.Serialization
{
	internal interface ISerializerService
	{
		void Deserialize(out List<TestPing> testPings);
		void Serialize(TestPing[] testPings);
	}
}
