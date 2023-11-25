namespace PingTester.Serialization
{
	internal interface ISerializerService
	{
		#region Obsolate
		//void CreateOutputFile(IEnumerable<TestPing> testPings);
		#endregion

		void Serialize(TestPing[] testPings);
		void Deserialize(out List<TestPing> testPings);
	}
}
