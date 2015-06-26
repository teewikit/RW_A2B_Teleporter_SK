using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace A2B_Teleport
{
	public class A2BTeleportDataDef : Def
	{
		// Teleporter data
		public int				TeleporterWattsPerCellBase;
		public int				TeleporterWattsPerCellOffset;
		public float			TeleporterDegreesPerCellBase;
		public float			TeleporterDegreesPerCellOffset;
		public List< string >	TeleporterResearch;

	}

}
