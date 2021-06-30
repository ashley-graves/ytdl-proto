using Ini;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static YTDL.Classes.Globals;
using YTDL.Classes;
using YTDL.Forms;

namespace YTDL {
    class Program {
        [STAThread]
        static void Main(string[] args) {
            Application.EnableVisualStyles();

            if (!Utils.IsInstalled()) {
                if (!Utils.IsAdministrator())
                    Utils.RunAsAdmin();

                Utils.AddRegKeys();
            }

            string arg = "";

            if (args.Length > 0) {
                arg = args[0].Trim();
                if (arg.StartsWith(proto)) {
                    arg = arg.Substring(proto.Length);

                    if (arg.EndsWith("/")) {
                        arg = arg.Substring(0, arg.Length - 1);
                    }
                    comms.shouldListen = false;
                }
            }

            comms.Init();

            if(comms.shouldListen) {
                mainForm.ShowDialog();
            } else {
                try {
                    comms.Write(arg);
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
