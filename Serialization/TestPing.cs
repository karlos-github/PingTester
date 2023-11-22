namespace PingTester.Serialization
{
	//T#4 --- structure to write to xml file

    /// <summary>
    /// Result of some pings called to donated IP address for the some period of time
    /// </summary>
	internal class TestPing
    {
        /// <summary>
        /// IP address or URL we try to ping
        /// </summary>
        public string IP { get; }
        /// <summary>
        /// Results of keeping calling ping to some IP
        /// </summary>
        public IEnumerable<TestingResult> TestingResults { get; set; }

        public TestPing(string ipAddress, IEnumerable<TestingResult> testingResults)
        {
            IP = ipAddress;
            TestingResults = testingResults;
        }
    }
}
