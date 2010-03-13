using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ondit.Client;
using NUnit.Framework;

namespace Ondit.Tests.Client {
    [TestFixture]
    public class ChannelUserTests {
        [Test]
        public void Target() {
            var user = new ChannelUser(new Channel("#test"));
            
            Assert.AreEqual("#test", user.Target);
        }
    }
}
