using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Ondit {
    public class RawMessageTextReader : IRawMessageReader, IDisposable {
        private static string messageDivider = "\r\n";

        private TextReader source;
        private StringBuilder buffer = new StringBuilder();

        public RawMessageTextReader(TextReader source) {
            this.source = source;
        }

        private int BufferBlock(int blockSize, Func<char[], int> reader) {
            char[] tmpBuffer = new char[blockSize];
            int charsRead = reader(tmpBuffer);

            buffer.Append(new string(tmpBuffer.Take(charsRead).ToArray()));

            return charsRead;
        }

        private RawMessage ReadFromBuffer() {
            string str = buffer.ToString();

            int end = str.IndexOf(messageDivider);

            if(end < 0) {
                return null;
            }

            this.buffer = new StringBuilder(str.Substring(end + messageDivider.Length));

            return RawMessage.FromString(str.Substring(0, end));
        }

        private RawMessage ReadImpl(int blockSize, Func<char[], int> reader) {
            RawMessage ret = ReadFromBuffer();

            if(ret != null) {
                return ret;
            }

            int read = blockSize;

            while(read == blockSize) {
                read = BufferBlock(blockSize, reader);

                ret = ReadFromBuffer();

                if(ret != null) {
                    return ret;
                }
            }

            return null;
        }

        public RawMessage Read() {
            return ReadImpl(512, (char[] buffer) => source.Read(buffer, 0, buffer.Length));
        }

        public RawMessage ReadBlock() {
            return ReadImpl(1, (char[] buffer) => source.Read(buffer, 0, buffer.Length));
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing) {
            if(!disposed) {
                if(disposing) {
                    /* Nothing yet. */
                }

                disposed = true;
            }
        }

        public void Dispose() {
            Dispose(true);
        }
    }
}
