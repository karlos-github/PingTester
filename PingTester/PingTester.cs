using PingTester.Serialization;
using System.Collections.Concurrent;
using System.Net.NetworkInformation;

namespace PingTester
{
    internal class PingTester : IPingTester
    {
        BlockingCollection<TestPing> _pings = new();

        async Task PingHostAsync(string host, CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var delayTask = Task.Delay(new Setting().PingFrequency);
                    using Ping pingSender = new();
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        PingReply reply = pingSender.Send(host, new Setting().DefaultTimeOut);
                        //var testPing = new TestPing() { IP = host, Status = reply.Status, RoundtripTime = Convert.ToInt16(reply.RoundtripTime) };
                        _pings.Add(new TestPing() { IP = host, Status = reply.Status, RoundtripTime = Convert.ToInt16(reply.RoundtripTime) } /*testPing*/);
                        //Console.WriteLine($"{host} {testPing}");
                    }
                    await delayTask;
                };
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine($"{host} exception {ex.InnerException}.");
            }
        }

        public async Task Run(Setting setting)
        {
            CancellationTokenSource _cts = new();
            CancellationToken cancellationToken = _cts.Token;
            var tasks = new List<Task>();
            for (int i = 0; i < setting.Ips.Length; i++)
            {
                tasks.Add(PingHostAsync(setting.Ips[i], cancellationToken));
            }
            tasks.Add(Task.Run(() => ProgressBarUtility.WriteProgress(cancellationToken)));
            try
            {
                Task.WhenAll(tasks.ToArray()).Wait(setting.Duration, cancellationToken);

                _pings.CompleteAdding();
                new SerializerService().Serialize(_pings.GetConsumingEnumerable().ToArray());
            }
            catch (AggregateException ex)
            {
                foreach (Exception innerException in ex.InnerExceptions)
                {
                    Console.WriteLine($"Task error: {innerException.Message}");
                }
            }
            finally
            {
                _cts.Dispose();
            }
        }
    }
}
