using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ondit {
    public class RawMessage : IEquatable<RawMessage> {
        private static class Expressions {
            public static Regex GetFullMatcher(string expression) {
                return new Regex(@"^(" + expression + @")$");
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

            public static string To = Channel + @"|(" + User + @"@" + ServerName + @")|" + Nick + @"|" + Mask;
            public static string Target = To + @"(," + To + @")*?";

            public static string Prefix = ServerName + @"|(" + Nick + @"(!" + User + @")?(@" + Host + @"))?";
            public static string Command = Letter + @"(" + Letter + @")+?|(" + Number + @"){3}";
            public static string Arguments = @"(" + Space + MiddleArgument + @")*?(:" + Space + TrailingArgument + @")?";

            public static string Message = @"(:" + Prefix + Space + @")?" + Command + Arguments;
        }

        public string Prefix {
            get;
            set;
        }

        public string Command {
            get;
            set;
        }

        public string[] Arguments {
            get;
            set;
        }

        public RawMessage() :
            this(null) {
        }

        public RawMessage(string command, params string[] arguments) :
            this(command, arguments, (string)null) {
        }

        public RawMessage(string command, string[] arguments, string host) {
            Command = command;
            Arguments = arguments;
            Prefix = host;
        }

        public bool IsValid() {
            return IsPrefixValid() && IsCommandValid() && IsArgumentsValid();
        }

        public bool IsPrefixValid() {
            return Prefix == null || Expressions.GetFullMatcher(Expressions.Prefix).IsMatch(Prefix);
        }

        public bool IsCommandValid() {
            return Command != null && Expressions.GetFullMatcher(Expressions.Command).IsMatch(Command);
        }

        public bool IsArgumentsValid() {
            if(Arguments == null) {
                return true;
            }

            for(int i = 0; i < Arguments.Length - 1; ++i) {
                if(Arguments[i] == null) {
                    return false;
                }

                if(!Expressions.GetFullMatcher(Expressions.MiddleArgument).IsMatch(Arguments[i])) {
                    return false;
                }
            }

            if(Arguments.Length - 1 >= 0) {
                if(!Expressions.GetFullMatcher(Expressions.TrailingArgument).IsMatch(Arguments[Arguments.Length - 1])) {
                    return false;
                }
            }

            return true;
        }

        public static RawMessage FromString(string raw) {
            var re = new Regex(@"^(:(?<prefix>.*?)[ ]+)?  (?<args>.*?)  ([ ]:(?<lastarg>.*?))?$", RegexOptions.IgnorePatternWhitespace);

            var match = re.Match(raw);
            var message = new RawMessage();

            if(!match.Success) {
                return message;
            }

            if(match.Groups["prefix"].Success) {
                message.Prefix = match.Groups["prefix"].Value;
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

        public override string ToString() {
            string output = "";

            if(Prefix != null && Prefix.Trim() != "") {
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

            if(this.Prefix != other.Prefix || this.Command != other.Command) {
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
