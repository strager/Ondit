using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Ondit.IO {
    public class RawMessageTextWriter : IRawMessageWriter, IDisposable {
        private static string messageDivider = "\r\n";

        private TextWriter source;

        public RawMessageTextWriter(TextWriter source) {
            this.source = source;
        }

        public void Write(RawMessage message) {
            this.source.Write(message.ToString() + messageDivider);
            this.source.Flush();
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
