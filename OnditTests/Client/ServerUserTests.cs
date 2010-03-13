using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ondit.Client;
using NUnit.Framework;

namespace Ondit.Tests.Client {
    [TestFixture]
    public class ServerUserTests {
        [Test]
        public void Target() {
            var user = new ServerUser() {
                Nick = "nickhere"
            };

            Assert.AreEqual("nickhere", user.Nick);
            Assert.AreEqual("nickhere", user.Target);
        }
    }
}
