using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ondit {
    public sealed class RawMessagePrefix : IEquatable<RawMessagePrefix> {
        public bool IsIdentified {
            get;
            set;
        }

        public string Nick {
            get;
            set;
        }

        public string UserName {
            get;
            set;
        }

        public string Host {
            get;
            set;
        }

        public RawMessagePrefix() {
            IsIdentified = false;
        }

        public RawMessagePrefix(string rawPrefixString) {
            var match = RawMessage.Expressions.GetFullMatcher(RawMessage.Expressions.LazyPrefix).Match(rawPrefixString);

            if(!match.Success) {
                return;
            }

            if(match.Groups["Ident"].Success) {
                IsIdentified = !match.Groups["Ident"].Value.Contains('~');
            }

            if(match.Groups["Nick"].Success) {
                Nick = match.Groups["Nick"].Value;
            }

            if(match.Groups["User"].Success) {
                UserName = match.Groups["User"].Value;
            }

            if(match.Groups["Host"].Success) {
                Host = match.Groups["Host"].Value;
            }
        }

        public bool IsValid() {
            if(Nick == null) {
                return false;
            }

            if(!RawMessage.Expressions.GetFullMatcher(RawMessage.Expressions.Nick).IsMatch(Nick)) {
                return false;
            }

            if(UserName != null && !RawMessage.Expressions.GetFullMatcher(RawMessage.Expressions.User).IsMatch(UserName)) {
                return false;
            }

            if(Host != null && !RawMessage.Expressions.GetFullMatcher(RawMessage.Expressions.Host).IsMatch(Host)) {
                return false;
            }

            return true;
        }

        public override string ToString() {
            string s = "";

            if(!IsIdentified) {
                s += "~";
            }

            if(Nick != null) {
                s += Nick;
            }

            if(UserName != null) {
                s += "!" + UserName;
            }

            if(Host != null) {
                s += "@" + Host;
            }

            return s;
        }

        public bool Equals(RawMessagePrefix other) {
            if(other == null) {
                return false;
            }

            return this.ToString() == other.ToString();
        }

        public override bool Equals(object obj) {
            if(obj == null || !(obj is RawMessagePrefix)) {
                return false;
            }

            return this.Equals(obj as RawMessagePrefix);
        }

        public override int GetHashCode() {
            return base.GetHashCode() ^ Nick.GetHashCode() ^ UserName.GetHashCode() ^ Host.GetHashCode();
        }
    }
}
