namespace PingTester
{
	internal class ProgressBarUtility
    {
        const string _signs = "-\\|/";

        static void DoWriteProgress(int progress, bool update = false)
        {
            if (update)
                Console.Write("\b");
            Console.Write($"{_signs[progress % _signs.Length]}");
        }

        public static void WriteProgress(CancellationToken token)
        {
            Console.WriteLine("Process is running ..... ");
            while (!token.IsCancellationRequested)
            {
                DoWriteProgress(0);
                for (var i = 0; i <= 100; ++i)
                {
                    DoWriteProgress(i, true);
                    Thread.Sleep(50);
                }
                Console.Clear();
                Console.WriteLine("Process is running ..... ");
            }
        }


        public static async Task WriteProgressAsync(CancellationToken token)
        {
            Console.WriteLine("Process is running ..... ");
            while (!token.IsCancellationRequested)
            {
                var delayTask = Task.Delay(50);

                DoWriteProgress(0);
                for (var i = 0; i <= 100; ++i)
                {
                    DoWriteProgress(i, true);
                    await delayTask;
                }
                Console.Clear();
                Console.WriteLine("Process is running ..... ");
            }
        }
    }
}
