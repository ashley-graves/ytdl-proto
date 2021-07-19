using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeDLSharp;
using YoutubeDLSharp.Metadata;
using static YTDL.Classes.Globals;

namespace YTDL.Forms {
    public partial class frmMain : Form {
        public Dictionary<string, bool> RunningDownloads = new Dictionary<string, bool>();

        public frmMain() {
            InitializeComponent();
        }

        public async void DownloadVideo(string url) {
            WindowState = FormWindowState.Normal;
            Activate();

            Panel pnl = new Panel();
            pnl.Size = new Size(776, 60);
            pnl.Dock = DockStyle.Top;

            PictureBox thumb = new PictureBox();
            thumb.Location = new Point(3, 3);
            thumb.BackColor = Color.Black;
            thumb.Size = new Size(96, 54);
            thumb.SizeMode = PictureBoxSizeMode.StretchImage;

            Label title = new Label();
            title.AutoSize = true;
            title.Location = new Point(103, 2);
            title.Text = "Fetching video data...";

            ProgressBar progressBar = new ProgressBar();
            progressBar.Location = new Point(106, 36);
            progressBar.Size = new Size(591, 21);
            progressBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            VideoData data = null;

            var cts = new CancellationTokenSource();

            Button cancelButton = new Button();
            cancelButton.Location = new Point(698, 35);
            cancelButton.Size = new Size(75, 23);
            cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cancelButton.Click += (object sender, EventArgs e) => {
                cts.Cancel();
                if (data != null)
                    RunningDownloads[data.ID] = true;
                panel1.Controls.Remove(pnl);
            };
            cancelButton.Text = "Cancel";

            var progress = new Progress<DownloadProgress>(p => {
                RunningDownloads[data.ID] = p.State == DownloadState.Success || p.State == DownloadState.Error;
                title.Text = "[" + p.State.ToString() + "] " + data.Title;
                if(File.Exists(p.Data)) {
                    Process.Start("explorer.exe", $"/select, \"{p.Data}\"");
                }
                if(p.State == DownloadState.Success) {
                    title.Text += Environment.NewLine + "Successfully Downloaded!";
                    progressBar.Value = 100;
                    cancelButton.Text = "Finish";
                    return;
                }
                if(p.DownloadSpeed == null || string.IsNullOrEmpty(p.DownloadSpeed.Trim()) || p.ETA == null || string.IsNullOrEmpty(p.ETA.Trim())) return;
                title.Text += Environment.NewLine + "Size: " + p.TotalDownloadSize + " | Speed: " + p.DownloadSpeed + " | ETA: " + p.ETA;
                int progressVal = (int)Math.Floor(p.Progress * 100);
                if(progressBar.Value < progressVal) progressBar.Value = progressVal;
            });

            pnl.Controls.Add(thumb);
            pnl.Controls.Add(title);
            pnl.Controls.Add(progressBar);
            pnl.Controls.Add(cancelButton);

            panel1.Controls.Add(pnl);

            try {
                data = (await youtubeDl.RunVideoDataFetch(url)).Data;
                if (data == null) {
                    MessageBox.Show("Invalid Video URL", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                } else if (data.IsLive != null && (bool)data.IsLive) {
                    MessageBox.Show("Cannot download live videos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (data.Thumbnail.StartsWith("https://i.ytimg.com/")) {
                    thumb.ImageLocation = $"https://i.ytimg.com/vi/{data.ID}/maxresdefault.jpg";
                } else {
                    thumb.ImageLocation = data.Thumbnail;
                }
                title.Text = "[Starting] " + data.Title;
            } catch (Exception ex) {
                MessageBox.Show(ex.Message + Environment.NewLine + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try {
                await youtubeDl.RunVideoDownload(url, options.Format, progress: progress, ct: cts.Token, overrideOptions: options);
            } catch (TaskCanceledException) {
            } catch (Exception ex) {
                MessageBox.Show(ex.Message + Environment.NewLine + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmMain_Load(object sender, EventArgs e) {
            CheckForIllegalCrossThreadCalls = false;
            TaskbarIcon.Icon = Icon;
        }

        private void button1_Click(object sender, EventArgs e) {
            DownloadVideo(textBox1.Text);
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e) {
            e.Cancel = true;
            Hide();
            TaskbarIcon.Visible = true;
        }

        private void TaskbarIcon_MouseDoubleClick(object sender, MouseEventArgs e) {
        }

        private void TaskbarIcon_MouseClick(object sender, MouseEventArgs e) {
            if (e.Button != MouseButtons.Left) return;
            TaskbarIcon.Visible = false;
            ShowDialog();
            Activate();
        }

        private void exitButton_Click(object sender, EventArgs e) {
            Environment.Exit(Environment.ExitCode);
        }
    }
}
