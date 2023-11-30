namespace PingTester
{
	internal class ProgressBarUtility
	{
		const string _signs = @"-|/";

		static void DoWriteProgress(int progress, bool update = false)
		{
			if (update)
				Console.Write("\b");
			Console.Write($"{_signs[progress % _signs.Length]}");
		}

		public static async Task WriteProgress(CancellationToken token)
		{
			Console.WriteLine("Process is running ..... ");
			while (!token.IsCancellationRequested)
			{
				try
				{
					var delayTask = Task.Delay(50, token);
					DoWriteProgress(0);
					for (var i = 0; i <= 100 && !token.IsCancellationRequested; ++i)
					{
						DoWriteProgress(i, true);
						await delayTask;
					}
				}
				catch (Exception ex) when (ex is not OperationCanceledException)
				{

					throw;
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
				var delayTask = Task.Delay(50, token);

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
