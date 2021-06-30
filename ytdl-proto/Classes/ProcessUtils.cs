using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YTDL {
    public class OutputContext {
        public const int BufferSize = 1024;

        public bool IsEof { get; set; }
        public bool IsWaiting { get; set; }
        public byte[] Buffer { get; set; }
        public StreamReader Reader { get; set; }
        public object Tag { get; set; }

        public OutputContext(StreamReader r, object tag) {
            IsEof = false;
            IsWaiting = false;
            Buffer = new byte[BufferSize];
            Reader = r;
            Tag = tag;
        }
    }
    public static class Processor {
        public static void Callback(IAsyncResult ar) {
            lock (ar.AsyncState) {
                OutputContext ctx = ar.AsyncState as OutputContext;
                int c = ctx.Reader.BaseStream.EndRead(ar);
                ctx.IsWaiting = false;

                if (c == 0) {
                    ctx.IsEof = true;
                    return;
                }

                string content = Encoding.UTF8.GetString(ctx.Buffer, 0, c);
                Console.Write(content);
                Console.Out.Flush();

            }
        }

        public static void RedirectOutput(OutputContext ctx) {
            lock (ctx) {
                if (ctx.IsEof) {
                    return;
                }

                if (ctx.IsWaiting) {
                    return;
                }

                ctx.IsWaiting = true;
                IAsyncResult ar = ctx.Reader.BaseStream.BeginRead(ctx.Buffer, 0, OutputContext.BufferSize, Callback, ctx);
            }
        }
    }
}
