﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Ondit;

namespace Ondit.Tests {
    [TestFixture]
    public class RawMessageTests {
        [Test]
        public static void FromRaw() {
            RawMessage message = RawMessage.FromRaw(":~a!b@c.d PRIVMSG rec :message goes here");

            Assert.AreEqual("~a!b@c.d", message.Host);
            Assert.AreEqual("PRIVMSG", message.Command);
            Assert.AreEqual(2, message.Arguments);
            Assert.AreEqual("rec", message.Arguments[0]);
            Assert.AreEqual("message goes here", message.Arguments[1]);
        }

        [Test]
        public static void ToRaw() {
            RawMessage message = new RawMessage();

            message.Host = "~a!b@c.d";
            message.Command = "PRIVMSG";
            message.Arguments = new string[] { "rec", "message goes here" };

            Assert.AreEqual(":~a!b@c.d PRIVMSG rec :message goes here", message.ToString());
        }

        [Test]
        public static void MessageValidity() {
            Assert.IsFalse((new RawMessage()).IsValid());
            Assert.IsFalse((new RawMessage("")).IsValid());
            Assert.IsFalse((new RawMessage("x", new string[] { "x y", "z" })).IsValid());
            Assert.IsFalse((new RawMessage("x", null, "x y")).IsValid());

            Assert.IsTrue((new RawMessage("x")).IsValid());
            Assert.IsTrue((new RawMessage("x", new string[] { })).IsValid());
            Assert.IsTrue((new RawMessage("x", new string[] { "y z" })).IsValid());
            Assert.IsTrue((new RawMessage("x", new string[] { "x", "y z" })).IsValid());
            Assert.IsTrue((new RawMessage("x", new string[] { "x", "y z" }, "abc")).IsValid());
        }
    }
}
