using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ondit.Client;
using NUnit.Framework;

namespace Ondit.Tests.Client {
    [TestFixture]
    public class ChannelTests {
        [Test]
        public void Target() {
            var channel = new Channel("#test");

            Assert.AreEqual("#test", channel.Target);
        }
    }
}
