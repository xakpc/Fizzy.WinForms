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
    }
}
