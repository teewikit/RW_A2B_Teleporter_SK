﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Verse;
using RimWorld;
using A2B;
using A2B_Teleport;

namespace A2B_Teleport
{
    public class Building_Teleporter : Building
    {

        public override Graphic Graphic
        {
            get
            {
                BeltComponent belt = GetComp<BeltComponent>();

                if (belt.BeltPhase == Phase.Active)
                    return base.Graphic;

                AnimatedGraphic animation = (AnimatedGraphic)base.Graphic;
                return animation.DefaultGraphic;
            }
        }

        private int prevFrame = 0;

        public override void SpawnSetup()
        {
            base.SpawnSetup();

            AnimatedGraphic animation = (AnimatedGraphic) base.Graphic;
            animation.DefaultFrame = 11;
        }

        public override void Tick()
        {
            base.Tick();

            CompPowerTrader power = GetComp<CompPowerTrader>();
            BeltComponent belt = GetComp<BeltComponent>();

            if (Graphic.GetType() == typeof(AnimatedGraphic))
            {
                AnimatedGraphic animation = (AnimatedGraphic)Graphic;

                if (animation.CurrentFrame != prevFrame)
                {
					Find.MapDrawer.MapMeshDirty( Position, MapMeshFlag.Things, true, false);
                    prevFrame = animation.CurrentFrame;
                }
            }
        }
    }
}
