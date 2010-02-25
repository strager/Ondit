using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ondit {
    /// <summary>
    /// Provides a container for the prefix portion of an IRC message.
    /// </summary>
    public sealed class RawMessagePrefix : IEquatable<RawMessagePrefix> {
        /// <summary>
        /// Specifies if the sender has been identified using ident.
        /// </summary>
        public bool IsIdentified {
            get;
            set;
        }

        /// <summary>
        /// The nickname of the sender.
        /// </summary>
        public string Nick {
            get;
            set;
        }

        /// <summary>
        /// The username of the sender.
        /// </summary>
        public string UserName {
            get;
            set;
        }

        /// <summary>
        /// The host of the sender.
        /// </summary>
        public string Host {
            get;
            set;
        }

        /// <summary>
        /// Creates an instance of RawMessagePrefix.
        /// </summary>
        public RawMessagePrefix() {
            IsIdentified = false;
        }

        /// <summary>
        /// Creates an instance of RawMessagePrefix given a raw prefix string.
        /// </summary>
        /// <param name="rawPrefixString">Raw prefix string from an IRC message.</param>
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

        /// <summary>
        /// Returns if this instance makes a valid prefix.
        /// </summary>
        /// <returns>True if the instance is valid.</returns>
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

        /// <summary>
        /// Returns the prefix string this instance represents.
        /// </summary>
        /// <returns>Raw prefix string.</returns>
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
