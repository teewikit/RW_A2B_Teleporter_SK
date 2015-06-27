using Verse;
using A2B;
using System;

namespace A2B_Teleport {

    public static class A2BTeleportData {

        public static A2BTeleportDataDef def;

        public static bool  IsResearched;
        public static int   WattsPerCell;
        public static float DegreesPerCell;

        public static float BasePowerConsumption;

        public static int   NumTeleporterChannels;

        public static float SpeedMultiplier;

        public static Version RequiredVersion;

        static A2BTeleportData() {
            def = DefDatabase<A2BTeleportDataDef>.GetNamed("A2BTeleport", false);

            if (def == null) {
                Log.ErrorOnce("A2B_Teleport - Unable to load teleport data!", 0);
                return;
            }

            IsResearched = false;
            WattsPerCell = def.WattsPerCellBase;
            DegreesPerCell = def.DegreesPerCellBase;
            BasePowerConsumption = def.BasePowerConsumptionBase;
            NumTeleporterChannels = def.NumTeleporterChannelsBase;
            SpeedMultiplier = def.SpeedMultiplierBase;
            RequiredVersion = new Version(def.RequiredVersion);

            A2BData.CheckVersion(RequiredVersion);
        }

        public static bool IsReady {
            get { return (def != null); }
        }

        public static bool AllResearchDone {
            get { return IsResearched; }
        }

        public static void ApplyOffsets() {
            WattsPerCell         += def.WattsPerCellOffset;
            BasePowerConsumption += def.BasePowerConsumptionOffset;
            DegreesPerCell       += def.DegreesPerCellOffset;
        }

        public static bool ResearchGroupComplete {
            get {
                return A2BResearch.ResearchGroupComplete(def.Research);
            }
        }

    }

    public class A2BTeleportMonitor : MapComponent {

        public override void MapComponentTick() {
            base.MapComponentTick();

            if (!A2BTeleportData.IsReady || A2BTeleportData.AllResearchDone)
                return;

            if (!A2BTeleportData.IsResearched && A2BTeleportData.ResearchGroupComplete) {
                A2BTeleportData.ApplyOffsets();
            }
        }

    }
}
