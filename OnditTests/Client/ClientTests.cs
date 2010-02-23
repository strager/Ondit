using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ondit;
using Ondit.IO;
using Ondit.Client;
using NUnit.Framework;

namespace Ondit.Tests.Client {
    [TestFixture]
    public class ClientTests {
        [Test]
        public void MessageSend() {
            ICollection<RawMessage> inputMessages = new List<RawMessage>();

            inputMessages.Add(new RawMessage("THEGAME", "a", "b", "cd ef g"));

            string expectedOutput = string.Join("\r\n", inputMessages.Select((message) => message.ToString()).ToArray()) + "\r\n";

            var reader = new IO.Helpers.DummyRawMessageReader();

            using(var stringWriter = new StringWriter())
            using(var writer = new RawMessageTextWriter(stringWriter)) {
                var client = new Ondit.Client.Client(reader, writer);

                foreach(var message in inputMessages) {
                    client.SendMessage(message);
                }

                Assert.AreEqual(expectedOutput, stringWriter.ToString());
            }
        }

        [Test]
        public void MessageReceive() {
            ICollection<RawMessage> expectedOutput = new List<RawMessage>();

            expectedOutput.Add(new RawMessage("THEGAME", "a", "b", "cd ef g"));

            string input = string.Join("\r\n", expectedOutput.Select((message) => message.ToString()).ToArray()) + "\r\n";

            var writer = new IO.Helpers.DummyRawMessageReader();

            using(var stringReader = new StringReader(input))
            using(var reader = new RawMessageTextReader(stringReader)) {
                var client = new Ondit.Client.Client(reader, writer);

                foreach(var expectedMessage in expectedOutput) {
                    var receivedMessage = client.HandleMessage();

                    Assert.AreEqual(expectedMessage, receivedMessage);
                }
            }
        }
    }
}
