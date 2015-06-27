using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Verse;
using A2B;

namespace A2B_Teleport {
    
    using Receiver = A2B_Teleport.BeltReceiverComponent;
    using Sender = A2B_Teleport.BeltSenderComponent;

    public static class A2BTeleporterManager {

        private static List<Sender>   Senders   = new List<Sender>();
        private static List<Receiver> Receivers = new List<Receiver>();

        public static void AddReceiver(Receiver recv) {
            Receivers.Add(recv);
        }

        public static void RemoveReceiver(Receiver recv) {
            Receivers.Remove(recv);
        }

        public static void AddSender(Sender tele) {
            Senders.Add(tele);
        }

        public static void RemoveSender(Sender tele) {
            Senders.Remove(tele);
        }

        public static IEnumerable<Receiver> GetReceivers() {
            return Receivers;
        }

        public static IEnumerable<Receiver> GetReceivers(int channel) {
            return Receivers.Where(r => r.Channel == channel);
        }

        public static Receiver GetReceiverAtPos(IntVec3 pos) {
            return pos.GetBeltComponent(Level.Both) as Receiver;
        }

        public static IEnumerable<Sender> GetSenders() {
            return Senders;
        }

        public static IEnumerable<Sender> GetSenders(int channel) {
            return Senders.Where(r => r.Channel == channel);
        }

        public static Sender GetSenderAtPos(IntVec3 pos) {
            return pos.GetBeltComponent(Level.Both) as Sender;
        }
    }
}

