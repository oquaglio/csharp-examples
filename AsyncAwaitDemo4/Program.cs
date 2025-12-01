using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwaitRealWorldFeatures
{
    class Program
    {
        // Fix 403s: add a realistic User-Agent
        private static readonly HttpClient httpClient = new HttpClient(new HttpClientHandler
        {
            // Optional: some sites check this too
            AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
        });

        static Program()
        {
            httpClient.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/130.0.0.0 Safari/537.36");
        }

        static async Task Main()
        {
            Console.WriteLine("=== Real-world async/await demo (works everywhere) ===\n");

            var cts = new CancellationTokenSource();
            var progress = new Progress<int>(p => Console.WriteLine($"[Progress] {p}% completed"));

            Console.WriteLine("Downloading 5 popular sites in parallel...");
            Console.WriteLine("Press 'c' + Enter to cancel\n");

            _ = Task.Run(() =>
            {
                Console.WriteLine("Type 'c' and press Enter to cancel...");
                if (Console.ReadLine()?.Trim().ToLowerInvariant() == "c")
                    cts.Cancel();
            });

            try
            {
                string[] results = await DownloadMultipleSitesAsync(
                    new[]
                    {
                        "https://dotnet.microsoft.com",
                        "https://github.com",
                        "https://stackoverflow.com",
                        "https://news.ycombinator.com",
                        "https://reddit.com"
                    },
                    progress,
                    cts.Token);

                Console.WriteLine($"\nAll {results.Length} downloads completed successfully:");
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
                Console.WriteLine("\nCancelled by user.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nUnexpected error: {ex.GetBaseException().Message}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static async Task<string[]> DownloadMultipleSitesAsync(
            string[] urls,
            IProgress<int>? progress = null,
            CancellationToken ct = default)
        {
            var tasks = urls.Select(url => DownloadSiteAsync(url, ct)).ToArray();

            string[] results = await Task.WhenAll(tasks).ConfigureAwait(false);

            progress?.Report(100);
            return results;
        }

        static async Task<string> DownloadSiteAsync(string url, CancellationToken ct)
        {
            Console.WriteLine($"[Task {Task.CurrentId}] → {url}");

            using var response = await httpClient.GetAsync(url, ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);

            Console.WriteLine($"[Task {Task.CurrentId}] ← {url} ({content.Length / 1024} KB)");

            return $"{url}|{content.Length}";
        }
    }
}
