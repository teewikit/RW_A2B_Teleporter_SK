#region Usings
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;
using A2B;
#endregion

namespace A2B_Teleport
{

    public class BeltTeleporterComponent : BeltComponent
	{
        private int _chan = 0;
        public int Channel {
            get {
                return _chan;
            }
            protected set {
                _chan = (A2BTeleportData.NumTeleporterChannels + value) % A2BTeleportData.NumTeleporterChannels;
            }
        }

        public override int BeltSpeed {
            get { return (int) (A2BData.BeltSpeed.TicksToMove / A2BTeleportData.SpeedMultiplier); }
        }

        protected class ChannelGizmo : Command {

            public BeltTeleporterComponent teleporter;

            public override string Desc {
                get { return "Left click to increment channel, right click to decrement."; }
            }

            public override string Label {
                get { return "channel " + teleporter.Channel; }
            }

            public ChannelGizmo(BeltTeleporterComponent teleporter) {
                this.teleporter = teleporter;

                icon = ContentFinder<Texture2D>.Get("UI/Commands/TempRaise");
            }

            public override void ProcessInput(Event ev) {
                base.ProcessInput(ev);

                if (ev.button == 0)
                    teleporter.Channel++;
                else if (ev.button == 1)
                    teleporter.Channel--;
            }
        }

        private Command channelGizmo;

        public BeltTeleporterComponent() {
            channelGizmo = new ChannelGizmo(this);
        }

        protected override void DoFreezeCheck()
        {
            // Teleporters / Receivers don't freeze.
        }

        public Vector3 GetRotationOffset() {
            var mySize = parent.RotatedSize.ToVector3();
            var offset = new Vector3(0.5f * (1.0f - mySize.x), 0,
                                     0.5f * (1.0f - mySize.z));

            switch (parent.Rotation.AsInt) {
                case 1: offset.z++; break;
                case 2: offset.x++; break;
            }

            return offset;
        }
        
        public override void PostExposeData() {
            base.PostExposeData();

            Scribe_Values.LookValue(ref _chan, "channel", 0, true);
        }

        public override IEnumerable<Command> CompGetGizmosExtra() {
            yield return channelGizmo;
        }

    }
}
