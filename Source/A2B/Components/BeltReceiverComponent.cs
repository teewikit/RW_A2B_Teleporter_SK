using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using A2B;
using UnityEngine;
using Verse;
using RimWorld;

namespace A2B_Teleport {
    using Manager = A2BTeleporterManager;

    public class BeltReceiverComponent : BeltTeleporterComponent {

        private bool locked = false;
        public BeltSenderComponent Sender { get; protected set; }

        public override void PostSpawnSetup() {
            base.PostSpawnSetup();

            Manager.AddReceiver(this);
        }

        public override void PostDeSpawn() {
            Manager.RemoveReceiver(this);

            base.PostDeSpawn();
        }

        public void Lock(BeltSenderComponent sender) {
            locked = true;
            Sender = sender;
        }

        public void Unlock() {
            locked = false;
            Sender = null;
        }

        public bool IsLocked() {
            return locked;
        }

        public override void PostExposeData() {
            base.PostExposeData();

            Scribe_Values.LookValue(ref locked, "locked", false, true);

            IntVec3 pos = IntVec3.Invalid;

            if (Sender != null)
                pos = Sender.parent.Position;
            
            Scribe_Values.LookValue(ref pos, "sender", IntVec3.Invalid, true);

            if (pos == IntVec3.Invalid) {
                Unlock();
            } else {
                Sender = Manager.GetSenderAtPos(pos);
            }
        }

        // We can accept from any teleporter on our channel. If we are locked, we can only accept from
        // the sender that locked us.
        public override bool CanAcceptFrom(BeltComponent belt, bool onlyCheckConnection = false) {
            if (!belt.IsTeleporter())
                return false;

            BeltSenderComponent tele = belt as BeltSenderComponent;

            if (IsLocked() && Sender != null)
                return (tele == Sender);

            Unlock();

            return (tele.Channel == Channel);
        }

        protected override Vector3 GetOffset(ThingStatus status) {
            var direction = parent.Rotation.FacingCell;
            var progress = (float) status.Counter / A2BData.BeltSpeed.TicksToMove;
            return GetRotationOffset() + direction.ToVector3() * progress * 0.5f;
        }
    }

}
