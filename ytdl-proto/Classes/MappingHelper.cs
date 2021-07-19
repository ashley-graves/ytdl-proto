using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YTDL.Classes {
    public class MappingHelper {
        public MemoryMappedFile file;
        long capacity = 256;
        public MappingHelper(string fileName) {
            file = MemoryMappedFile.CreateOrOpen(fileName, capacity);
        }

        public void WriteString(string msg) {
            using (var stream = file.CreateViewStream()) {
                using (var writer = new BinaryWriter(stream)) {
                    writer.Seek(0, SeekOrigin.Begin);
                    writer.Write(msg);
                    writer.Flush();
                }
            }
        }

        public string ReadString() {
            using (var stream = file.CreateViewStream()) {
                using (var reader = new BinaryReader(stream)) {
                    List<byte> bytes = new List<byte>();
                    while (true) {
                        byte[] temp = new byte[256];
                        int readCount = reader.Read(temp, 0, temp.Length);
                        if (readCount == 0) {
                            break;
                        }
                        bytes.AddRange(temp);
                    }
                    bytes[0] = 0;
                    GC.Collect();
                    if (bytes.Count > 0) {
                        //Remove "\0"
                        return Encoding.Default.GetString(bytes.ToArray()).Replace("\0", "");
                    } else {
                        return null;
                    }
                }
            }

        }
    }
}
