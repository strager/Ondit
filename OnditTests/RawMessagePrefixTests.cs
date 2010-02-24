using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Ondit;

namespace Ondit.Tests {
    [TestFixture]
    public class RawMessagePrefixTests {
        [Test]
        public static void FromString() {
            RawMessagePrefix prefix = new RawMessagePrefix("~aaa!b@c.d");

            Assert.AreEqual("aaa", prefix.Nick);
            Assert.AreEqual("b", prefix.UserName);
            Assert.AreEqual("c.d", prefix.Host);
            Assert.AreEqual(false, prefix.IsIdentified);
        }

        [Test]
        public new static void ToString() {
            RawMessagePrefix prefix = new RawMessagePrefix();

            prefix.Nick = "aaa";
            prefix.UserName = "b";
            prefix.Host = "c.d";
            prefix.IsIdentified = false;

            Assert.AreEqual("~aaa!b@c.d", prefix.ToString());
        }

        [Test]
        public static void MessageValidity() {
            // TODO
        }

        [Test]
        public static void Equals() {
            Assert.AreEqual(new RawMessagePrefix("~aaa!b@c.d"), new RawMessagePrefix("~aaa!b@c.d"));
            Assert.AreNotEqual(new RawMessagePrefix("~aaa!b@c.d"), new RawMessagePrefix("~aa!b@c.d"));
            Assert.AreNotEqual(new RawMessagePrefix("~aaa!b@c.d"), new RawMessagePrefix("~aaa!d@c.d"));
            Assert.AreNotEqual(new RawMessagePrefix("~aaa!b@c.d"), new RawMessagePrefix("~aaa!b@c.a"));
            Assert.AreNotEqual(new RawMessagePrefix("~aaa!b@c.d"), new RawMessagePrefix("aaa!b@c.d"));
        }
    }
}
