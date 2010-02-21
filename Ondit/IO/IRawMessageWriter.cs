using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ondit.IO {
    public interface IRawMessageWriter {
        void Write(RawMessage message);
    }
}
