using PingTester.Serialization;
using System.Collections.Concurrent;
using System.Net.NetworkInformation;

namespace PingTester
{
    internal class PingTester : IPingTester
    {
        readonly ISerializerService _serializerService;
        ConcurrentQueue<TestPing> _pingQueue = new();
        const int PING_FREQUENCY = 100;
        const int DEFAULT_TIMEOUT = 300;//in ms
        const int ITEMS_SERIALIZE = 100;
        const int TIMEOUT_SERIALIZE = 3000;//in ms

        public PingTester(ISerializerService serializerService) => _serializerService = serializerService;

        async Task PingHostAsync(string host, CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var delayTask = Task.Delay(PING_FREQUENCY, CancellationToken.None);
                    using Ping pingSender = new();
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        PingReply reply = pingSender.Send(host, DEFAULT_TIMEOUT);
                        _pingQueue.Enqueue(new TestPing() { IP = host, Status = reply.Status, RoundtripTime = Convert.ToInt32(reply.RoundtripTime) });
                    }
                    await delayTask;
                };
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                Console.WriteLine($"{host} exception {ex.InnerException}.");
            }
        }

        async Task SavingItemsAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (_pingQueue.Count > ITEMS_SERIALIZE)
                    {
                        TestPing[] arrTestPing = new TestPing[ITEMS_SERIALIZE];
                        int counter = 0;
                        while (counter < ITEMS_SERIALIZE)
                        {
                            if (_pingQueue.TryDequeue(out var item))
                            {
                                arrTestPing[counter] = item;
                                counter++;
                            }
                            else
                                throw new InvalidOperationException("Error reading from Queue!");
                        }
                        await _serializerService.SerializeAsync(arrTestPing);
                    }
                    await Task.Delay(TIMEOUT_SERIALIZE, CancellationToken.None);
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                Console.WriteLine($"Exception {ex.InnerException}.");
            }
        }

        public async Task RunAsync(Setting setting)
        {
            var timeoutCts = new CancellationTokenSource();
            var timeoutTask = Task.Delay(TimeSpan.FromMilliseconds(setting.Duration), timeoutCts.Token);

            var hosts = setting.Ips.ToArray();
            Task[] tasks = new Task[hosts.Length + 2];

            for (int i = 0; i < hosts.Length; i++)
                tasks[i] = PingHostAsync(hosts[i], timeoutCts.Token);
            tasks[^1] = SavingItemsAsync(timeoutCts.Token);
            tasks[^2] = new ProgressBarUtility().WriteProgressAsync(timeoutCts.Token);

            var completedTask = await Task.WhenAny(Task.WhenAll(tasks), timeoutTask);

            if (completedTask == timeoutTask)
            {
                timeoutCts.Cancel();
            }
            else
            {
                try
                {
                    await completedTask;
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    Console.WriteLine($"An exception : {ex.Message}");
                }
            }

            await _serializerService.SerializeAsync(_pingQueue.ToArray());
            Console.Clear();
        }
    }
}
