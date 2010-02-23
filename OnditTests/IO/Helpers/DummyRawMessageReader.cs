using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ondit.IO;

namespace Ondit.Tests.IO.Helpers {
    public class DummyRawMessageReader : IRawMessageReader {
        public RawMessage Read() {
            return null;
        }

        public RawMessage ReadBlock() {
            return null;
        }
    }
}
