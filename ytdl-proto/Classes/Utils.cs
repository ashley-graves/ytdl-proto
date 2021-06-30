using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using static YTDL.Classes.Globals;

namespace YTDL.Classes {
    public static class Utils {
        public static bool IsInstalled() {
            bool handler = false;
            using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"ytdl\shell\open\command")) {
                if ((string)key.GetValue("") == cmd) handler = true;
            }
            
            bool startup = false;
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true)) {
                if (key.GetValueNames().Contains("ytdl")) {
                    if ((string)key.GetValue("ytdl") == appPath) startup = true;
                }
            }
            
            return handler && startup;
        }
        public static void AddRegKeys() {
            using (var YTDLKey = Registry.ClassesRoot.CreateSubKey("ytdl")) {
                YTDLKey.SetValue("", "URL:ytdl Protocol");
                YTDLKey.SetValue("URL Protocol", "");

                using (var CommandKey = YTDLKey.CreateSubKey(@"shell\open\command")) {
                    CommandKey.SetValue("", cmd);
                }
            }

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true)) {
                key.SetValue("ytdl", appPath);
            }
        }
        public static bool IsAdministrator() {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static void RunAsAdmin() {
            var exeName = Process.GetCurrentProcess().MainModule.FileName;
            ProcessStartInfo startInfo = new ProcessStartInfo(exeName);
            startInfo.Verb = "runas";
            startInfo.Arguments = "restart";
            try {
                Process.Start(startInfo);
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                Console.ReadKey(true);
            }
            Environment.Exit(Environment.ExitCode);
        }
    }
}
