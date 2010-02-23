using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ondit.IO;

namespace Ondit.Tests.IO.Helpers {
    public class DummyRawMessageWriter : IRawMessageWriter {
        public void Write(RawMessage message) {
            // Do nothing.
        }
    }
}
