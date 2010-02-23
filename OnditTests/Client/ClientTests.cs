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
                using(var client = new Ondit.Client.Client(reader, writer)) {
                    foreach(var message in inputMessages) {
                        client.SendMessage(message);
                    }

                    Assert.AreEqual(expectedOutput, stringWriter.ToString());
                }
            }
        }

        [Test]
        public void MessageSentEvent() {
            ICollection<RawMessage> messages = new List<RawMessage>();

            messages.Add(new RawMessage("THEGAME", "a", "b", "cd ef g"));

            IEnumerator<RawMessage> messageChecker = messages.GetEnumerator();

            var reader = new IO.Helpers.DummyRawMessageReader();
            var writer = new IO.Helpers.DummyRawMessageWriter();

            using(var client = new Ondit.Client.Client(reader, writer)) {
                client.RawMessageSent += delegate(object sender, RawMessageEventArgs e) {
                    bool elementExists = messageChecker.MoveNext();

                    Assert.IsTrue(elementExists);
                    Assert.AreEqual(messageChecker.Current, e.Message);
                };

                foreach(var message in messages) {
                    client.SendMessage(message);
                }
            }
        }

        [Test]
        public void MessageReceive() {
            ICollection<RawMessage> expectedOutput = new List<RawMessage>();

            expectedOutput.Add(new RawMessage("THEGAME", "a", "b", "cd ef g"));

            string input = string.Join("\r\n", expectedOutput.Select((message) => message.ToString()).ToArray()) + "\r\n";

            var writer = new IO.Helpers.DummyRawMessageWriter();

            using(var stringReader = new StringReader(input))
            using(var reader = new RawMessageTextReader(stringReader)) {
                using(var client = new Ondit.Client.Client(reader, writer)) {
                    foreach(var expectedMessage in expectedOutput) {
                        var receivedMessage = client.HandleMessage();

                        Assert.AreEqual(expectedMessage, receivedMessage);
                    }
                }
            }
        }

        [Test]
        public void MessageReceivedEvent() {
            ICollection<RawMessage> expectedOutput = new List<RawMessage>();

            expectedOutput.Add(new RawMessage("THEGAME", "a", "b", "cd ef g"));

            string input = string.Join("\r\n", expectedOutput.Select((message) => message.ToString()).ToArray()) + "\r\n";

            IEnumerator<RawMessage> messageChecker = expectedOutput.GetEnumerator();

            var writer = new IO.Helpers.DummyRawMessageWriter();

            using(var stringReader = new StringReader(input))
            using(var reader = new RawMessageTextReader(stringReader)) {
                using(var client = new Ondit.Client.Client(reader, writer)) {
                    client.RawMessageReceived += delegate(object sender, RawMessageEventArgs e) {
                        bool elementExists = messageChecker.MoveNext();

                        Assert.IsTrue(elementExists);
                        Assert.AreEqual(messageChecker.Current, e.Message);
                    };

                    foreach(var expectedMessage in expectedOutput) {
                        client.HandleMessage();
                    }
                }
            }
        }
    }
}
