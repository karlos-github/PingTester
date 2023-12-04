namespace PingTester
{
	internal class ProgressBarUtility
	{
		public async Task WriteProgressAsync(CancellationToken token)
		{
			const int MAX_PRINTED_DOTS = 3;
			const int ONE_SECOND = 1000;
			int printedDots = 1;
			Console.Write("Please Wait");
			while (!token.IsCancellationRequested)
			{
				Console.Write(".");
				await Task.Delay(ONE_SECOND, token);
				if (printedDots % MAX_PRINTED_DOTS == 0)
				{
					for (int i = 0; i < printedDots; i++)
						Console.Write("\b \b");
					printedDots = 0;
					await Task.Delay(ONE_SECOND, token);
				}
				printedDots++;
			}
		}
	}
}
