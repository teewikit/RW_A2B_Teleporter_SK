using System.Collections.Generic;
using Verse;

namespace A2B_Teleport
{
	public class A2BTeleportDataDef : Def
	{
		// Teleporter mod version requirement
		public string           RequiredVersion;
        
		// Power data
        public float            BasePowerConsumptionBase;
        public float            BasePowerConsumptionOffset;

		public float			WattsPerCellBase;
        public float			WattsPerCellOffset;

		public List<string>	    PowerResearch;

		// Temperature data
        public float			DegreesPerCellBase;
		public float			DegreesPerCellOffset;

        public List<string>	    HeatResearch;

		// Teleporter Channels data
        public int              NumTeleporterChannelsBase;
        public int              NumTeleporterChannelsOffset;
		public List<string>	    ChannelsResearch;

		// Teleport Speed data
        public float            SpeedMultiplierBase;
        public float            SpeedMultiplierOffset;
		public List<string>	    SpeedResearch;

	}

}
