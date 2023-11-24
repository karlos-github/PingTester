namespace PingTester.Serialization
{
	internal interface ISerializer
	{
		#region Obsolate
		//void CreateOutputFile(IEnumerable<TestPing> testPings);
		#endregion

		void Serialize(TestPing[] testPings);
	}
}
