﻿using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Wings;
using CalamityMod.Items.Fishing.AstralCatches;
using CalamityMod.Items.Fishing.BrimstoneCragCatches;
using CalamityMod.Items.Fishing.SunkenSeaCatches;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Weapons.Melee;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod
{
    internal class CalamityRecipes
    {
        private static ModRecipe GetNewRecipe()
        {
            return new ModRecipe(ModContent.GetInstance<CalamityMod>());
        }

        public static void AddRecipes()
        {
            EditTerraBladeRecipe();
			EditFireGauntletRecipe();

			AddPotionRecipes();
            AddCookedFood();
            AddToolRecipes();
            AddProgressionRecipes();
            AddEarlyGameWeaponRecipes();
            AddEarlyGameAccessoryRecipes();
            AddAnkhShieldRecipes();
            AddAlternateHardmodeRecipes();

            // Leather from Vertebrae, for Crimson worlds
            ModRecipe r = GetNewRecipe();
            r.AddIngredient(ItemID.Vertebrae, 5);
            r.AddTile(TileID.WorkBenches);
            r.SetResult(ItemID.Leather);
            r.AddRecipe();

            // Black Lens
            r = GetNewRecipe();
            r.AddIngredient(ItemID.Lens);
            r.AddIngredient(ItemID.BlackDye);
            r.AddTile(TileID.DyeVat);
            r.SetResult(ItemID.BlackLens);
            r.AddRecipe();

            // Fallen Star
            r = GetNewRecipe();
            r.AddIngredient(ModContent.ItemType<Stardust>(), 5);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(ItemID.FallenStar);
            r.AddRecipe();

            // Ectoplasm from Ectoblood
            r = GetNewRecipe();
            r.AddIngredient(ModContent.ItemType<Ectoblood>(), 3);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(ItemID.Ectoplasm);
            r.AddRecipe();

            // Rocket I from Empty Bullet
            r = GetNewRecipe();
            r.AddIngredient(ItemID.EmptyBullet, 20);
            r.AddIngredient(ItemID.ExplosivePowder, 1);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(ItemID.RocketI, 20);
            r.AddRecipe();

            // Life Crystal
            r = GetNewRecipe();
            r.AddIngredient(ItemID.Bone, 5);
            r.AddIngredient(ItemID.PinkGel);
            r.AddIngredient(ItemID.HealingPotion);
            r.AddIngredient(ItemID.Ruby);
            r.AddTile(TileID.Anvils);
            r.SetResult(ItemID.LifeCrystal);
            r.AddRecipe();

            // Life Fruit
            r = GetNewRecipe();
            r.AddIngredient(ModContent.ItemType<PlantyMush>(), 10);
            r.AddIngredient(ModContent.ItemType<LivingShard>());
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(ItemID.LifeFruit);
            r.AddRecipe();

            // Glass
            r = GetNewRecipe();
            r.AddIngredient(ModContent.ItemType<AstralSand>(), 2);
            r.AddTile(TileID.Furnaces);
            r.SetResult(ItemID.Glass);
            r.AddRecipe();
        }

        // Change Terra Blade's recipe to require 7 Living Shards (forces the Blade to be post-Plantera)
        private static void EditTerraBladeRecipe()
        {
            List<Recipe> rec = Main.recipe.ToList();
            rec.Where(x => x.createItem.type == ItemID.TerraBlade).ToList().ForEach(s =>
            {
                for (int i = 0; i < s.requiredItem.Length; i++)
                {
                    s.requiredItem[i] = new Item();
                }
                s.requiredItem[0].SetDefaults(ItemID.TrueNightsEdge, false);
                s.requiredItem[0].stack = 1;
                s.requiredItem[1].SetDefaults(ItemID.TrueExcalibur, false);
                s.requiredItem[1].stack = 1;
                s.requiredItem[2].SetDefaults(ModContent.ItemType<LivingShard>(), false);
                s.requiredItem[2].stack = 7;

                s.createItem.SetDefaults(ItemID.TerraBlade, false);
                s.createItem.stack = 1;
            });
        }

		// Change Fire Gauntlet's recipe to require 5 Chaotic Bars (forces the item to be post-Golem)
		private static void EditFireGauntletRecipe()
		{
			List<Recipe> rec = Main.recipe.ToList();
			rec.Where(x => x.createItem.type == ItemID.FireGauntlet).ToList().ForEach(s =>
			{
				for (int i = 0; i < s.requiredItem.Length; i++)
				{
					s.requiredItem[i] = new Item();
				}
				s.requiredItem[0].SetDefaults(ItemID.MagmaStone, false);
				s.requiredItem[0].stack = 1;
				s.requiredItem[1].SetDefaults(ItemID.MechanicalGlove, false);
				s.requiredItem[1].stack = 1;
				s.requiredItem[2].SetDefaults(ModContent.ItemType<CruptixBar>(), false);
				s.requiredItem[2].stack = 5;

				s.createItem.SetDefaults(ItemID.FireGauntlet, false);
				s.createItem.stack = 1;
			});
		}

		#region Potions
		// Equivalent Blood Orb recipes for almost all vanilla potions
		private static void AddPotionRecipes()
        {
            short[] potions = new[]
            {
                ItemID.WormholePotion,
                ItemID.TeleportationPotion,
                ItemID.SwiftnessPotion,
                ItemID.FeatherfallPotion,
                ItemID.GravitationPotion,
                ItemID.ShinePotion,
                ItemID.InvisibilityPotion,
                ItemID.NightOwlPotion,
                ItemID.SpelunkerPotion,
                ItemID.HunterPotion,
                ItemID.TrapsightPotion,
                ItemID.BattlePotion,
                ItemID.CalmingPotion,
                ItemID.WrathPotion,
                ItemID.RagePotion,
                ItemID.ThornsPotion,
                ItemID.IronskinPotion,
                ItemID.EndurancePotion,
                ItemID.RegenerationPotion,
                ItemID.LifeforcePotion,
                ItemID.HeartreachPotion,
                ItemID.TitanPotion,
                ItemID.ArcheryPotion,
                ItemID.AmmoReservationPotion,
                ItemID.MagicPowerPotion,
                ItemID.ManaRegenerationPotion,
                ItemID.SummoningPotion,
                ItemID.InfernoPotion,
                ItemID.WarmthPotion,
                ItemID.ObsidianSkinPotion,
                ItemID.GillsPotion,
                ItemID.WaterWalkingPotion,
                ItemID.FlipperPotion,
                ItemID.BuilderPotion,
                ItemID.MiningPotion,
                ItemID.FishingPotion,
                ItemID.CratePotion,
                ItemID.SonarPotion,
                ItemID.GenderChangePotion,
                ItemID.LovePotion,
                ItemID.StinkPotion
            };
            ModRecipe r;

            foreach (var potion in potions)
            {
                r = GetNewRecipe();
                r.AddIngredient(ModContent.ItemType<BloodOrb>(), 10);
                r.AddIngredient(ItemID.BottledWater);
                r.AddTile(TileID.AlchemyTable);
                r.SetResult(potion);
                r.AddRecipe();
            }
        }
        #endregion

        #region Cooked Food
        private static void AddCookedFood()
        {
            ModRecipe r = GetNewRecipe();
            r.AddIngredient(ModContent.ItemType<TwinklingPollox>());
            r.AddTile(TileID.CookingPots);
            r.SetResult(ItemID.CookedFish);
            r.AddRecipe();

            r = GetNewRecipe();
            r.AddIngredient(ModContent.ItemType<PrismaticGuppy>());
            r.AddTile(TileID.CookingPots);
            r.SetResult(ItemID.CookedFish);
            r.AddRecipe();

            r = GetNewRecipe();
            r.AddIngredient(ModContent.ItemType<BrimstoneFish>());
            r.AddTile(TileID.CookingPots);
            r.SetResult(ItemID.CookedFish);
            r.AddRecipe();

            r = GetNewRecipe();
            r.AddIngredient(ModContent.ItemType<ProcyonidPrawn>());
            r.AddTile(TileID.CookingPots);
            r.SetResult(ItemID.CookedShrimp);
            r.AddRecipe();
        }
        #endregion

        #region Tools
        // Essential tools such as the Magic Mirror and Rod of Discord
        private static void AddToolRecipes()
        {
            // Magic Mirror
            ModRecipe r = GetNewRecipe();
            r.AddIngredient(ItemID.IronBar, 10);
            r.AddIngredient(ItemID.Glass, 10);
            r.AddIngredient(ItemID.FallenStar, 10);
            r.anyIronBar = true;
            r.AddTile(TileID.Anvils);
            r.SetResult(ItemID.MagicMirror);
            r.AddRecipe();

            // Ice Mirror
            r = GetNewRecipe();
            r.AddIngredient(ItemID.IceBlock, 20);
            r.AddIngredient(ItemID.Glass, 10);
            r.AddIngredient(ItemID.FallenStar, 10);
            r.AddIngredient(ItemID.IronBar, 5);
            r.anyIronBar = true;
            r.AddTile(TileID.Anvils);
            r.SetResult(ItemID.IceMirror);
            r.AddRecipe();

            // Shadow Key
            r = GetNewRecipe();
            r.AddIngredient(ItemID.GoldenKey);
            r.AddIngredient(ItemID.Obsidian, 20);
            r.AddIngredient(ItemID.Bone, 5);
            r.AddTile(TileID.Anvils);
            r.SetResult(ItemID.ShadowKey);
            r.AddRecipe();

            // Rod of Discord
            r = GetNewRecipe();
            r.AddIngredient(ItemID.SoulofLight, 30);
            r.AddIngredient(ItemID.ChaosFish, 5);
            r.AddIngredient(ItemID.PixieDust, 50);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(ItemID.RodofDiscord);
            r.AddRecipe();

            // Sky Mill
            r = GetNewRecipe();
            r.AddIngredient(ItemID.SunplateBlock, 10);
            r.AddIngredient(ItemID.Cloud, 5);
            r.AddIngredient(ItemID.RainCloud, 3);
            r.AddTile(TileID.Anvils);
            r.SetResult(ItemID.SkyMill);
            r.AddRecipe();

            // Ice Machine
            r = GetNewRecipe();
            r.AddIngredient(ItemID.IceBlock, 25);
            r.AddIngredient(ItemID.SnowBlock, 15);
            r.AddIngredient(ItemID.IronBar, 3);
            r.anyIronBar = true;
            r.AddTile(TileID.Anvils);
            r.SetResult(ItemID.IceMachine);
            r.AddRecipe();
        }
        #endregion

        #region ProgressionItems
        // Boss summon and progression items
        private static void AddProgressionRecipes()
        {
            // Guide Voodoo Doll
            ModRecipe r = GetNewRecipe();
            r.AddIngredient(ItemID.Leather, 2);
            r.AddRecipeGroup("FetidBloodletting", 2);
            r.AddTile(TileID.Hellforge);
            r.SetResult(ItemID.GuideVoodooDoll);
            r.AddRecipe();

            // Temple Key
            r = GetNewRecipe();
            r.AddIngredient(ItemID.JungleSpores, 15);
            r.AddIngredient(ItemID.RichMahogany, 15);
            r.AddIngredient(ItemID.SoulofNight, 15);
            r.AddIngredient(ItemID.SoulofLight, 15);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(ItemID.TempleKey);
            r.AddRecipe();

            // Lihzahrd Power Cell (NOT Calamity's Old Power Cell)
            r = GetNewRecipe();
            r.AddIngredient(ItemID.LihzahrdBrick, 15);
            r.AddIngredient(ModContent.ItemType<CoreofCinder>());
            r.AddTile(TileID.LihzahrdFurnace);
            r.SetResult(ItemID.LihzahrdPowerCell);
            r.AddRecipe();

            // Truffle Worm
            r = GetNewRecipe();
            r.AddIngredient(ItemID.GlowingMushroom, 15);
            r.AddIngredient(ItemID.Worm);
            r.AddTile(TileID.Autohammer);
            r.SetResult(ItemID.TruffleWorm);
            r.AddRecipe();
        }
        #endregion

        #region EarlyGameWeapons
        // Early game weapons such as Enchanted Sword
        private static void AddEarlyGameWeaponRecipes()
        {
            // Shuriken
            ModRecipe r = GetNewRecipe();
            r.AddIngredient(ItemID.IronBar);
            r.anyIronBar = true;
            r.AddTile(TileID.Anvils);
            r.SetResult(ItemID.Shuriken, 50);
            r.AddRecipe();

            // Throwing Knife
            r = GetNewRecipe();
            r.AddIngredient(ItemID.IronBar);
            r.anyIronBar = true;
            r.AddTile(TileID.Anvils);
            r.SetResult(ItemID.ThrowingKnife, 50);
            r.AddRecipe();

            // Wand of Sparking
            r = GetNewRecipe();
            r.AddIngredient(ItemID.Wood, 5);
            r.AddIngredient(ItemID.Torch, 3);
            r.AddIngredient(ItemID.FallenStar);
            r.AddTile(TileID.Anvils);
            r.SetResult(ItemID.WandofSparking);
            r.AddRecipe();

            // Starfury w/ Gold Broadsword
            r = GetNewRecipe();
            r.AddIngredient(ItemID.GoldBroadsword);
            r.AddIngredient(ItemID.FallenStar, 10);
            r.AddIngredient(ModContent.ItemType<VictoryShard>(), 3);
            r.AddTile(TileID.Anvils);
            r.SetResult(ItemID.Starfury);
            r.AddRecipe();

            // Starfury w/ Platinum Broadsword
            r = GetNewRecipe();
            r.AddIngredient(ItemID.PlatinumBroadsword);
            r.AddIngredient(ItemID.FallenStar, 10);
            r.AddIngredient(ModContent.ItemType<VictoryShard>(), 3);
            r.AddTile(TileID.Anvils);
            r.SetResult(ItemID.Starfury);
            r.AddRecipe();

            // Enchanted Sword (requires Hardmode materials)
            r = GetNewRecipe();
            r.AddIngredient(ModContent.ItemType<VictoryShard>(), 10);
            r.AddIngredient(ItemID.SoulofLight, 15);
            r.AddIngredient(ItemID.UnicornHorn, 3);
            r.AddIngredient(ItemID.LightShard);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(ItemID.EnchantedSword);
            r.AddRecipe();

            // Muramasa
            r = GetNewRecipe();
            r.AddRecipeGroup("AnyCobaltBar", 15);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(ItemID.Muramasa);
            r.AddRecipe();

            // Water Bolt w/ Hardmode Spell Tome
            r = GetNewRecipe();
            r.AddIngredient(ItemID.SpellTome);
            r.AddIngredient(ItemID.Waterleaf, 3);
            r.AddIngredient(ItemID.WaterCandle);
            r.AddTile(TileID.Bookcases);
            r.SetResult(ItemID.WaterBolt);
            r.AddRecipe();
        }
        #endregion

        #region EarlyGameAccessories
        // Early game accessories such as Cloud in a Bottle
        private static void AddEarlyGameAccessoryRecipes()
        {
            // Cloud in a Bottle
            ModRecipe r = GetNewRecipe();
            r.AddIngredient(ItemID.Feather, 2);
            r.AddIngredient(ItemID.Bottle);
            r.AddIngredient(ItemID.Cloud, 25);
            r.AddTile(TileID.Anvils);
            r.SetResult(ItemID.CloudinaBottle);
            r.AddRecipe();

            // Hermes Boots
            r = GetNewRecipe();
            r.AddIngredient(ItemID.Silk, 10);
            r.AddIngredient(ItemID.SwiftnessPotion, 4);
            r.AddTile(TileID.Loom);
            r.SetResult(ItemID.HermesBoots);
            r.AddRecipe();

            // Blizzard in a Bottle
            r = GetNewRecipe();
            r.AddIngredient(ItemID.Feather, 4);
            r.AddIngredient(ItemID.Bottle);
            r.AddIngredient(ItemID.SnowBlock, 50);
            r.AddTile(TileID.Anvils);
            r.SetResult(ItemID.BlizzardinaBottle);
            r.AddRecipe();

            // Sandstorm in a Bottle
            r = GetNewRecipe();
            r.AddIngredient(ModContent.ItemType<DesertFeather>(), 10);
            r.AddIngredient(ItemID.Feather, 6);
            r.AddIngredient(ItemID.Bottle);
            r.AddIngredient(ItemID.SandBlock, 70);
            r.AddTile(TileID.Anvils);
            r.SetResult(ItemID.SandstorminaBottle);
            r.AddRecipe();

            // Frog Leg
            r = GetNewRecipe();
            r.AddIngredient(ItemID.Frog, 10);
            r.AddTile(TileID.Anvils);
            r.SetResult(ItemID.FrogLeg);
            r.AddRecipe();

            // Flying Carpet
            r = GetNewRecipe();
            r.AddIngredient(ItemID.AncientCloth, 10);
            r.AddIngredient(ItemID.SoulofLight, 10);
            r.AddIngredient(ItemID.SoulofNight, 10);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(ItemID.FlyingCarpet);
            r.AddRecipe();

            // Aglet
            r = GetNewRecipe();
            r.AddIngredient(ItemID.IronBar, 5);
            r.anyIronBar = true;
            r.AddTile(TileID.Anvils);
            r.SetResult(ItemID.Aglet);
            r.AddRecipe();

            // Anklet of the Wind
            r = GetNewRecipe();
            r.AddIngredient(ItemID.JungleSpores, 15);
            r.AddIngredient(ItemID.Cloud, 15);
            r.AddIngredient(ItemID.PinkGel, 5);
            r.AddTile(TileID.Anvils);
            r.SetResult(ItemID.AnkletoftheWind);
            r.AddRecipe();

            // Water Walking Boots
            r = GetNewRecipe();
            r.AddIngredient(ItemID.Leather, 5);
            r.AddIngredient(ItemID.WaterWalkingPotion, 8);
            r.AddTile(TileID.Anvils);
            r.SetResult(ItemID.WaterWalkingBoots);
            r.AddRecipe();

            // Ice Skates
            r = GetNewRecipe();
            r.AddIngredient(ItemID.IceBlock, 20);
            r.AddIngredient(ItemID.Leather, 5);
            r.AddIngredient(ItemID.IronBar, 5);
            r.anyIronBar = true;
            r.AddTile(TileID.IceMachine);
            r.SetResult(ItemID.IceSkates);
            r.AddRecipe();

            // Lucky Horseshoe
            r = GetNewRecipe();
            r.AddIngredient(ItemID.SunplateBlock, 10);
            r.AddIngredient(ItemID.Cloud, 10);
            r.AddRecipeGroup("AnyGoldBar", 5);
            r.AddTile(TileID.Anvils);
            r.SetResult(ItemID.LuckyHorseshoe);
            r.AddRecipe();

            // Shiny Red Balloon
            r = GetNewRecipe();
            r.AddIngredient(ItemID.WhiteString);
            r.AddIngredient(ItemID.Gel, 80);
            r.AddIngredient(ItemID.Cloud, 40);
            r.AddTile(TileID.Solidifier);
            r.SetResult(ItemID.ShinyRedBalloon);
            r.AddRecipe();

            // Lava Charm
            r = GetNewRecipe();
            r.AddIngredient(ItemID.LavaBucket, 5);
            r.AddIngredient(ItemID.Obsidian, 25);
            r.AddIngredient(ItemID.IronBar, 5);
            r.anyIronBar = true;
            r.AddTile(TileID.Anvils);
            r.SetResult(ItemID.LavaCharm);
            r.AddRecipe();

            // Obsidian Rose
            r = GetNewRecipe();
            r.AddIngredient(ItemID.JungleRose);
            r.AddIngredient(ItemID.Obsidian, 10);
            r.AddIngredient(ItemID.Hellstone, 10);
            r.anyIronBar = true;
            r.AddTile(TileID.Anvils);
            r.SetResult(ItemID.ObsidianRose);
            r.AddRecipe();

            // Feral Claws
            r = GetNewRecipe();
            r.AddIngredient(ItemID.Leather, 10);
            r.AddTile(TileID.Anvils);
            r.SetResult(ItemID.FeralClaws);
            r.AddRecipe();

            // Radar
            r = GetNewRecipe();
            r.AddIngredient(ItemID.IronBar, 5);
            r.anyIronBar = true;
            r.AddTile(TileID.Anvils);
            r.SetResult(ItemID.Radar);
            r.AddRecipe();
        }
        #endregion

        #region AnkhShield
        // Every base component for the Ankh Shield
        private static void AddAnkhShieldRecipes()
        {
            // Cobalt Shield
            ModRecipe r = GetNewRecipe();
            r.AddRecipeGroup("AnyCobaltBar", 10);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(ItemID.CobaltShield);
            r.AddRecipe();

            // Armor Polish (broken armor)
            r = GetNewRecipe();
            r.AddIngredient(ItemID.Bone, 50);
            r.AddIngredient(ModContent.ItemType<AncientBoneDust>(), 3);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(ItemID.ArmorPolish);
            r.AddRecipe();

            // Adhesive Bandage (bleeding)
            r = GetNewRecipe();
            r.AddIngredient(ItemID.Silk, 10);
            r.AddIngredient(ItemID.Gel, 50);
            r.AddIngredient(ItemID.GreaterHealingPotion);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(ItemID.AdhesiveBandage);
            r.AddRecipe();

            // Bezoar (poison)
            r = GetNewRecipe();
            r.AddIngredient(ItemID.Stinger, 15);
            r.AddIngredient(ModContent.ItemType<MurkyPaste>());
            r.AddTile(TileID.Anvils);
            r.SetResult(ItemID.Bezoar);
            r.AddRecipe();

            // Nazar (curse)
            r = GetNewRecipe();
            r.AddIngredient(ItemID.SoulofNight, 20);
            r.AddIngredient(ItemID.Lens, 5);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(ItemID.Nazar);
            r.AddRecipe();

            // Vitamins (weakness)
            r = GetNewRecipe();
            r.AddIngredient(ItemID.BottledWater);
            r.AddIngredient(ItemID.Waterleaf, 5);
            r.AddIngredient(ItemID.Blinkroot, 5);
            r.AddIngredient(ItemID.Daybloom, 5);
            r.AddIngredient(ModContent.ItemType<BeetleJuice>(), 3);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(ItemID.Vitamins);
            r.AddRecipe();

            // Blindfold (darkness)
            r = GetNewRecipe();
            r.AddIngredient(ItemID.Silk, 30);
            r.AddIngredient(ItemID.SoulofNight, 5);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(ItemID.Blindfold);
            r.AddRecipe();

            // Trifold Map (confusion)
            r = GetNewRecipe();
            r.AddIngredient(ItemID.Silk, 20);
            r.AddIngredient(ItemID.SoulofLight, 3);
            r.AddIngredient(ItemID.SoulofNight, 3);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(ItemID.TrifoldMap);
            r.AddRecipe();

            // Fast Clock (slow)
            r = GetNewRecipe();
            r.AddIngredient(ItemID.Timer1Second);
            r.AddIngredient(ItemID.PixieDust, 15);
            r.AddIngredient(ItemID.SoulofLight, 5);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(ItemID.FastClock);
            r.AddRecipe();

            // Megaphone (silence)
            r = GetNewRecipe();
            r.AddIngredient(ItemID.Wire, 10);
            r.AddIngredient(ItemID.HallowedBar, 5);
            r.AddIngredient(ItemID.Ruby, 3);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(ItemID.Megaphone);
            r.AddRecipe();
        }
        #endregion

        #region HardmodeEquipment
        // Alternate recipes for vanilla Hardmode equipment
        private static void AddAlternateHardmodeRecipes()
        {
            // Avenger Emblem made with Rogue Emblem
            ModRecipe r = GetNewRecipe();
            r.AddIngredient(ModContent.ItemType<RogueEmblem>());
            r.AddIngredient(ItemID.SoulofMight, 5);
            r.AddIngredient(ItemID.SoulofSight, 5);
            r.AddIngredient(ItemID.SoulofFright, 5);
            r.AddTile(TileID.TinkerersWorkbench);
            r.SetResult(ItemID.AvengerEmblem);
            r.AddRecipe();

            // Celestial Magnet
            r = GetNewRecipe();
            r.AddIngredient(ItemID.FallenStar, 20);
            r.AddIngredient(ItemID.SoulofMight, 10);
            r.AddIngredient(ItemID.SoulofLight, 5);
            r.AddIngredient(ItemID.SoulofNight, 5);
            r.AddIngredient(ModContent.ItemType<CryoBar>(), 3);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(ItemID.CelestialMagnet);
            r.AddRecipe();

            // Frozen Turtle Shell
            r = GetNewRecipe();
            r.AddIngredient(ItemID.TurtleShell, 3);
            r.AddIngredient(ModContent.ItemType<EssenceofEleum>(), 9);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(ItemID.FrozenTurtleShell);
            r.AddRecipe();

            // Magic Quiver
            r = GetNewRecipe();
            r.AddIngredient(ItemID.EndlessQuiver);
            r.AddIngredient(ItemID.PixieDust, 10);
            r.AddIngredient(ModContent.ItemType<BlightedLens>(), 5);
            r.AddIngredient(ItemID.SoulofLight, 8);
            r.AddTile(TileID.CrystalBall);
            r.SetResult(ItemID.MagicQuiver);
            r.AddRecipe();

            // Frost Helmet w/ Frigid Bars
            r = GetNewRecipe();
            r.AddIngredient(ModContent.ItemType<CryoBar>(), 6);
            r.AddIngredient(ItemID.FrostCore);
            r.AddTile(TileID.IceMachine);
            r.SetResult(ItemID.FrostHelmet);
            r.AddRecipe();

            // Frost Breastplate w/ Frigid Bars
            r = GetNewRecipe();
            r.AddIngredient(ModContent.ItemType<CryoBar>(), 10);
            r.AddIngredient(ItemID.FrostCore);
            r.AddTile(TileID.IceMachine);
            r.SetResult(ItemID.FrostBreastplate);
            r.AddRecipe();

            // Frost Leggings w/ Frigid Bars
            r = GetNewRecipe();
            r.AddIngredient(ModContent.ItemType<CryoBar>(), 8);
            r.AddIngredient(ItemID.FrostCore);
            r.AddTile(TileID.IceMachine);
            r.SetResult(ItemID.FrostLeggings);
            r.AddRecipe();

            // Terra Blade w/ True Bloody Edge
            r = GetNewRecipe();
            r.AddIngredient(ModContent.ItemType<TrueBloodyEdge>());
            r.AddIngredient(ItemID.TrueExcalibur);
            r.AddIngredient(ModContent.ItemType<LivingShard>(), 7);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(ItemID.TerraBlade);
            r.AddRecipe();
        }
        #endregion

        public static void AddRecipeGroups()
        {
            RecipeGroup group = new RecipeGroup(() => "Any Copper Bar", new int[]
            {
                ItemID.CopperBar,
                ItemID.TinBar
            });
            RecipeGroup.RegisterGroup("AnyCopperBar", group);

            group = new RecipeGroup(() => "Any Gold Bar", new int[]
            {
                ItemID.GoldBar,
                ItemID.PlatinumBar
            });
            RecipeGroup.RegisterGroup("AnyGoldBar", group);

            group = new RecipeGroup(() => "Any Cobalt Bar", new int[]
            {
                ItemID.CobaltBar,
                ItemID.PalladiumBar
            });
            RecipeGroup.RegisterGroup("AnyCobaltBar", group);

            group = new RecipeGroup(() => "Any Adamantite Bar", new int[]
            {
                ItemID.AdamantiteBar,
                ItemID.TitaniumBar
            });
            RecipeGroup.RegisterGroup("AnyAdamantiteBar", group);

            group = new RecipeGroup(() => "Nightmare Fuel or Endothermic Energy", new int[]
            {
                ModContent.ItemType<NightmareFuel>(),
                ModContent.ItemType<EndothermicEnergy>()
            });
            RecipeGroup.RegisterGroup("NForEE", group);

            group = new RecipeGroup(() => "Fetid or Bloodletting Essence", new int[]
            {
                ModContent.ItemType<FetidEssence>(),
                ModContent.ItemType<BloodlettingEssence>()
            });
            RecipeGroup.RegisterGroup("FetidBloodletting", group);

            group = new RecipeGroup(() => "Shadow Scale or Tissue Sample", new int[]
            {
                ItemID.ShadowScale,
                ItemID.TissueSample
            });
            RecipeGroup.RegisterGroup("Boss2Material", group);

            group = new RecipeGroup(() => "Cursed Flame or Ichor", new int[]
            {
                ItemID.CursedFlame,
                ItemID.Ichor
            });
            RecipeGroup.RegisterGroup("CursedFlameIchor", group);

            group = new RecipeGroup(() => "Any Silt", new int[]
            {
                ItemID.SiltBlock,
                ItemID.SlushBlock
            });
            RecipeGroup.RegisterGroup("SiltGroup", group);

            group = new RecipeGroup(() => "Any Hardmode Anvil", new int[]
            {
                ItemID.MythrilAnvil,
                ItemID.OrichalcumAnvil
            });
            RecipeGroup.RegisterGroup("HardmodeAnvil", group);

            group = new RecipeGroup(() => "Any Hardmode Forge", new int[]
            {
                ItemID.AdamantiteForge,
                ItemID.TitaniumForge
            });
            RecipeGroup.RegisterGroup("HardmodeForge", group);

            group = new RecipeGroup(() => "Any Lunar Pickaxe", new int[]
            {
                ItemID.SolarFlarePickaxe,
                ItemID.VortexPickaxe,
                ItemID.NebulaPickaxe,
                ItemID.StardustPickaxe
            });
            RecipeGroup.RegisterGroup("LunarPickaxe", group);

            group = new RecipeGroup(() => "Any Lunar Hamaxe", new int[]
            {
                ItemID.LunarHamaxeSolar,
                ItemID.LunarHamaxeVortex,
                ItemID.LunarHamaxeNebula,
                ItemID.LunarHamaxeStardust
            });
            RecipeGroup.RegisterGroup("LunarHamaxe", group);

            group = new RecipeGroup(() => "Any Wings", new int[]
            {
                ItemID.DemonWings,
                ItemID.AngelWings,
                ItemID.RedsWings,
                ItemID.ButterflyWings,
                ItemID.FairyWings,
                ItemID.HarpyWings,
                ItemID.BoneWings,
                ItemID.FlameWings,
                ItemID.FrozenWings,
                ItemID.GhostWings,
                ItemID.SteampunkWings,
                ItemID.LeafWings,
                ItemID.BatWings,
                ItemID.BeeWings,
                ItemID.DTownsWings,
                ItemID.WillsWings,
                ItemID.CrownosWings,
                ItemID.CenxsWings,
                ItemID.TatteredFairyWings,
                ItemID.SpookyWings,
                ItemID.Hoverboard,
                ItemID.FestiveWings,
                ItemID.BeetleWings,
                ItemID.FinWings,
                ItemID.FishronWings,
                ItemID.MothronWings,
                ItemID.WingsSolar,
                ItemID.WingsVortex,
                ItemID.WingsNebula,
                ItemID.WingsStardust,
                ItemID.Yoraiz0rWings,
                ItemID.JimsWings,
                ItemID.SkiphsWings,
                ItemID.LokisWings,
                ItemID.BetsyWings,
                ItemID.ArkhalisWings,
                ItemID.LeinforsWings,
                ModContent.ItemType<SkylineWings>(),
                ModContent.ItemType<StarlightWings>(),
                ModContent.ItemType<AureateWings>(),
                ModContent.ItemType<DiscordianWings>(),
                ModContent.ItemType<TarragonWings>(),
                ModContent.ItemType<XerocWings>(),
                ModContent.ItemType<HadarianWings>()
            });
            RecipeGroup.RegisterGroup("WingsGroup", group);
        }
    }
}
