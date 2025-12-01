using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwaitRealWorldFeatures
{
    class Program
    {
        private static readonly HttpClient httpClient = new HttpClient();

        static async Task Main()
        {
            Console.WriteLine("=== Real-world async/await demo ===\n");

            var cts = new CancellationTokenSource();
            var progress = new Progress<int>(percent =>
                Console.WriteLine($"[Progress] {percent}% completed"));

            // Start a cancellable operation with progress reporting
            Console.WriteLine("Starting 5 website downloads in parallel...");
            Console.WriteLine("Press 'c' + Enter to cancel\n");

            // Background task to listen for cancel input
            _ = Task.Run(() =>
            {
                Console.WriteLine("Type 'c' and press Enter to cancel...");
                if (Console.ReadLine()?.Trim().ToLower() == "c")
                    cts.Cancel();
            });

            try
            {
                string[] results = await DownloadMultipleSitesAsync(
                    [
                        "https://dotnet.microsoft.com",
                        "https://github.com",
                        "https://stackoverflow.com",
                        "https://news.ycombinator.com",
                        "https://reddit.com"
                    ],
                    progress,
                    cts.Token);

                Console.WriteLine($"\nAll done! Downloaded {results.Length} pages:");
                foreach (var item in results)
                {
                    var parts = item.Split('|');
                    string url = parts[0];
                    int bytes = int.Parse(parts[1]);
                    Console.WriteLine($"  {url} → {bytes / 1024} KB");
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("\nOperation was cancelled by user!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.GetBaseException().Message}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static async Task<string[]> DownloadMultipleSitesAsync(
            string[] urls,
            IProgress<int>? progress = null,
            CancellationToken ct = default)
        {
            var tasks = new Task<string>[urls.Length];

            for (int i = 0; i < urls.Length; i++)
            {
                int index = i;
                tasks[index] = DownloadSiteAsync(urls[index], ct);
            }

            // This is the magic: run ALL downloads in parallel!
            string[] results = await Task.WhenAll(tasks).ConfigureAwait(false);

            // Report final progress
            progress?.Report(100);
            return results;
        }

        static async Task<string> DownloadSiteAsync(string url, CancellationToken ct)
        {
            Console.WriteLine($"[Task {Task.CurrentId}] Downloading {url}");

            using var response = await httpClient.GetAsync(url, ct)
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync(ct)
                .ConfigureAwait(false);

            Console.WriteLine($"[Task {Task.CurrentId}] Completed {url} ({content.Length / 1024} KB)");

            Console.WriteLine($"[{Task.CurrentId}] Finished: {url} ({content.Length / 1024} KB)");

            // Simulate progress reporting (in real app you'd do this incrementally)
            // Here we just fake it after completion
            return $"{url}|{content.Length}";
        }
    }
}
