using System.Security.Cryptography;
using System.Text;

namespace Shared
{
    public class Test
    {

        private static CancellationTokenSource token => new CancellationTokenSource();

        public static void Cancel(object? sender, ConsoleCancelEventArgs e)
        {
            token.Cancel();
        }

        public static async Task Run()
        {
            Console.CancelKeyPress += Cancel;

            const int Threads = 20;

            var completed = 0;

            var saltBytes = Convert.FromBase64String("KMUNCBwjf5FXsYQepyktLA==");
            var passwd = "SomePassword".Normalize(NormalizationForm.FormKC);
            var dataBytes = "Client Key"u8.ToArray();

            var tasks = new List<Task>(Threads);
            for (var i = 0; i < Threads; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        var saltedPassword = Rfc2898DeriveBytes.Pbkdf2(passwd, saltBytes, 4096, HashAlgorithmName.SHA256, 256 / 8);
                        HMACSHA256.HashData(saltedPassword, dataBytes);
                        Interlocked.Increment(ref completed);
                    }
                }, TaskCreationOptions.LongRunning));
            }

            _ = Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    await Task.Delay(1000);
                    var current = Interlocked.Exchange(ref completed, 0);
                    Console.WriteLine($"Completed: {current}/s");
                }
            }, token.Token);

            await Task.WhenAll(tasks);

        }
    }
}
