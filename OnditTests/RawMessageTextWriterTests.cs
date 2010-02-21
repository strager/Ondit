using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NUnit.Framework;

namespace Ondit.Tests {
    [TestFixture]
    public class RawMessageTextWriterTests {
        private static RawMessage messageA = new RawMessage("COMMAND", new string[] { "arg", "args here" }, "host");
        private static RawMessage messageB = new RawMessage("123", new string[] { "abc" });

        [Test]
        public void TestWrite() {
            using(var stringWriter = new StringWriter())
            using(var messageWriter = new RawMessageTextWriter(StringWriter)) {
                messageWriter.Write(messageA);
                messageWriter.Write(messageB);

                Assert.AreEqual(messageA + "\r\n" + messageB + "\r\n", stringWriter.ToString());
            }
        }
    }
}
