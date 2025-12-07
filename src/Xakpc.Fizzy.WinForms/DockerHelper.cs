using System.Diagnostics;
using System.Net.Http;

namespace Xakpc.Fizzy.WinForms
{
    internal static class DockerHelper
    {
        private const string ContainerName = "fizzy";
        private const string ImageName = "ghcr.io/xakpc/fizzy-win-local";
        private const string HealthCheckUrl = "http://localhost:9461";
        private const int MaxRetries = 30;
        private const int RetryDelayMs = 500;
        private static readonly string DataPath = Path.Combine(AppContext.BaseDirectory, "data");

        public static async Task StartContainerAsync()
        {
            try
            {
                var output = await RunCommandAsync($"ps -q -f name={ContainerName}");

                if (string.IsNullOrWhiteSpace(output))
                {
                    var stoppedOutput = await RunCommandAsync($"ps -aq -f name={ContainerName}");

                    if (!string.IsNullOrWhiteSpace(stoppedOutput))
                    {
                        await RunCommandAsync($"start {ContainerName}");
                    }
                    else
                    {
                        Directory.CreateDirectory(DataPath);
                        await RunCommandAsync($"run -d -p 9461:80 -v \"{DataPath}:/rails/storage\" -e SECRET_KEY_BASE=a9f8b7c6d5e4f3a2b1c0d9e8f7a6b5c4d3e2f1a0b9c8d7e6f5a4b3c2d1e0f9a8 --name {ContainerName} {ImageName}");
                    }
                }
                else
                {
                    // Container is running, check if paused
                    var pausedOutput = await RunCommandAsync($"ps -q -f name={ContainerName} -f status=paused");
                    if (!string.IsNullOrWhiteSpace(pausedOutput))
                    {
                        await RunCommandAsync($"unpause {ContainerName}");
                    }
                }

                await WaitForContainerReadyAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DockerHelper] Failed to start container: {ex}");
                MessageBox.Show($"Failed to start Docker container: {ex.Message}", "Docker Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static async Task WaitForContainerReadyAsync()
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };

            for (int i = 0; i < MaxRetries; i++)
            {
                try
                {
                    var response = await client.GetAsync(HealthCheckUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DockerHelper] Health check attempt {i + 1}/{MaxRetries} failed: {ex.Message}");
                }

                await Task.Delay(RetryDelayMs);
            }
        }

        public static void StopContainer()
        {
            try
            {
                RunCommand($"stop {ContainerName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DockerHelper] Failed to stop container: {ex}");
            }
        }

        public static void PauseContainer()
        {
            try
            {
                RunCommand($"pause {ContainerName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DockerHelper] Failed to pause container: {ex}");
            }
        }

        public static void UnpauseContainer()
        {
            try
            {
                RunCommand($"unpause {ContainerName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DockerHelper] Failed to unpause container: {ex}");
            }
        }

        public static async Task<bool> IsDockerAvailableAsync()
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "docker",
                        Arguments = "info",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                await process.WaitForExitAsync();
                return process.ExitCode == 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DockerHelper] Docker not available: {ex}");
                return false;
            }
        }

        public static void UpdateContainer()
        {
            StopContainer();
            Directory.CreateDirectory(DataPath);
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = "cmd",
                Arguments = $"/k docker pull {ImageName} && docker rm {ContainerName} && docker run -d -p 9461:80 -v \"{DataPath}:/rails/storage\" -e SECRET_KEY_BASE=a9f8b7c6d5e4f3a2b1c0d9e8f7a6b5c4d3e2f1a0b9c8d7e6f5a4b3c2d1e0f9a8 --name {ContainerName} {ImageName}",
                UseShellExecute = true
            });
            process?.WaitForExit();
        }

        private static async Task<string> RunCommandAsync(string arguments)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "docker",
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();
            return output;
        }

        private static void RunCommand(string arguments)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "docker",
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            process.Start();
            process.WaitForExit();
        }
    }
}
