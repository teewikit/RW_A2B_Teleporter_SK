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

namespace A2B_Teleport
{

    using Manager = A2BTeleporterManager;

    public enum TeleportState
    {
        Default = 0,
        Teleporting
    }

    public class BeltSenderComponent : BeltTeleporterComponent
    {

        public TeleportState CurrentState = TeleportState.Default;

        private BeltReceiverComponent Receiver;

        public override int BeltSpeed{
            //get { return (int) (A2BData.BeltSpeed.TicksToMove / A2BTeleportData.Speed.SpeedMultiplier); }
            get{
                if( CurrentState == TeleportState.Default )
                    return A2BData.BeltSpeed.TicksToMove;
                return ( int )( A2BData.BeltSpeed.TicksToMove / A2BTeleportData.Speed.SpeedMultiplier );
            }
        }

        // Receiver callback
        public void Released()
        {
            // Receiver has released our lock
            Receiver = null;
        }

        // Sender target lock
        public bool LockTarget( BeltReceiverComponent targ )
        {
            // Unlock old target
            if( ( Receiver != null )&&( targ != Receiver ) )
                Receiver.Unlock( this );

            // Lock new target
            if( targ.Lock( this ) ){
                Receiver = targ;
                return true;
            }
            return false;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.LookValue( ref CurrentState, "CurrentState", TeleportState.Default, true );

            if( CurrentState == TeleportState.Default ){
                Receiver = null;
            } else {

                IntVec3 pos = IntVec3.Invalid;

                if( Receiver != null )
                    pos = Receiver.parent.Position;

                Scribe_Values.LookValue( ref pos, "receiver", IntVec3.Invalid, true );

                if( pos == IntVec3.Invalid ){
                    Receiver = null;
                } else {
                    Receiver = Manager.GetReceiverAtPos( pos );
                }
            }
        }

        public override void PostSpawnSetup()
        {
            base.PostSpawnSetup();

            Manager.AddSender( this );
        }

        public override void PostDeSpawn()
        {
            // Unlock anyone we're teleporting to
            if( Receiver != null )
                Receiver.Unlock( this );

            // Deregister from manager
            Manager.RemoveSender( this );

            base.PostDeSpawn();
        }

        protected override void PostItemContainerTick()
        {
            base.PostItemContainerTick();

            // Keep loading item
            if( CurrentState == TeleportState.Default ){
                // Wait until fully loaded
                foreach( var status in ItemContainer.ThingStatus ){
                    var progress = ( float )status.Counter / BeltSpeed;
                    if( progress > 0.5f ){
                        CurrentState = TeleportState.Teleporting;
                    }
                }
                return;
            }

            // No target
            if( Receiver == null ){
                PowerComponent.PowerOutput = -PowerComponent.props.basePowerConsumption;
                return;
            }

            // Adjust power
            float dist = ( float )Math.Sqrt( Receiver.parent.Position.DistanceToSquared( parent.Position ) );
            float power = dist * A2BTeleportData.Power.WattsPerCell;

            PowerComponent.PowerOutput = -( A2BTeleportData.Power.BasePowerConsumption + power );

            // Push heat and throw motes until teleport complete
            if( Gen.IsHashIntervalTick( this.parent, 30 ) ){
                Room room = GridsUtility.GetRoom( parent.Position );
                if( room != null ){
                    //var dist = (float) Math.Sqrt(Receiver.parent.Position.DistanceToSquared(parent.Position));
                    float heat = dist * A2BTeleportData.Heat.DegreesPerCell;
                    room.PushHeat( heat );
                }

                int minPuffs = ( A2BTeleportData.Heat.isResearched ? 2 : 4 );
                int maxPuffs = ( A2BTeleportData.Heat.isResearched ? 2 : 6 );

                for( int i = 0; i < Rand.RangeInclusive( minPuffs, maxPuffs ); ++i )
                    MoteThrower.ThrowAirPuffUp( parent.DrawPos );

                for( int i = 0; i < Rand.RangeInclusive( minPuffs, maxPuffs ); ++i )
                    MoteThrower.ThrowAirPuffUp( Gen.TrueCenter( Receiver.parent.Position, parent.Rotation, new IntVec2( 2, 1 ), 0.5f ) );

            }

            if( !ItemContainer.WorkToDo || Random.Range( 0.0f, 1.0f ) > 0.15f )
                return;

            MoteThrower.ThrowLightningGlow( parent.DrawPos + 0.5f * GetRotationOffset(), Random.Range( 0.2f, 0.4f ) );
        }

        // Look for a receiver and lock it.
        public override IntVec3 GetDestinationForThing( Thing thing )
        {
            // Already targetted
            if( ( Receiver != null )&&
                ( Receiver.CanAcceptSomething() ) )
                return Receiver.parent.Position;

            // Find a new target
            var pos = parent.Position.ToVector3();

            // Get only only receivers on my channel and return them in order of
            // closest to furthest.
            var recv = Manager.GetReceivers( Channel )
                              .Where( r => r.CanAcceptFrom( this ) )
                              .OrderBy( r => ( r.parent.Position.ToVector3() - pos ).sqrMagnitude );

            try{
                // This will be the closest unlocked receiver, try to lock it
                var target = recv.First();
                if( LockTarget( target ) ){
                    return target.parent.Position;
                } else{
                    return IntVec3.Invalid;
                }

            } catch( InvalidOperationException ){
                // If nothing was acceptable, return IntVec3.Invalid
                return IntVec3.Invalid;
            }
        }


        // Unlock the receiver after the item finishes.
        public override void OnItemTransfer( Thing item, BeltComponent other )
        {
            base.OnItemTransfer( item, other );

            BeltReceiverComponent recv = other as BeltReceiverComponent;

            // Play a nice teleport sound
            var sound = DefDatabase<SoundDef>.GetNamed( "A2B_Teleport" );
            sound.PlayOneShot( SoundInfo.InWorld( parent.Position ) );
            sound.PlayOneShot( SoundInfo.InWorld( recv.parent.Position ) );

            // And blind anyone who's looking
            MoteThrower.ThrowLightningGlow( parent.DrawPos, Random.Range( 4.8f, 5.2f ) );
            MoteThrower.ThrowLightningGlow( Receiver.parent.DrawPos, Random.Range( 4.8f, 5.2f ) );

            // Reset power usage and mode
            PowerComponent.PowerOutput = -PowerComponent.props.basePowerConsumption;
            CurrentState = TeleportState.Default;
            Receiver.Unlock( this );
        }

        public override void PostDraw()
        {
            base.PostDraw();

            foreach( var status in ItemContainer.ThingStatus ){
                var drawPos = parent.DrawPos + GetOffset( status ) + Altitudes.AltIncVect * Altitudes.AltitudeFor( AltitudeLayer.Item );

                status.Thing.DrawAt( drawPos );

                DrawGUIOverlay( status, drawPos );
            }
        }

        protected override Vector3 GetOffset( ThingStatus status )
        {
            IntVec3 midDirection;

            if( ThingOrigin != IntVec3.Invalid ){
                midDirection = parent.Position + parent.Rotation.FacingCell - ThingOrigin;
            } else{
                midDirection = parent.Rotation.FacingCell;
            }

            float progress = 0.5f;
            if( CurrentState == TeleportState.Default ){
                progress = ( float )status.Counter / BeltSpeed;
                // No further than the middle of the pad
                if( progress > 0.5f )
                    progress = 0.5f;
            }

            var mid = midDirection.ToVector3();

            return GetRotationOffset() + mid.normalized * ( progress - 0.5f );
        }
    }
}
