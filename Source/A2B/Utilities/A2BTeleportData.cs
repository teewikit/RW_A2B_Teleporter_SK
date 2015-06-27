using System.Collections.Generic;
using Verse;

namespace A2B_Teleport
{
	public class A2BTeleportDataDef : Def
	{
        // Teleporter data
        public float            BasePowerConsumptionBase;
        public float            BasePowerConsumptionOffset;

		public int				WattsPerCellBase;
        public int				WattsPerCellOffset;

        public float			DegreesPerCellBase;
		public float			DegreesPerCellOffset;

        public List<string>	    Research;

        public int              NumTeleporterChannelsBase;
        public int              NumTeleporterChannelsOffset;

        public float            SpeedMultiplierBase;
        public float            SpeedMultiplierOffset;

        public string           RequiredVersion;
	}

}
