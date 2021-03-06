﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ondit {
    /// <summary>
    /// Provides a container for a single IRC message.
    /// </summary>
    public sealed class RawMessage : IEquatable<RawMessage> {
        internal static class Expressions {
            public static Regex GetFullMatcher(string expression) {
                System.Diagnostics.Debug.Assert(expression != null, "expression is null");

                return new Regex(@"^(" + expression + @")$", RegexOptions.IgnorePatternWhitespace);
            }

            public static string CharString = @"[^ \b\0\n\r,]*?";
            public static string NonWhite = @"[^ \0\n\r]";
            public static string SpecialChar = @"[][\\`^{}-]";
            public static string Number = @"[0-9]";
            public static string Letter = @"[a-zA-Z]";
            public static string Space = @"[ ]+?";

            public static string MiddleArgument = @"[^ \0\n\r:][^ \0\n\r]*?";
            public static string TrailingArgument = @"[^\0\n\r]*?";

            public static string Host = @"(" + NonWhite + @")+?";    // TODO
            public static string ServerName = Host;
            public static string Channel = @"[#&]" + CharString;
            public static string Nick = Letter + @"(" + Letter + @"|" + Number + @"|" + SpecialChar + @").*?";
            public static string User = @"(" + NonWhite + @")+?";
            public static string Mask = @"[#$]" + CharString;
            public static string Ident = @"[~]?";

            public static string To = Channel + @"|(" + User + @"@" + ServerName + @")|" + Nick + @"|" + Mask;
            public static string Target = To + @"(," + To + @")*?";

            public static string Prefix = @"(" + ServerName + @")|(" + Ident + @")(" + Nick + @"(!(" + User + @"))?(@(" + Host + @"))?)";
            public static string Command = Letter + @"(" + Letter + @")+?|(" + Number + @"){3}";
            public static string Arguments = @"(" + Space + MiddleArgument + @")*?(:" + Space + TrailingArgument + @")?";

            public static string Message = @"(:" + Prefix + Space + @")?" + Command + Arguments;

            public static string LazyPrefix = @"(?<Ident>" + Ident + @")(?<Nick>.*?)(!(?<User>.*?))?(@(?<Host>.*?))?";
            public static string LazyMessage = @"(:(?<prefix>.*?)[ ]+)?  (?<args>.*?)  ([ ]:(?<lastarg>.*?))?";
        }

        /// <summary>
        /// Source of the message.
        /// </summary>
        public RawMessagePrefix Prefix {
            get;
            set;
        }

        /// <summary>
        /// Type of message.
        /// </summary>
        public string Command {
            get;
            set;
        }

        /// <summary>
        /// Additional arguments the message contains.
        /// </summary>
        public string[] Arguments {
            get;
            set;
        }

        /// <summary>
        /// Creates an empty message.
        /// </summary>
        public RawMessage() :
            this(null) {
        }

        /// <summary>
        /// Creates a message.
        /// </summary>
        /// <param name="command">Type of message.</param>
        /// <param name="arguments">Additional arguments the message contains.</param>
        public RawMessage(string command, params string[] arguments) :
            this(command, arguments, (RawMessagePrefix)null) {
        }

        /// <summary>
        /// Creates a message.
        /// </summary>
        /// <param name="command">Type of message.</param>
        /// <param name="arguments">Additional arguments the message contains.</param>
        /// <param name="prefix">Source of the message.</param>
        public RawMessage(string command, string[] arguments, string prefix) :
            this(command, arguments, new RawMessagePrefix(prefix)) {
        }

        /// <summary>
        /// Creates a message.
        /// </summary>
        /// <param name="command">Type of message.</param>
        /// <param name="arguments">Additional arguments the message contains.</param>
        /// <param name="prefix">Source of the message.</param>
        public RawMessage(string command, string[] arguments, RawMessagePrefix prefix) {
            Command = command;
            Arguments = arguments;
            Prefix = prefix;
        }

        /// <summary>
        /// Checks whether this message is a valid one to sent.
        /// </summary>
        /// <returns>True if the message is valid.</returns>
        public bool IsValid() {
            return IsPrefixValid() && IsCommandValid() && IsArgumentsValid();
        }

        private bool IsPrefixValid() {
            return Prefix == null || Prefix.IsValid();
        }

        private bool IsCommandValid() {
            return Command != null && Expressions.GetFullMatcher(Expressions.Command).IsMatch(Command);
        }

        private bool IsArgumentsValid() {
            if(Arguments == null) {
                return true;
            }

            for(int i = 0; i < Arguments.Length - 1; ++i) {
                if(Arguments[i] == null) {
                    continue;
                }

                if(!Expressions.GetFullMatcher(Expressions.MiddleArgument).IsMatch(Arguments[i])) {
                    return false;
                }
            }

            if(Arguments.Length - 1 >= 0 && Arguments[Arguments.Length - 1] != null) {
                if(!Expressions.GetFullMatcher(Expressions.TrailingArgument).IsMatch(Arguments[Arguments.Length - 1])) {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Converts a raw string message to a RawMessage as defined by the IRC RFC.
        /// </summary>
        /// <param name="raw">String containing the raw message.</param>
        /// <returns>Message <paramref name="raw"/> represents.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="raw"/> is null.</exception>
        public static RawMessage FromString(string raw) {
            if(raw == null) {
                throw new ArgumentNullException("raw");
            }

            var re = Expressions.GetFullMatcher(Expressions.LazyMessage);

            var match = re.Match(raw);
            var message = new RawMessage();

            if(!match.Success) {
                return message;
            }

            if(match.Groups["prefix"].Success) {
                message.Prefix = new RawMessagePrefix(match.Groups["prefix"].Value);
            }

            string[] rawArgs = match.Groups["args"].Value.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if(match.Groups["lastarg"].Success) {
                var argList = rawArgs.ToList();
                argList.Add(match.Groups["lastarg"].Value);
                rawArgs = argList.ToArray();
            }

            if(rawArgs.Length > 0) {
                message.Command = rawArgs[0];
                message.Arguments = rawArgs.Skip(1).ToArray();
            }

            return message;
        }

        /// <summary>
        /// Converts this message into a raw string message as defined by the IRC RFC.
        /// </summary>
        /// <returns>Raw string representing the message.</returns>
        /// <exception cref="InvalidOperationException"><see cref="Command"/> is null.</exception>
        public override string ToString() {
            string output = "";

            if(Prefix != null && Prefix.ToString().Trim() != "") {
                output += ":" + Prefix + " ";
            }

            if(Command == null) {
                throw new InvalidOperationException("Command cannot be null");
            }

            output += Command;

            if(Arguments != null) {
                for(int i = 0; i < Arguments.Length - 1; ++i) {
                    output += " " + Arguments[i] ?? "";
                }

                if(Arguments.Length - 1 >= 0) {
                    output += " :" + Arguments[Arguments.Length - 1] ?? "";
                }
            }

            return output;
        }

        public bool Equals(RawMessage other) {
            if(other == null) {
                return false;
            }

            if(this.Prefix != null && !this.Prefix.Equals(other.Prefix)) {
                return false;
            }

            if(this.Command != other.Command) {
                return false;
            }

            if(this.Arguments.Length != other.Arguments.Length) {
                return false;
            }

            for(int i = 0; i < this.Arguments.Length; ++i) {
                if(this.Arguments[i] != other.Arguments[i]) {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(object obj) {
            if(obj == null || !(obj is RawMessage)) {
                return false;
            }

            return this.Equals(obj as RawMessage);
        }

        public override int GetHashCode() {
            return base.GetHashCode() ^ Prefix.GetHashCode() ^ Command.GetHashCode() ^ Arguments.GetHashCode();
        }
    }
}
