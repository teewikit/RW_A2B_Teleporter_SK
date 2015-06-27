using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Verse;
using UnityEngine;
using A2B;
using RimWorld;

using Verse.Sound;

using Random = UnityEngine.Random;

namespace A2B_Teleport {

    using Manager = A2BTeleporterManager;

    public class BeltSenderComponent : BeltTeleporterComponent {

        public override void PostSpawnSetup() {
            base.PostSpawnSetup();

            Manager.AddSender(this);
        }

        public override void PostDeSpawn() {
            Manager.RemoveSender(this);

            base.PostDeSpawn();
        }

        protected override void PostItemContainerTick() {
            base.PostItemContainerTick();

            if (!ItemContainer.WorkToDo || Random.Range(0.0f, 1.0f) > 0.15f)
                return;

            MoteThrower.ThrowLightningGlow(parent.DrawPos + 0.5f * GetRotationOffset(), Random.Range(0.2f, 0.4f));
        }

        public override IntVec3 GetDestinationForThing(Thing thing) {
            var pos = parent.Position.ToVector3();

            // Get only only receivers on my channel and return them in order of
            // closest to furthest.
            var recv = Manager.GetReceivers(Channel)
                              .Where(r => !r.IsLocked() || r.Sender == this)
                              .OrderBy(r => (r.parent.Position.ToVector3() - pos).sqrMagnitude);

            try {
                // This will be the closest unlocked receiver.
                var target = recv.First();
                return target.parent.Position;
            } catch (InvalidOperationException) {
                // If nothing was acceptable, return IntVec3.Invalid
                return IntVec3.Invalid;
            }
        }

        // We'll use OnBeginMove to lock the receiver until the item finishes transferring.
        public override void OnBeginMove(Thing thing, IntVec3 dest) {
            var recv = Manager.GetReceiverAtPos(dest);
            if (recv != null) {
                recv.Lock(this);
            }

            float dist = (float) Math.Sqrt(dest.DistanceToSquared(parent.Position));
            float power = dist * A2BTeleportData.WattsPerCell;

            PowerComponent.PowerOutput = A2BTeleportData.BasePowerConsumption + power;

            base.OnBeginMove(thing, dest);
        }

        // Unlock the receiver after the item finishes.
        public override void OnItemTransfer(Thing item, BeltComponent other) {
            base.OnItemTransfer(item, other);

            BeltReceiverComponent recv = other as BeltReceiverComponent;
            recv.Unlock();

            int minPuffs = (A2BTeleportData.IsResearched ? 1 : 4);
            int maxPuffs = (A2BTeleportData.IsResearched ? 2 : 6);

            PowerComponent.PowerOutput = A2BTeleportData.BasePowerConsumption;

            float dist = (float) Math.Sqrt(other.parent.Position.DistanceToSquared(parent.Position));
            float heat = dist * A2BTeleportData.DegreesPerCell;

            Room room = GridsUtility.GetRoom(parent.Position);
            if (room != null)
                room.PushHeat(heat);

            for (int i = 0; i < Rand.RangeInclusive(minPuffs, maxPuffs); ++i)
                MoteThrower.ThrowAirPuffUp(parent.DrawPos);

            room = GridsUtility.GetRoom(recv.parent.Position);
            if (room != null)
                room.PushHeat(heat);

            for (int i = 0; i < Rand.RangeInclusive(minPuffs, maxPuffs); ++i)
                MoteThrower.ThrowAirPuffUp(Gen.TrueCenter(recv.parent.Position, parent.Rotation, new IntVec2(2, 1), 0.5f));

            MoteThrower.ThrowLightningGlow(parent.DrawPos, Random.Range(4.8f, 5.2f));
            MoteThrower.ThrowLightningGlow(recv.parent.DrawPos, Random.Range(4.8f, 5.2f));

            var sound = DefDatabase<SoundDef>.GetNamed("A2B_Teleport");
            sound.PlayOneShot(SoundInfo.InWorld(parent.Position));
            sound.PlayOneShot(SoundInfo.InWorld(recv.parent.Position));
        }

       protected override Vector3 GetOffset(ThingStatus status) {
            IntVec3 midDirection;

            if (ThingOrigin != IntVec3.Invalid) {
                midDirection = parent.Position + parent.Rotation.FacingCell - ThingOrigin;
            } else {
                midDirection = parent.Rotation.FacingCell;
            }

            var progress = (float) status.Counter / BeltSpeed;

            var mid = midDirection.ToVector3();

            return GetRotationOffset() + mid.normalized * (progress * 0.75f - 0.5f);
        }
    }
}
