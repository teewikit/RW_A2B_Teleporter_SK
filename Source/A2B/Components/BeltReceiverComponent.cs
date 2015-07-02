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
            // Tell sender it's lost the lock
            if( Sender != null )
                Unlock( Sender );

            // Deregister from manager
            Manager.RemoveReceiver(this);

            base.PostDeSpawn();
        }

        public bool Lock(BeltSenderComponent sender) {
			if( ( locked )&&( Sender != sender ) )
				return false;
            locked = true;
            Sender = sender;
			return true;
        }

		public bool Unlock(BeltSenderComponent sender) {
			if( ( locked )&&( Sender != sender ) )
				return false;
			Sender.Released();
            Sender = null;
			locked = false;
			return true;
        }

		private void MasterKey() {
			Unlock( Sender );
		}

        public bool IsLocked() {
            if( ( locked == true )&&( Sender == null ) ){
                locked = false;
                Sender = null;
            }
            return locked;
        }

        public override void PostExposeData() {
            base.PostExposeData();

            Scribe_Values.LookValue(ref locked, "locked", false, true);

            IntVec3 pos = IntVec3.Invalid;

            if (Sender != null)
                pos = Sender.parent.Position;
            
            Scribe_Values.LookValue(ref pos, "sender", IntVec3.Invalid, true);

            if( ( Scribe.mode == LoadSaveMode.LoadingVars )&&
                ( pos != IntVec3.Invalid ) ){

                var sender = Manager.GetSenderAtPos( pos );

                if( sender == null ){
                    locked = false;
                    Sender = null;
                } else {
                    Sender = sender;
                }
            }
        }

        // We can accept from any teleporter on our channel. If we are locked, we can only accept from
        // the sender that locked us.
        public override bool CanAcceptFrom(BeltComponent belt, bool onlyCheckConnection = false) {
            if (!belt.IsTeleporter())
                return false;

            // If I can't accept from anyone, I certainly can't accept from you.
            if( !onlyCheckConnection && !CanAcceptSomething() )
                return false;

            BeltSenderComponent tele = belt as BeltSenderComponent;

            // Does whomever holds the lock still operate on the same channel?
            if( IsLocked() )
                return ( tele == Sender )&&( tele.Channel == Channel );

            return (tele.Channel == Channel);
        }

        protected override Vector3 GetOffset(ThingStatus status) {
            var direction = parent.Rotation.FacingCell;
            var progress = (float) status.Counter / A2BData.BeltSpeed.TicksToMove;

            // Comes in at the middle of the pad
            if( progress < 0.5f )
                progress = 0.5f;
            
            return GetRotationOffset() + direction.ToVector3() * ( progress - 0.5f );
        }
    }

}
