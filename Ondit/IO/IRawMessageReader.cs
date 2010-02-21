using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ondit.IO {
    public interface IRawMessageReader {
        RawMessage Read();
        RawMessage ReadBlock();
    }
}
