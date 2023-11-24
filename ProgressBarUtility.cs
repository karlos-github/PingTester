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

		public static async Task WriteProgressAsync(CancellationToken token) => await Task.Run(() =>
																						 {
																							 while (!token.IsCancellationRequested)
																							 {
																								 Console.WriteLine("Process is running ..... ");
																								 DoWriteProgress(0);
																								 for (var i = 0; i <= 100; ++i)
																								 {
																									 DoWriteProgress(i, true);
																									 Thread.Sleep(50);
																								 }
																							 }
																						 });
	}
}
