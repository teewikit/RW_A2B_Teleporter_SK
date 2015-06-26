using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;
using A2B;
using A2B_Teleport;

namespace A2B_Teleport
{

	public struct A2B_Teleportation
	{
		public bool isResearched;
		public int WattsPerCell;
		public float DegreesPerCell;
	}

	public static class A2BTeleportData
	{

		public static A2BTeleportDataDef	def;

		public static A2B_Teleportation		Teleportation;

		static A2BTeleportData ()
		{
			def = DefDatabase< A2BTeleportDataDef >.GetNamed( "A2BTeleport" );
			if( def == null )
			{
				Log.ErrorOnce( "A2B_Teleport - Unable to load teleport data!", 0 );
				return;
			}

			Teleportation.isResearched = false;
			Teleportation.WattsPerCell = def.TeleporterWattsPerCellBase;
			Teleportation.DegreesPerCell = def.TeleporterDegreesPerCellBase;

		}

		public static bool IsReady
		{
			get{ return ( def != null ); }
		}

		public static bool AllResearchDone
		{
			get
			{
				if( Teleportation.isResearched )
					return true;
				return false;
			}
		}

	}

	public class A2BTeleportMonitor : MapComponent
	{
		public override void MapComponentTick()
		{
			base.MapComponentTick();

			if( !A2BTeleportData.IsReady )
				return;

			if( A2BTeleportData.AllResearchDone )
				return;

			if( ( A2BTeleportData.Teleportation.isResearched == false )&&
				( A2BResearch.ResearchGroupComplete( A2BTeleportData.def.TeleporterResearch ) ) )
			{
				A2BTeleportData.Teleportation.WattsPerCell += A2BTeleportData.def.TeleporterWattsPerCellOffset;
				A2BTeleportData.Teleportation.DegreesPerCell += A2BTeleportData.def.TeleporterDegreesPerCellOffset;
				A2BTeleportData.Teleportation.isResearched = true;
			}

		}
	}
}
