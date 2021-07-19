using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static YTDL.Classes.Globals;

namespace YTDL.Classes {
    public class IPC {
        private string lastRecieved = "";
        public bool isListening = false;
        public bool shouldListen = true;
        public bool active = true;
        public Thread serverThread = null;
        MappingHelper helper = new MappingHelper("YTDL");
        byte[] buf = new byte[1024];

        public IPC() {
            helper.WriteString("");
        }

        public string Read() {
            string data = helper.ReadString();
            if (!string.IsNullOrEmpty(data)) {
                lastRecieved = data;
                helper.file.CreateViewStream().Write(buf, 0, buf.Length);
            }
            var last = lastRecieved;
            lastRecieved = "";
            return last;
        }

        public void Write(string data) {
            helper.WriteString(data);
        }

        public void Init() {
            if (shouldListen) {
                serverThread = new Thread(Server);
                serverThread.Start();
            }
        }

        private void Server() {
            while (true) {
                string str = Read();
                if (!string.IsNullOrEmpty(str)) {
                    str = str.Split('&')[0];
                    if(mainForm.Visible) {
                        Action action = () => mainForm.DownloadVideo(str);
                        mainForm.Invoke(action);
                    } else {
                        new Thread(() => mainForm.ShowDialog()).Start();
                        Write(str);
                    }
                }
                Thread.Sleep(100);
            }
        }
    }
}