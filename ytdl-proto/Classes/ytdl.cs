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
using YTDL.Classes;
using static YTDL.Classes.Globals;

namespace YTDL.Classes {
    class ytdl {
        static void YTDL(string[] args) {
            Console.Title = "YTDL Protocol Handler";
            Console.CursorVisible = false;

            if (args.Length > 0) {
                var arg = args[0].Trim();
                if (arg.StartsWith(proto)) {
                    arg = arg.Substring(proto.Length);

                    if (arg.EndsWith("/")) {
                        arg = arg.Substring(0, arg.Length - 1);
                    }
                }
                try {
                    RunExternalExe(ytdlPath, arg);
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            } else {
                Console.WriteLine("[ytdl-proto] This application is only meant to be used as a URL protocol handler, it cannot be used as a standalone application.");
                Console.ReadKey(true);
            }
        }

        public static void RunExternalExe(string filename, string arguments = null) {
            var process = new Process();

            process.StartInfo.FileName = filename;
            if (!string.IsNullOrEmpty(arguments)) {
                process.StartInfo.Arguments = arguments;
            }

            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.UseShellExecute = false;

            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;

            try {
                process.Start();

                char[] buffer = new char[1];

                OutputContext stdoutCtx = new OutputContext(process.StandardOutput, "STDOUT");
                OutputContext stderrCtx = new OutputContext(process.StandardError, "STDERR");

                while (!stdoutCtx.IsEof && !stderrCtx.IsEof) {
                    Processor.RedirectOutput(stdoutCtx);
                    Processor.RedirectOutput(stderrCtx);
                }


                process.WaitForExit();
            } catch (Exception e) {
                throw new Exception("OS error while executing " + Format(filename, arguments) + ": " + e.Message, e);
            }

            if (process.ExitCode != 0) {
                RunExternalExe(filename, arguments);
            }
        }
        private static string Format(string filename, string arguments) {
            return "'" + filename + (string.IsNullOrEmpty(arguments) ? string.Empty : " " + arguments) + "'";
        }


    }
}
