using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Ondit;

namespace Ondit.Tests {
    [TestFixture]
    public class RawMessageTests {
        [Test]
        public static void FromString() {
            RawMessage message = RawMessage.FromString(":~a!b@c.d PRIVMSG rec :message goes here");

            Assert.AreEqual("~a!b@c.d", message.Prefix.ToString());
            Assert.AreEqual("PRIVMSG", message.Command);
            Assert.AreEqual(2, message.Arguments.Length);
            Assert.AreEqual("rec", message.Arguments[0]);
            Assert.AreEqual("message goes here", message.Arguments[1]);
        }

        [Test]
        public new static void ToString() {
            RawMessage message = new RawMessage();

            message.Prefix = new RawMessagePrefix("~a!b@c.d");
            message.Command = "PRIVMSG";
            message.Arguments = new string[] { "rec", "message goes here" };

            Assert.AreEqual(":~a!b@c.d PRIVMSG rec :message goes here", message.ToString());
        }

        [Test]
        public static void MessageValidity() {
            Assert.IsFalse((new RawMessage()).IsValid());
            Assert.IsFalse((new RawMessage("")).IsValid());
            Assert.IsFalse((new RawMessage("xy", new string[] { "x y", "z" })).IsValid());
            Assert.IsFalse((new RawMessage("xy", "x y", "z")).IsValid());
            Assert.IsFalse((new RawMessage("xy", new string[] { }, "x y")).IsValid());
            Assert.IsFalse((new RawMessage("xy", new string[] { "x", "y z\r\n" })).IsValid());

            Assert.IsTrue((new RawMessage("xy")).IsValid());
            Assert.IsTrue((new RawMessage("xy", (string[])null)).IsValid());
            Assert.IsTrue((new RawMessage("xy", new string[] { })).IsValid());
            Assert.IsTrue((new RawMessage("xy", new string[] { null })).IsValid());
            Assert.IsTrue((new RawMessage("xy", new string[] { "y z" })).IsValid());
            Assert.IsTrue((new RawMessage("xy", new string[] { "x", "y z" })).IsValid());
            Assert.IsTrue((new RawMessage("xy", "x", "y z")).IsValid());
            Assert.IsTrue((new RawMessage("xy", new string[] { "x", "y z" }, "abc")).IsValid());
        }

        [Test]
        public static void Equals() {
            Assert.AreEqual(new RawMessage("abc", new string[] { "def", "ghi jkl" }, "mno"), new RawMessage("abc", new string[] { "def", "ghi jkl" }, "mno"));
            Assert.AreNotEqual(new RawMessage("abc", new string[] { "def", "ghi jkl" }, "mno"), new RawMessage("abq", new string[] { "def", "ghi jkl" }, "mno"));
            Assert.AreNotEqual(new RawMessage("abc", new string[] { "def", "ghi jkl" }, "mno"), new RawMessage("abc", new string[] { "dzf", "ghi jkl" }, "mno"));
            Assert.AreNotEqual(new RawMessage("abc", new string[] { "def", "ghi jkl" }, "mno"), new RawMessage("abc", new string[] { "def", "ghi jklasd" }, "mno"));
            Assert.AreNotEqual(new RawMessage("abc", new string[] { "def", "ghi jkl" }, "mno"), new RawMessage("abc", new string[] { "def", "123", "ghi jkl" }, "mno"));
            Assert.AreNotEqual(new RawMessage("abc", new string[] { "def", "ghi jkl" }, "mno"), new RawMessage("abc", new string[] { "def", "ghi jkl" }, "XYZ"));
        }
    }
}
