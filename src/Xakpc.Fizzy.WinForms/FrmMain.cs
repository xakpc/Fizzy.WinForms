namespace Xakpc.Fizzy.WinForms
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();

            WindowState = FormWindowState.Maximized;

            Load += FrmMain_Load;
            FormClosing += FrmMain_FormClosing;
        }

        private void FrmMain_FormClosing(object? sender, FormClosingEventArgs e)
        {
            DockerHelper.PauseContainer();
        }

        private async void FrmMain_Load(object? sender, EventArgs e)
        {
            if (!await DockerHelper.IsDockerAvailableAsync())
            {
                MessageBox.Show(
                    this,
                    "Docker is not installed or not running.\n\nPlease install Docker Desktop and ensure it's running before starting this application.",
                    "Docker Required",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Close();
                return;
            }

            await StartContainerAsync();
        }

        private async Task StartContainerAsync()
        {
            pbLoading.Visible = true;
            await DockerHelper.StartContainerAsync();
            webView.Source = new Uri("http://localhost:9461");
            startToolStripMenuItem.Enabled = false;
            stopToolStripMenuItem.Enabled = true;
            pbLoading.Visible = false;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            await StartContainerAsync();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DockerHelper.StopContainer();
            startToolStripMenuItem.Enabled = true;
            stopToolStripMenuItem.Enabled = false;
        }

        private async void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DockerHelper.UpdateContainer();
            await StartContainerAsync();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            var dockerVersion = DockerHelper.GetImageVersion();
            var aboutText = $"""
                Fizzy WinForms v{version?.ToString(3) ?? "1.0.0"}

                A Windows Forms wrapper for the Fizzy Docker container.

                Docker Image: ghcr.io/xakpc/fizzy-win-local
                Docker Version: {dockerVersion}

                Links:
                • GitHub: github.com/xakpc/Fizzy.WinForms
                • Docker Image: github.com/xakpc/fizzy-win-local

                Author: Pavel Osadchuk
                License: MIT
                """;

            MessageBox.Show(
                this,
                aboutText,
                "About Fizzy",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void addToProgramsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var startMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
                var programsPath = Path.Combine(startMenuPath, "Programs");
                var shortcutPath = Path.Combine(programsPath, "Fizzy.lnk");

                var exePath = Application.ExecutablePath;
                var workingDir = Path.GetDirectoryName(exePath) ?? AppContext.BaseDirectory;

                // Use PowerShell to create the shortcut
                var psScript = $"""
                    $WshShell = New-Object -ComObject WScript.Shell
                    $Shortcut = $WshShell.CreateShortcut('{shortcutPath}')
                    $Shortcut.TargetPath = '{exePath}'
                    $Shortcut.WorkingDirectory = '{workingDir}'
                    $Shortcut.IconLocation = '{exePath},0'
                    $Shortcut.Description = 'Fizzy Application'
                    $Shortcut.Save()
                    """;

                var process = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "powershell",
                        Arguments = $"-NoProfile -Command \"{psScript.Replace("\"", "\\\"")}\"",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    MessageBox.Show(
                        this,
                        "Fizzy has been added to your Start Menu.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(
                        this,
                        "Failed to create Start Menu shortcut.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    $"Failed to add to programs: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
