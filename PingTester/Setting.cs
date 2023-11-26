namespace PingTester
{
    public class Setting
    {
        public int Duration { get; }
        public string[] Ips { get; }
        public int DefaultTimeOut { get; } = 300;
        public int PingFrequency { get; } = 100;

        public Setting()
        {

        }

        public Setting(int timePeriod, string[] ips)
        {
            Duration = timePeriod;
            Ips = ips;
        }

        public Setting(int timePeriod, string[] ips, int defaultTimeOut, int pingFrequency)
        {
            Duration = timePeriod;
            Ips = ips;
            DefaultTimeOut = defaultTimeOut;
            PingFrequency = pingFrequency;
        }
    }
}
