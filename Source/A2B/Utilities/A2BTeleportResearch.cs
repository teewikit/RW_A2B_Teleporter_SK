using Verse;
using A2B;
using System;

namespace A2B_Teleport {

	public struct A2B_TeleportPower
	{
        public bool     isResearched;
        public float    BasePowerConsumption;
        public float    WattsPerCell;
	}

	public struct A2B_TeleportHeat
	{
        public bool     isResearched;
        public float    DegreesPerCell;
	}

	public struct A2B_TeleportChannels
	{
        public bool     isResearched;
        public int      NumTeleporterChannels;
	}

	public struct A2B_TeleportSpeed
	{
        public bool     isResearched;
        public float    SpeedMultiplier;
	}

    public static class A2BTeleportData {

        private static A2BTeleportDataDef	_def;

		public static Version               RequiredVersion;

		public static A2B_TeleportPower     Power;
		public static A2B_TeleportHeat      Heat;
		public static A2B_TeleportChannels  Channels;
		public static A2B_TeleportSpeed     Speed;

		public static A2BTeleportDataDef def
		{
			get {
				if( _def == null )
					_def = DefDatabase<A2BTeleportDataDef>.GetNamed("A2BTeleport");
				if( _def == null )
					_def = DefDatabase<A2BTeleportDataDef>.GetRandom();
				return _def;
			}
		}

        static A2BTeleportData() {
			if( !A2BData.IsReady )
			{
				Log.ErrorOnce( "A2B Core not ready!", 0 );
				return;
			}
            if (def == null) {
				Log.ErrorOnce( "A2B - Unable to load teleport data!", 0 );
				return;
			}

			RequiredVersion = new Version(def.RequiredVersion);
			A2BData.CheckVersion(RequiredVersion);

            Power.isResearched = false;
			Power.BasePowerConsumption = def.BasePowerConsumptionBase;
			Power.WattsPerCell = def.WattsPerCellBase;

            Heat.isResearched = false;
			Heat.DegreesPerCell = def.DegreesPerCellBase;

            Channels.isResearched = false;
            Channels.NumTeleporterChannels = def.NumTeleporterChannelsBase;

            Speed.isResearched = false;
			Speed.SpeedMultiplier = def.SpeedMultiplierBase;
            
			A2BMonitor.RegisterOccasionalAction( "A2BTeleportResearch.Power", A2BTeleportResearch.Power );
			A2BMonitor.RegisterOccasionalAction( "A2BTeleportResearch.Heat", A2BTeleportResearch.Heat );
			A2BMonitor.RegisterOccasionalAction( "A2BTeleportResearch.Channels", A2BTeleportResearch.Channels );
			A2BMonitor.RegisterOccasionalAction( "A2BTeleportResearch.Speed", A2BTeleportResearch.Speed );

			Log.Message( "A2B_Teleport initialized" );

        }

		public static bool IsReady
		{
			get { return def != null; }
		}

    }

	public static class A2BTeleportResearch
	{

		public static bool Power()
		{
			if (A2BResearch.ResearchGroupComplete(A2BTeleportData.def.PowerResearch)) {
				A2BTeleportData.Power.BasePowerConsumption += A2BTeleportData.def.BasePowerConsumptionOffset;
				A2BTeleportData.Power.WattsPerCell += A2BTeleportData.def.WattsPerCellOffset;
                A2BTeleportData.Power.isResearched = true;
				return true;
			}
			return false;
		}

		public static bool Heat()
		{
			if (A2BResearch.ResearchGroupComplete(A2BTeleportData.def.HeatResearch)) {
				A2BTeleportData.Heat.DegreesPerCell += A2BTeleportData.def.DegreesPerCellOffset;
                A2BTeleportData.Heat.isResearched = true;
				return true;
			}
			return false;
		}

		public static bool Channels()
		{
			if (A2BResearch.ResearchGroupComplete(A2BTeleportData.def.ChannelsResearch)) {
				A2BTeleportData.Channels.NumTeleporterChannels += A2BTeleportData.def.NumTeleporterChannelsOffset;
                A2BTeleportData.Channels.isResearched = true;
				return true;
			}
			return false;
		}

		public static bool Speed()
		{
			if (A2BResearch.ResearchGroupComplete(A2BTeleportData.def.SpeedResearch)) {
				A2BTeleportData.Speed.SpeedMultiplier += A2BTeleportData.def.SpeedMultiplierOffset;
                A2BTeleportData.Speed.isResearched = true;
				return true;
			}
			return false;
		}
	}

}