using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NUnit.Framework;
using Ondit.IO;

namespace Ondit.Tests.IO {
    [TestFixture]
    public class RawMessageTextReaderTests {
        private static RawMessage messageA = new RawMessage("COMMAND", new string[] { "arg", "args here" }, "host");
        private static RawMessage messageB = new RawMessage("123", new string[] { "abc" });

        [Test]
        public void TestRead() {
            string input = messageA.ToString() + "\r\n" + messageB.ToString();

            using(var stringReader = new StringReader(input))
            using(var messageReader = new RawMessageTextReader(stringReader)) {
                var message = messageReader.Read();

                Assert.AreEqual(messageA, message);

                message = messageReader.Read();

                Assert.IsNull(message);
            }
        }
    }
}
