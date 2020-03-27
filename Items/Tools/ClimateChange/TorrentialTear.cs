﻿using CalamityMod.Events;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools.ClimateChange
{
    public class TorrentialTear : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Torrential Tear");
            Tooltip.SetDefault("Summons the rain.\n" +
                "Rain will start some time after this item is used.\n" +
                "If used while it's raining, the rain will stop some time afterward.\n" +
				"In Death Mode, using this item while it's raining will reduce the amount of time the rain lingers for\n" +
				"to one minute; however, not any lower, and this will cause the rain to turn violent for that minute.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = 5;
            item.useAnimation = 20;
            item.useTime = 20;
            item.useStyle = 4;
            item.UseSound = SoundID.Item66;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.slimeRain && (Main.rainTime > 3600 || !CalamityWorld.death || !Main.raining);
        }

        public override bool UseItem(Player player)
        {
            if (!Main.raining)
            {
				CalamityUtils.StartRain(true);
            }
            else if (CalamityWorld.death)
            {
                if (Main.rainTime > 3600)
                {
                    Main.rainTime = 3600;
                    AdjustRainSeverity(true);
                }
                if (CalamityWorld.rainingAcid)
                {
                    CalamityWorld.acidRainPoints = 0;
                    CalamityWorld.triedToSummonOldDuke = false;
                    AcidRainEvent.UpdateInvasion(false);
                }
            }
            else
            {
                Main.raining = false;
                if (CalamityWorld.rainingAcid)
                {
                    CalamityWorld.acidRainPoints = 0;
                    CalamityWorld.triedToSummonOldDuke = false;
                    AcidRainEvent.UpdateInvasion(false);
                }
            }

            CalamityMod.UpdateServerBoolean();
            return true;
        }

		public static void AdjustRainSeverity(bool maxSeverity)
		{
			if (maxSeverity)
			{
				Main.cloudBGActive = 1f;
				Main.numCloudsTemp = Main.cloudLimit;
				Main.numClouds = Main.numCloudsTemp;
				Main.windSpeedTemp = (float)Main.rand.Next(50, 75) * 0.01f;
				Main.windSpeedSet = Main.windSpeedTemp;
				Main.weatherCounter = Main.rand.Next(3600, 18000);
				Main.maxRaining = 0.89f;
			}
			else
			{
				if (Main.cloudBGActive >= 1f || (double)Main.numClouds > 150.0)
				{
					if (Main.rand.Next(3) == 0)
						Main.maxRaining = (float)Main.rand.Next(20, 90) * 0.01f;
					else
						Main.maxRaining = (float)Main.rand.Next(40, 90) * 0.01f;
				}
				else if ((double)Main.numClouds > 100.0)
				{
					if (Main.rand.Next(3) == 0)
						Main.maxRaining = (float)Main.rand.Next(10, 70) * 0.01f;
					else
						Main.maxRaining = (float)Main.rand.Next(20, 60) * 0.01f;
				}
				else
				{
					if (Main.rand.Next(3) == 0)
						Main.maxRaining = (float)Main.rand.Next(5, 40) * 0.01f;
					else
						Main.maxRaining = (float)Main.rand.Next(5, 30) * 0.01f;
				}
			}
		}
    }
}
