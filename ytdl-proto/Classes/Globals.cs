using Ini;
using YoutubeDLSharp;
using YoutubeDLSharp.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Forms;
using YTDL.Forms;

namespace YTDL.Classes {
    public static class Globals {
        public static string proto = "ytdl://";

        public static string appPath = Assembly.GetExecutingAssembly().Location;
        public static string cmd = $"\"{appPath}\" \"%1\"";
        public static string outdir = KnownFolders.GetPath(KnownFolder.Downloads);
        public static string ytdlPath = Path.Combine(Path.GetTempPath(), "youtube-dl.exe");
        public static string ffmpegPath = Path.Combine(Path.GetTempPath(), "ffmpeg.exe");
        public static string configPath = Path.Combine(Application.StartupPath, "config.txt");
        public static frmMain mainForm = new frmMain();
        public static IPC comms = new IPC();

        public static YoutubeDL youtubeDl = new YoutubeDL();
        public static OptionSet options = OptionSet.Default;

        static Globals() {
            if (!File.Exists(configPath)) {
                options.Output = Path.Combine(outdir, "%(title)s.%(ext)s");
                options.NoMtime = true;
                options.Format = "mp4";
                options.WriteConfigFile(configPath);
            }
            options = OptionSet.LoadConfigFile(configPath);

            if (!File.Exists(ytdlPath)) {
                Console.WriteLine("[ytdl-proto] Downloading ytdl...");
                new WebClient().DownloadFile("https://yt-dl.org/downloads/latest/youtube-dl.exe", ytdlPath);
            }

            /*if (!File.Exists(ffmpegPath)) {
                Console.WriteLine("[ytdl-proto] Downloading ffmpeg...");
                new WebClient().DownloadFile("https://yt-dl.org/downloads/latest/youtube-dl.exe", ffmpegPath);
            }*/

            youtubeDl.YoutubeDLPath = ytdlPath;
        }
    }
}
