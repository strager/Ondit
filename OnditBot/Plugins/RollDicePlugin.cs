using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ondit.Client;

namespace OnditBot.Plugins {
    public class RollDicePlugin : CommandPlugin {
        private readonly Random random = new Random();

        public override string Name {
            get {
                return "Dice rolling plugin";
            }
        }

        public override string Trigger {
            get {
                return "roll";
            }
        }

        protected override void  MessageReceived(IConversable sender, string message) {
            int roll = random.Next(1, 6);

            Client.SendMessage(sender, string.Format("Rolling a 6-sided die: {0}", roll));
        }
    }
}
