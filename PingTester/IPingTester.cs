namespace PingTester
{
    internal interface IPingTester
    {
        Task RunAsync(Setting setting);
    }
}
