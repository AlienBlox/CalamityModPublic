﻿using CalamityMod.Items.Accessories;
using CalamityMod.Items.Fishing;
using CalamityMod.Items.Fishing.AstralCatches;
using CalamityMod.Items.Fishing.BrimstoneCragCatches;
using CalamityMod.Items.Fishing.FishingRods;
using CalamityMod.Items.Fishing.SulphurCatches;
using CalamityMod.Items.Fishing.SunkenSeaCatches;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer
{
    public class CalamityPlayerFishing
    {
        #region Catch Fish
        public static void CalamityCatchFish(Player player, ref Item fishingRod, ref Item bait, ref int power, ref int liquidType, ref int poolSize,
			ref int worldLayer, ref int questFish, ref int caughtType, ref bool junk)
        {
            CalamityPlayer modPlayer = player.Calamity();

			bool water = liquidType == 0;
			bool lava = liquidType == 1;
			bool honey = liquidType == 2;

			Point point = player.Center.ToTileCoordinates();
			bool abyssPosX = false;
			if (CalamityWorld.abyssSide)
			{
				if (point.X < 380)
					abyssPosX = true;
			}
			else
			{
				if (point.X > Main.maxTilesX - 380)
					abyssPosX = true;
			}

			if (modPlayer.ZoneAbyss || modPlayer.ZoneSulphur)
				abyssPosX = true;

			if (modPlayer.alluringBait)
			{
				int chanceForPotionFish = 1000 / power;

				if (chanceForPotionFish < 3)
					chanceForPotionFish = 3;

				if (Main.rand.NextBool(chanceForPotionFish))
				{
					List<int> fishList = new List<int>();

					if (lava)
					{
						if (!modPlayer.ZoneCalamity)
						{
							fishList.Add(ItemID.FlarefinKoi);
							fishList.Add(ItemID.Obsidifish);
						}
						if (modPlayer.ZoneCalamity)
						{
							fishList.Add(ModContent.ItemType<CoastalDemonfish>());
						}
					}
					else if (water)
					{
						if (player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight)
						{
							fishList.Add(ItemID.ArmoredCavefish);

							if (player.ZoneHoly)
							{
								fishList.Add(ItemID.ChaosFish);
							}
							if (player.ZoneJungle)
							{
								fishList.Add(ItemID.VariegatedLardfish);
							}
						}
						if (player.ZoneSnow)
						{
							fishList.Add(ItemID.FrostMinnow);
						}
						if (player.ZoneCorrupt)
						{
							fishList.Add(ItemID.Ebonkoi);
						}
						if (player.ZoneCrimson)
						{
							fishList.Add(ItemID.CrimsonTigerfish);
							fishList.Add(ItemID.Hemopiranha);
						}
						if (player.ZoneHoly)
						{
							fishList.Add(ItemID.PrincessFish);
							fishList.Add(ItemID.Prismite);
						}
						if (player.ZoneSkyHeight)
						{
							fishList.Add(ItemID.Damselfish);
						}
						if (player.ZoneJungle)
						{
							if (player.ZoneOverworldHeight || player.ZoneSkyHeight)
							{
								fishList.Add(ItemID.DoubleCod);
							}
						}
						if (modPlayer.ZoneAstral)
						{
							fishList.Add(ModContent.ItemType<AldebaranAlewife>());
						}
						if (modPlayer.ZoneSunkenSea)
						{
							fishList.Add(ModContent.ItemType<CoralskinFoolfish>());
							fishList.Add(ModContent.ItemType<SunkenSailfish>());
							fishList.Add(ModContent.ItemType<ScarredAngelfish>());
						}
					}

					if (fishList.Any())
					{
						int fishAmt = fishList.Count;
						int caughtFish = fishList[Main.rand.Next(fishAmt)];
						caughtType = caughtFish;
					}
				}
			}

			if (modPlayer.enchantedPearl || modPlayer.fishingStation)
			{
				int chanceForCrates = (modPlayer.enchantedPearl ? 10 : 0) +
					(modPlayer.fishingStation ? 10 : 0);

				int poolSizeAmt = poolSize / 10;
				if (poolSizeAmt > 100)
					poolSizeAmt = 100;

				int fishingPowerDivisor = power + poolSizeAmt;

				int chanceForIronCrate = 1000 / fishingPowerDivisor;
				int chanceForBiomeCrate = 2000 / fishingPowerDivisor;
				int chanceForGoldCrate = 3000 / fishingPowerDivisor;
				int chanceForRareItems = 4000 / fishingPowerDivisor;

				if (chanceForIronCrate < 3)
					chanceForIronCrate = 3;

				if (chanceForBiomeCrate < 4)
					chanceForBiomeCrate = 4;

				if (chanceForGoldCrate < 5)
					chanceForGoldCrate = 5;

				if (chanceForRareItems < 6)
					chanceForRareItems = 6;

				if (lava)
				{
					if (Main.rand.Next(100) < chanceForCrates)
					{
						if (Main.rand.NextBool(chanceForRareItems) && modPlayer.enchantedPearl && modPlayer.fishingStation && player.cratePotion)
						{
							if (modPlayer.ZoneCalamity)
								caughtType = ModContent.ItemType<BrimstoneCrate>();
						}
					}
				}

				if (water)
				{
					if (Main.rand.Next(100) < chanceForCrates)
					{
						if (Main.rand.NextBool(chanceForRareItems) && modPlayer.enchantedPearl && modPlayer.fishingStation && player.cratePotion)
						{
							List<int> rareItemList = new List<int>();

							if (abyssPosX)
							{
								switch (Main.rand.Next(4))
								{
									case 0:
										rareItemList.Add(ModContent.ItemType<IronBoots>());
										break;
									case 1:
										rareItemList.Add(ModContent.ItemType<DepthCharm>());
										break;
									case 2:
										rareItemList.Add(ModContent.ItemType<AnechoicPlating>());
										break;
									case 3:
										rareItemList.Add(ModContent.ItemType<StrangeOrb>());
										break;
								}
							}
							if (modPlayer.ZoneAstral)
							{
								switch (Main.rand.Next(3))
								{
									case 0:
										rareItemList.Add(ModContent.ItemType<GacruxianMollusk>());
										break;
									case 1:
										rareItemList.Add(ModContent.ItemType<PolarisParrotfish>());
										break;
									case 2:
										rareItemList.Add(ModContent.ItemType<UrsaSergeant>());
										break;
								}
							}
							if (modPlayer.ZoneSunkenSea)
							{
								switch (Main.rand.Next(2))
								{
									case 0:
										rareItemList.Add(ModContent.ItemType<SerpentsBite>());
										break;
									case 1:
										rareItemList.Add(ModContent.ItemType<RustedJingleBell>());
										break;
								}
							}
							if (player.ZoneSnow && player.ZoneRockLayerHeight && (player.ZoneCorrupt || player.ZoneCrimson || player.ZoneHoly))
							{
								rareItemList.Add(ItemID.ScalyTruffle);
							}
							if (player.ZoneCorrupt)
							{
								rareItemList.Add(ItemID.Toxikarp);
							}
							if (player.ZoneCrimson)
							{
								rareItemList.Add(ItemID.Bladetongue);
							}
							if (player.ZoneHoly)
							{
								rareItemList.Add(ItemID.CrystalSerpent);
							}

							if (rareItemList.Any())
							{
								int rareItemAmt = rareItemList.Count;
								int caughtRareItem = rareItemList[Main.rand.Next(rareItemAmt)];
								caughtType = caughtRareItem;
							}
						}
						else if (Main.rand.NextBool(chanceForGoldCrate))
						{
							caughtType = ItemID.GoldenCrate;
						}
						else if (Main.rand.NextBool(chanceForBiomeCrate))
						{
							List<int> biomeCrateList = new List<int>();

							if (modPlayer.ZoneAstral)
							{
								biomeCrateList.Add(ModContent.ItemType<AstralCrate>());
							}
							if (modPlayer.ZoneSunkenSea)
							{
								biomeCrateList.Add(ModContent.ItemType<SunkenCrate>());
							}
							if (abyssPosX)
							{
								biomeCrateList.Add(ModContent.ItemType<AbyssalCrate>());
							}
							if (player.ZoneCorrupt)
							{
								biomeCrateList.Add(ItemID.CorruptFishingCrate);
							}
							if (player.ZoneCrimson)
							{
								biomeCrateList.Add(ItemID.CrimsonFishingCrate);
							}
							if (player.ZoneHoly)
							{
								biomeCrateList.Add(ItemID.HallowedFishingCrate);
							}
							if (player.ZoneDungeon)
							{
								biomeCrateList.Add(ItemID.DungeonFishingCrate);
							}
							if (player.ZoneJungle)
							{
								biomeCrateList.Add(ItemID.JungleFishingCrate);
							}
							if (player.ZoneSkyHeight)
							{
								biomeCrateList.Add(ItemID.FloatingIslandFishingCrate);
							}


							if (biomeCrateList.Any())
							{
								int biomeCrateAmt = biomeCrateList.Count;
								int caughtBiomeCrate = biomeCrateList[Main.rand.Next(biomeCrateAmt)];
								caughtType = caughtBiomeCrate;
							}
						}
						else if (Main.rand.NextBool(chanceForIronCrate))
						{
							caughtType = ItemID.IronCrate;
						}
						else
						{
							caughtType = ItemID.WoodenCrate;
						}
						return;
					}
				}
			}

			if (water)
			{
				if ((player.ZoneCrimson || player.ZoneCorrupt) && player.ZoneRockLayerHeight && Main.hardMode)
				{
					if (Main.rand.NextBool(15))
					{
						caughtType = ModContent.ItemType<FishofNight>();
					}
				}

				if (player.ZoneHoly && player.ZoneRockLayerHeight && Main.hardMode)
				{
					if (Main.rand.NextBool(15))
					{
						caughtType = ModContent.ItemType<FishofLight>();
					}
				}

				if (player.ZoneSkyHeight && Main.hardMode)
				{
					if (Main.rand.NextBool(15))
					{
						caughtType = ModContent.ItemType<FishofFlight>();
					}
					else if (Main.rand.NextBool(14))
					{
						caughtType = ModContent.ItemType<SunbeamFish>();
					}
				}

				if (player.ZoneOverworldHeight && !Main.dayTime)
				{
					if (Main.rand.NextBool(10))
					{
						caughtType = ModContent.ItemType<EnchantedStarfish>();
					}
				}

				if (player.ZoneOverworldHeight && Main.dayTime)
				{
					if (Main.rand.NextBool(15))
					{
						caughtType = ModContent.ItemType<StuffedFish>();
					}
				}

				if (player.ZoneRockLayerHeight)
				{
					if (Main.rand.NextBool(15))
					{
						caughtType = ModContent.ItemType<GlimmeringGemfish>();
					}
				}

				if (player.ZoneSnow)
				{
					if (Main.rand.NextBool(15) && Main.hardMode)
					{
						caughtType = ModContent.ItemType<FishofEleum>();
					}
				}

				if (player.ZoneDirtLayerHeight)
				{
					if (Main.rand.NextBool(Main.hardMode ? 100 : 40))
					{
						caughtType = ModContent.ItemType<Spadefish>();
					}
				}

				if (modPlayer.ZoneAstral) // Astral Infection, fishing in water
				{
					int astralFish = Main.rand.Next(100);
					if (caughtType == ItemID.WoodenCrate)
					{
						caughtType = ItemID.WoodenCrate;
					}
					else if (caughtType == ItemID.IronCrate)
					{
						caughtType = ItemID.IronCrate;
					}
					else if (caughtType == ItemID.GoldenCrate)
					{
						caughtType = ItemID.GoldenCrate;
					}
					else if (caughtType == ItemID.FrogLeg)
					{
						caughtType = ItemID.FrogLeg;
					}
					else if (caughtType == ItemID.BalloonPufferfish)
					{
						caughtType = ItemID.BalloonPufferfish;
					}
					else if (caughtType == ItemID.ZephyrFish)
					{
						caughtType = ItemID.ZephyrFish;
					}
					else if (astralFish >= 85) // 15%
					{
						caughtType = ModContent.ItemType<ProcyonidPrawn>();
					}
					else if (astralFish <= 84 && astralFish >= 70) // 15%
					{
						caughtType = ModContent.ItemType<ArcturusAstroidean>();
					}
					else if (astralFish <= 69 && astralFish >= 55) // 15%
					{
						caughtType = ModContent.ItemType<AldebaranAlewife>();
					}
					else if (player.cratePotion && astralFish <= 28 && astralFish >= 9) // 20%
					{
						caughtType = ModContent.ItemType<AstralCrate>();
					}
					else if (!player.cratePotion && astralFish <= 18 && astralFish >= 9) // 10%
					{
						caughtType = ModContent.ItemType<AstralCrate>();
					}
					else if (astralFish <= 8 && astralFish >= 6) // 3%
					{
						caughtType = ModContent.ItemType<UrsaSergeant>();
					}
					else if (astralFish <= 5 && astralFish >= 3) // 3%
					{
						caughtType = ModContent.ItemType<GacruxianMollusk>();
					}
					else if (astralFish <= 2 && astralFish >= 0) // 3%
					{
						caughtType = ModContent.ItemType<PolarisParrotfish>();
					}
					else // 31% w/o crate pot, 21% w/ crate pot
					{
						caughtType = ModContent.ItemType<TwinklingPollox>();
					}
				}

				if (modPlayer.ZoneSunkenSea) // Sunken Sea, fishing in water
				{
					int sunkenFish = Main.rand.Next(100);
					if (caughtType == ItemID.WoodenCrate)
					{
						caughtType = ItemID.WoodenCrate;
					}
					else if (caughtType == ItemID.IronCrate)
					{
						caughtType = ItemID.IronCrate;
					}
					else if (caughtType == ItemID.GoldenCrate)
					{
						caughtType = ItemID.GoldenCrate;
					}
					else if (caughtType == ItemID.FrogLeg)
					{
						caughtType = ItemID.FrogLeg;
					}
					else if (caughtType == ItemID.BalloonPufferfish)
					{
						caughtType = ItemID.BalloonPufferfish;
					}
					else if (caughtType == ItemID.ZephyrFish)
					{
						caughtType = ItemID.ZephyrFish;
					}
					else if (sunkenFish >= 85 && Main.hardMode) // 15%
					{
						caughtType = ModContent.ItemType<ScarredAngelfish>();
					}
					else if (sunkenFish <= 84 && sunkenFish >= 70) // 15%
					{
						caughtType = ModContent.ItemType<SunkenSailfish>();
					}
					else if (sunkenFish <= 69 && sunkenFish >= 55) // 15%
					{
						caughtType = ModContent.ItemType<CoralskinFoolfish>();
					}
					else if (player.cratePotion && sunkenFish <= 28 && sunkenFish >= 9) // 20%
					{
						caughtType = ModContent.ItemType<SunkenCrate>();
					}
					else if (!player.cratePotion && sunkenFish <= 18 && sunkenFish >= 9) // 10%
					{
						caughtType = ModContent.ItemType<SunkenCrate>();
					}
					else if (sunkenFish <= 31 && sunkenFish >= 29) // 3%
					{
						caughtType = ModContent.ItemType<GreenwaveLoach>();
					}
					else if (sunkenFish <= 8 && sunkenFish >= 6 && Main.hardMode) // 3%
					{
						caughtType = ModContent.ItemType<SerpentsBite>();
					}
					else if (sunkenFish <= 5 && sunkenFish >= 3) // 3%
					{
						caughtType = ModContent.ItemType<RustedJingleBell>();
					}
					else if (sunkenFish <= 2 && sunkenFish >= 0) // 3%
					{
						caughtType = ModContent.ItemType<SparklingEmpress>();
					}
					else // 33% w/o crate pot, 23% w/ crate pot + 18% if prehardmode
					{
						caughtType = ModContent.ItemType<PrismaticGuppy>();
					}
				}

				if (power >= 60 && player.FindBuffIndex(BuffID.Gills) > -1 && NPC.downedPlantBoss && Main.rand.NextBool(25) && power < 160)
				{
					caughtType = ModContent.ItemType<Floodtide>();
				}

				if (junk)
				{
					if (abyssPosX && power < 40)
					{
						caughtType = ModContent.ItemType<PlantyMush>();
					}
					return;
				}

				/*if (abyssPosX && (bait.type == ItemID.GoldWorm || bait.type == ItemID.GoldGrasshopper || bait.type == ItemID.GoldButterfly) && power > 150)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						CalamityGlobalNPC.OldDukeSpawn(player.whoAmI, ModContent.NPCType<OldDuke>());
					}
					else
					{
						NetMessage.SendData(61, -1, -1, null, player.whoAmI, (float)ModContent.NPCType<OldDuke>(), 0f, 0f, 0, 0, 0);
					}
					switch (Main.rand.Next(4))
					{
						case 0: caughtType = ModContent.ItemType<IronBoots>(); break; //movement acc
						case 1: caughtType = ModContent.ItemType<DepthCharm>(); break; //regen acc
						case 2: caughtType = ModContent.ItemType<AnechoicPlating>(); break; //defense acc
						case 3: caughtType = ModContent.ItemType<StrangeOrb>(); break; //light pet
					}
					return;
				}*/

				if (abyssPosX)
				{
					if (caughtType == ItemID.WoodenCrate)
					{
						caughtType = ItemID.WoodenCrate;
					}
					else if (caughtType == ItemID.IronCrate)
					{
						caughtType = ItemID.IronCrate;
					}
					else if (caughtType == ItemID.GoldenCrate)
					{
						caughtType = ItemID.GoldenCrate;
					}
					else if (caughtType == ItemID.FrogLeg)
					{
						caughtType = ItemID.FrogLeg;
					}
					else if (caughtType == ItemID.BalloonPufferfish)
					{
						caughtType = ItemID.BalloonPufferfish;
					}
					else if (caughtType == ItemID.ZephyrFish)
					{
						caughtType = ItemID.ZephyrFish;
					}
					else if (power >= 40)
					{
						if (Main.rand.NextBool(15) && power < 80)
						{
							caughtType = ModContent.ItemType<PlantyMush>();
						}
						if (Main.rand.NextBool(25) && power < 160)
						{
							caughtType = ModContent.ItemType<AlluringBait>();
						}
						if (power >= 110)
						{
							if (abyssPosX && Main.rand.NextBool(25) && power < 240)
							{
								caughtType = ModContent.ItemType<AbyssalAmulet>();
							}
						}
					}
					else if (player.cratePotion && Main.rand.NextBool(5)) // 20%
					{
						caughtType = ModContent.ItemType<AbyssalCrate>();
					}
					else if (!player.cratePotion && Main.rand.NextBool(10)) // 10%
					{
						caughtType = ModContent.ItemType<AbyssalCrate>();
					}
				}

				if (player.ZoneOverworldHeight && Main.bloodMoon)
				{
					if (Main.rand.NextBool(15))
					{
						caughtType = ModContent.ItemType<Xerocodile>();
					}
				}
			}

			if (lava)
			{
				if (modPlayer.ZoneCalamity) // Brimstone Crags, fishing in lava
				{
					int cragFish = Main.rand.Next(100);
					if (cragFish >= 85) // 15%
					{
						caughtType = ModContent.ItemType<CoastalDemonfish>();
					}
					else if (cragFish <= 84 && cragFish >= 70) // 15%
					{
						caughtType = ModContent.ItemType<BrimstoneFish>();
					}
					else if (cragFish <= 69 && cragFish >= 55) // 15%
					{
						caughtType = ModContent.ItemType<Shadowfish>();
					}
					else if (cragFish <= 54 && cragFish >= 41 && Main.hardMode) // 14%
					{
						caughtType = ModContent.ItemType<ChaoticFish>();
					}
					else if (player.cratePotion && cragFish <= 40 && cragFish >= 21) // 20%
					{
						caughtType = ModContent.ItemType<BrimstoneCrate>();
					}
					else if (!player.cratePotion && cragFish <= 30 && cragFish >= 21) // 10%
					{
						caughtType = ModContent.ItemType<BrimstoneCrate>();
					}
					else if (cragFish <= 20 && cragFish >= 11 && CalamityWorld.downedProvidence) // 10%
					{
						caughtType = ModContent.ItemType<Bloodfin>();
					}
					else if (cragFish <= 10 && cragFish >= 5) // 5%
					{
						caughtType = ModContent.ItemType<DragoonDrizzlefish>();
					}
					else if (cragFish <= 2 && cragFish >= 0) // 3%
					{
						caughtType = ModContent.ItemType<CharredLasher>();
					}
					else // 27% w/o crate pot, 17% w/ crate pot, add 10% pre-Prov, add another 14% prehardmode
					{
						caughtType = ModContent.ItemType<CragBullhead>();
					}
				}
			}

			// Quest Fish
			if (modPlayer.ZoneSunkenSea && questFish == ModContent.ItemType<EutrophicSandfish>() && Main.rand.NextBool(10))
			{
				caughtType = ModContent.ItemType<EutrophicSandfish>();
			}
			if (modPlayer.ZoneSunkenSea && questFish == ModContent.ItemType<SurfClam>() && Main.rand.NextBool(10))
			{
				caughtType = ModContent.ItemType<SurfClam>();
			}
			if (modPlayer.ZoneSunkenSea && questFish == ModContent.ItemType<Serpentuna>() && Main.rand.NextBool(10))
			{
				caughtType = ModContent.ItemType<Serpentuna>();
			}
			if (modPlayer.ZoneCalamity && questFish == ModContent.ItemType<Brimlish>() && Main.rand.NextBool(10))
			{
				caughtType = ModContent.ItemType<Brimlish>();
			}
			if (modPlayer.ZoneCalamity && questFish == ModContent.ItemType<Slurpfish>() && Main.rand.NextBool(10))
			{
				caughtType = ModContent.ItemType<Slurpfish>();
			}
		}
        #endregion

        #region Get Fishing Level
        public static void CalamityGetFishingLevel(Player player, ref Item fishingRod, ref Item bait, ref int fishingLevel)
        {
            CalamityPlayer modPlayer = player.Calamity();

			if ((modPlayer.ZoneAstral || modPlayer.ZoneAbyss || modPlayer.ZoneSulphur) && bait.type == ModContent.ItemType<ArcturusAstroidean>())
				fishingLevel = (int)(fishingLevel * 1.1f);
			if (Main.player[Main.myPlayer].ZoneSnow && fishingRod.type == ModContent.ItemType<VerstaltiteFishingRod>())
				fishingLevel = (int)(fishingLevel * 1.1f);
			if (Main.player[Main.myPlayer].ZoneSkyHeight && fishingRod.type == ModContent.ItemType<HeronRod>())
				fishingLevel = (int)(fishingLevel * 1.1f);
		}
        #endregion
    }
}
