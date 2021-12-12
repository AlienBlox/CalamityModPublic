using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Wings;
using CalamityMod.Items.Critters;
using CalamityMod.Items.Fishing.AstralCatches;
using CalamityMod.Items.Fishing.BrimstoneCragCatches;
using CalamityMod.Items.Fishing.SunkenSeaCatches;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Tools;
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
            EditLeatherRecipe();
            EditTerraBladeRecipe();
            EditFireGauntletRecipe();
            EditGoblinArmySummonRecipe();
            EditEvilBossSummonRecipes();
            EditEarlyHardmodeRecipes();
            EditMechBossSummonRecipes();
            EditWingRecipes();
            EditEvilBulletRecipes();
            EditPhasesaberRecipes();
            EditOpticStaffRecipes();
            AstralAlternatives();

            AddPotionRecipes();
            AddCookedFood();
            AddToolRecipes();
            AddProgressionRecipes();
            AddEarlyGameWeaponRecipes();
            AddEarlyGameAccessoryRecipes();
            AddArmorRecipes();
            AddAnkhShieldRecipes();
            AddAlternateHardmodeRecipes();

            // Leather from Vertebrae, for Crimson worlds
            ModRecipe r = GetNewRecipe();
            r.AddIngredient(ItemID.Vertebrae, 2);
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
            r.AddTile(TileID.Anvils);
            r.SetResult(ItemID.FallenStar);
            r.AddRecipe();

            // Hallowed Bars
            r = GetNewRecipe();
            r.AddIngredient(ModContent.ItemType<HallowedOre>(), 4);
            r.AddTile(TileID.AdamantiteForge);
            r.SetResult(ItemID.HallowedBar);
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
            r.AddIngredient(ItemID.StoneBlock, 5);
            r.AddIngredient(ItemID.Ruby, 2);
            r.AddIngredient(ItemID.HealingPotion);
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

            // Ultrabright Torch
            r = GetNewRecipe();
            r.AddIngredient(ItemID.Torch, 33);
            r.AddIngredient(ModContent.ItemType<SeaPrism>());
            r.AddTile(TileID.Anvils);
            r.SetResult(ItemID.UltrabrightTorch, 33);
            r.AddRecipe();

            // Target Dummy Reverse Compatibility
            r = GetNewRecipe();
            r.AddIngredient(ModContent.ItemType<SuperDummy>());
            r.AddTile(TileID.Anvils);
            r.SetResult(ItemID.TargetDummy);
            r.AddRecipe();
        }

        // Change Leather's recipe to require 2 Rotten Chunks/Vertebrae
        private static void EditLeatherRecipe()
        {
            List<Recipe> rec = Main.recipe.ToList();
            rec.Where(x => x.createItem.type == ItemID.Leather).ToList().ForEach(s =>
            {
                s.requiredItem[0].stack = 2;
            });
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

        private static void EditGoblinArmySummonRecipe()
        {
            List<Recipe> rec = Main.recipe.ToList();
            rec.Where(x => x.createItem.type == ItemID.GoblinBattleStandard).ToList().ForEach(s =>
            {
                s.requiredItem[0].stack = 5;
            });
        }

        private static void EditEvilBossSummonRecipes() // Evil Mushroom spawns are inconsistent and it bothers me. - Merkalto
        {
            List<Recipe> rec = Main.recipe.ToList();
            rec.Where(x => x.createItem.type == ItemID.BloodySpine || x.createItem.type == ItemID.WormFood).ToList().ForEach(s =>
            {
                s.requiredItem[0].stack = 20;
                s.requiredItem[1].stack = 10;
            });
        }
        private static void EditEarlyHardmodeRecipes()
        {
            List<Recipe> rec = Main.recipe.ToList();
            rec.Where(x => x.createItem.type == ItemID.DaoofPow || x.createItem.type == ItemID.Chik || x.createItem.type == ItemID.MeteorStaff).ToList().ForEach(s =>
            {
                s.requiredTile[0] = TileID.Anvils;
            });
        }

        private static void EditMechBossSummonRecipes()
        {
            List<Recipe> rec = Main.recipe.ToList();
            rec.Where(x => x.createItem.type == ItemID.MechanicalWorm || x.createItem.type == ItemID.MechanicalEye || x.createItem.type == ItemID.MechanicalSkull).ToList().ForEach(s =>
            {
                s.requiredTile[0] = TileID.Anvils;
            });
        }

        private static void EditWingRecipes()
        {
            List<Recipe> rec = Main.recipe.ToList();
            rec.Where(x => x.createItem.type == ItemID.AngelWings || x.createItem.type == ItemID.DemonWings || x.createItem.type == ItemID.FairyWings).ToList().ForEach(s =>
            {
                s.requiredTile[0] = TileID.Anvils;
            });
        }

        // Change Ichor, and Cursed Bullets/Arrows to be pre-Hardmode Boss
        private static void EditEvilBulletRecipes()
        {
            List<Recipe> rec = Main.recipe.ToList();
            rec.Where(x => x.createItem.type == ItemID.IchorBullet || x.createItem.type == ItemID.IchorArrow || x.createItem.type == ItemID.CursedBullet || x.createItem.type == ItemID.CursedArrow).ToList().ForEach(s =>
            {
                s.requiredTile[0] = TileID.Anvils;
            });
        }

        // Change Phasesaber recipes to require 20 Crystal Shards
        private static void EditPhasesaberRecipes()
        {
            List<Recipe> rec = Main.recipe.ToList();
            rec.Where(x => x.createItem.type == ItemID.BluePhasesaber || x.createItem.type == ItemID.GreenPhasesaber || x.createItem.type == ItemID.PurplePhasesaber ||
            x.createItem.type == ItemID.RedPhasesaber || x.createItem.type == ItemID.WhitePhasesaber || x.createItem.type == ItemID.YellowPhasesaber).ToList().ForEach(s =>
            {
                s.requiredItem[1].stack = 20;
            });
        }

        // Remove Hallowed Bars from Optic Staff
        private static void EditOpticStaffRecipes()
        {
            List<Recipe> rec = Main.recipe.ToList();
            rec.Where(x => x.createItem.type == ItemID.OpticStaff).ToList().ForEach(s =>
            {
                for (int i = 0; i < s.requiredItem.Length; i++)
                {
                    s.requiredItem[i] = new Item();
                }
                s.requiredItem[0].SetDefaults(ItemID.BlackLens, false);
                s.requiredItem[0].stack = 1;
                s.requiredItem[1].SetDefaults(ItemID.Lens, false);
                s.requiredItem[1].stack = 2;
                s.requiredItem[2].SetDefaults(ItemID.SoulofSight, false);
                s.requiredItem[2].stack = 20;

                s.createItem.SetDefaults(ItemID.OpticStaff, false);
                s.createItem.stack = 1;
            });
        }

        #region Astral Alternatives
        private static void AstralAlternatives()
        {
            //Bowl
            ModRecipe r = GetNewRecipe();
            r.AddIngredient(ModContent.ItemType<AstralClay>(), 2);
            r.AddTile(TileID.Furnaces);
            r.SetResult(ItemID.Bowl);
            r.AddRecipe();

            //Clay Pot
            r = GetNewRecipe();
            r.AddIngredient(ModContent.ItemType<AstralClay>(), 2);
            r.AddTile(TileID.Furnaces);
            r.SetResult(ItemID.ClayPot);
            r.AddRecipe();

            //Pink Vase
            r = GetNewRecipe();
            r.AddIngredient(ModContent.ItemType<AstralClay>(), 2);
            r.AddTile(TileID.Furnaces);
            r.SetResult(ItemID.PinkVase);
            r.AddRecipe();
        }
        #endregion

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
                ItemID.StinkPotion,
                ItemID.RecallPotion
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

            r = GetNewRecipe();
            r.AddIngredient(ItemID.BottledWater);
            r.AddIngredient(ItemID.Daybloom);
            r.AddIngredient(ModContent.ItemType<BlightedLens>());
            r.AddTile(TileID.Bottles);
            r.SetResult(ItemID.ArcheryPotion);
            r.AddRecipe();
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
            r.AddIngredient(ModContent.ItemType<CragBullhead>());
            r.AddTile(TileID.CookingPots);
            r.SetResult(ItemID.CookedFish);
            r.AddRecipe();

            r = GetNewRecipe();
            r.AddIngredient(ModContent.ItemType<ProcyonidPrawn>());
            r.AddTile(TileID.CookingPots);
            r.SetResult(ItemID.CookedShrimp);
            r.AddRecipe();

            r = GetNewRecipe();
            r.AddIngredient(ModContent.ItemType<PiggyItem>());
            r.AddTile(TileID.CookingPots);
            r.SetResult(ItemID.Bacon);
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
            r.AddRecipeGroup("AnyIceBlock", 20);
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
            r.AddRecipeGroup("AnyIceBlock", 25);
            r.AddRecipeGroup("AnySnowBlock", 15);
            r.AddIngredient(ItemID.IronBar, 3);
            r.anyIronBar = true;
            r.AddTile(TileID.Anvils);
            r.SetResult(ItemID.IceMachine);
            r.AddRecipe();

            // Bug Net
            r = GetNewRecipe();
            r.AddIngredient(ItemID.Cobweb, 30);
            r.AddIngredient(ItemID.IronBar, 3);
            r.anyIronBar = true;
            r.AddTile(TileID.Anvils);
            r.SetResult(ItemID.BugNet);
            r.AddRecipe();

            // Umbrella
            r = GetNewRecipe();
            r.AddIngredient(ItemID.Silk, 5);
            r.AddIngredient(ItemID.IronBar, 2);
            r.anyIronBar = true;
            r.AddTile(TileID.Loom);
            r.SetResult(ItemID.Umbrella);
            r.AddRecipe();

            // Living Loom
            r = GetNewRecipe();
            r.AddIngredient(ItemID.Loom);
            r.AddIngredient(ItemID.Vine, 2);
            r.AddTile(TileID.Sawmill);
            r.SetResult(ItemID.LivingLoom);
            r.AddRecipe();

            // Living Wood Wand
            r = GetNewRecipe();
            r.AddIngredient(ItemID.Wood, 30);
            r.AddTile(TileID.LivingLoom);
            r.SetResult(ItemID.LivingWoodWand);
            r.AddRecipe();

            // Living Leaf Wand
            r = GetNewRecipe();
            r.AddIngredient(ItemID.Wood, 30);
            r.AddTile(TileID.LivingLoom);
            r.SetResult(ItemID.LeafWand);
            r.AddRecipe();

            // Living Mahogany Wand
            r = GetNewRecipe();
            r.AddIngredient(ItemID.RichMahogany, 30);
            r.AddTile(TileID.LivingLoom);
            r.SetResult(ItemID.LivingMahoganyWand);
            r.AddRecipe();

            // Living Mahogany Leaf Wand
            r = GetNewRecipe();
            r.AddIngredient(ItemID.RichMahogany, 30);
            r.AddTile(TileID.LivingLoom);
            r.SetResult(ItemID.LivingMahoganyLeafWand);
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
            r.AddRecipeGroup("EvilPowder", 10);
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

            // Enchanted Sword
            r = GetNewRecipe();
            r.AddIngredient(ModContent.ItemType<VictoryShard>(), 10);
            r.AddRecipeGroup("AnyGoldBar", 12);
            r.AddIngredient(ItemID.Diamond);
            r.AddIngredient(ItemID.Ruby);
            r.AddTile(TileID.Anvils);
            r.SetResult(ItemID.EnchantedSword);
            r.AddRecipe();

            // Muramasa
            r = GetNewRecipe();
            r.AddRecipeGroup("AnyCobaltBar", 15);
            r.AddTile(TileID.Anvils);
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

            //Slime Staff
            r = GetNewRecipe();
            r.AddIngredient(ItemID.Wood, 6);
            r.anyWood = true;
            r.AddIngredient(ItemID.Gel, 40);
            r.AddIngredient(ItemID.PinkGel, 10);
            r.AddTile(TileID.Anvils);
            r.SetResult(ItemID.SlimeStaff);
            r.AddRecipe();

            //Ice Boomerang
            r = GetNewRecipe();
            r.AddRecipeGroup("AnyIceBlock", 20);
            r.AddRecipeGroup("AnySnowBlock", 10);
            r.AddIngredient(ItemID.Shiverthorn);
            r.AddTile(TileID.IceMachine);
            r.SetResult(ItemID.IceBoomerang);
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
            r.AddIngredient(ItemID.SwiftnessPotion, 2);
            r.AddTile(TileID.Loom);
            r.SetResult(ItemID.HermesBoots);
            r.AddRecipe();

            // Blizzard in a Bottle
            r = GetNewRecipe();
            r.AddIngredient(ItemID.Feather, 4);
            r.AddIngredient(ItemID.Bottle);
            r.AddRecipeGroup("AnySnowBlock", 50);
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
            r.AddIngredient(ItemID.Frog, 6);
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
            r.AddRecipeGroup("AnyIceBlock", 20);
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

            // Metal Detector
            r = GetNewRecipe();
            r.AddIngredient(ItemID.Wire, 10);
            r.AddIngredient(ItemID.GoldDust, 5);
            r.AddIngredient(ItemID.SpelunkerGlowstick, 5);
            r.AddIngredient(ItemID.IronBar, 5);
            r.anyIronBar = true;
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(ItemID.MetalDetector);
            r.AddRecipe();

            // Hand Warmer
            r = GetNewRecipe();
            r.AddIngredient(ItemID.Silk, 5);
            r.AddIngredient(ItemID.Shiverthorn);
            r.AddRecipeGroup("AnySnowBlock", 10);
            r.AddTile(TileID.Loom);
            r.SetResult(ItemID.HandWarmer);
            r.AddRecipe();

            // Flower Boots
            r = GetNewRecipe();
            r.AddIngredient(ItemID.Silk, 7);
            r.AddIngredient(ItemID.JungleRose);
            r.AddIngredient(ItemID.JungleGrassSeeds, 5);
            r.AddTile(TileID.Loom);
            r.SetResult(ItemID.FlowerBoots);
            r.AddRecipe();
        }
        #endregion

        #region Armor
        // Rare uncraftable armors like Eskimo armor
        private static void AddArmorRecipes()
        {
            // Eskimo armor
            ModRecipe r = GetNewRecipe();
            r.AddIngredient(ItemID.Silk, 4);
            r.AddIngredient(ItemID.Leather);
            r.AddIngredient(ItemID.BorealWood, 12);
            r.AddTile(TileID.Loom);
            r.SetResult(ItemID.EskimoHood);
            r.AddRecipe();

            r = GetNewRecipe();
            r.AddIngredient(ItemID.Silk, 8);
            r.AddIngredient(ItemID.Leather);
            r.AddIngredient(ItemID.BorealWood, 18);
            r.AddTile(TileID.Loom);
            r.SetResult(ItemID.EskimoCoat);
            r.AddRecipe();

            r = GetNewRecipe();
            r.AddIngredient(ItemID.Silk, 6);
            r.AddIngredient(ItemID.Leather);
            r.AddIngredient(ItemID.BorealWood, 15);
            r.AddTile(TileID.Loom);
            r.SetResult(ItemID.EskimoPants);
            r.AddRecipe();

            // Pharaoh set
            r = GetNewRecipe();
            r.AddIngredient(ItemID.AncientCloth, 3);
            r.AddTile(TileID.Loom);
            r.SetResult(ItemID.PharaohsMask);
            r.AddRecipe();

            r = GetNewRecipe();
            r.AddIngredient(ItemID.AncientCloth, 4);
            r.AddTile(TileID.Loom);
            r.SetResult(ItemID.PharaohsRobe);
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
            r.AddTile(TileID.Anvils);
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
            r.AddIngredient(ItemID.SoulofNight, 15);
            r.AddIngredient(ItemID.Lens, 3);
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
            r.AddRecipeGroup("AnyCobaltBar", 5);
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
            r.AddRecipeGroup("AnyCobaltBar", 3);
            r.AddTile(TileID.Anvils);
            r.SetResult(ItemID.CelestialMagnet);
            r.AddRecipe();

            // Frozen Turtle Shell
            r = GetNewRecipe();
            r.AddIngredient(ItemID.TurtleShell, 3);
            r.AddIngredient(ModContent.ItemType<EssenceofEleum>(), 9);
            r.AddTile(TileID.IceMachine);
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

            // Terra Blade w/ True Bloody Edge
            r = GetNewRecipe();
            r.AddIngredient(ModContent.ItemType<TrueBloodyEdge>());
            r.AddIngredient(ItemID.TrueExcalibur);
            r.AddIngredient(ModContent.ItemType<LivingShard>(), 7);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(ItemID.TerraBlade);
            r.AddRecipe();

            // Turtle Shell with Giant Tortoise Shell
            r = GetNewRecipe();
            r.AddIngredient(ModContent.ItemType<GiantTortoiseShell>());
            r.SetResult(ItemID.TurtleShell);
            r.AddRecipe();
        }
        #endregion

        public static void AddRecipeGroups()
        {
            //Modify Vanilla Recipe Groups
            RecipeGroup firefly = RecipeGroup.recipeGroups[RecipeGroup.recipeGroupIDs["Fireflies"]];
            firefly.ValidItems.Add(ModContent.ItemType<TwinklerItem>());

            RecipeGroup sand = RecipeGroup.recipeGroups[RecipeGroup.recipeGroupIDs["Sand"]];
            sand.ValidItems.Add(ModContent.ItemType<AstralSand>());
            sand.ValidItems.Add(ModContent.ItemType<EutrophicSand>());
            sand.ValidItems.Add(ModContent.ItemType<SulphurousSand>());

            RecipeGroup wood = RecipeGroup.recipeGroups[RecipeGroup.recipeGroupIDs["Wood"]];
            wood.ValidItems.Add(ModContent.ItemType<Acidwood>()); //Astral Monolith was decidedly not wood-like enough

            //New Groups
            RecipeGroup group = new RecipeGroup(() => "Any Copper Bar", new int[]
            {
                ItemID.CopperBar,
                ItemID.TinBar
            });
            RecipeGroup.RegisterGroup("AnyCopperBar", group);

            group = new RecipeGroup(() => "Any Gold Ore", new int[]
            {
                ItemID.GoldOre,
                ItemID.PlatinumOre
            });
            RecipeGroup.RegisterGroup("AnyGoldOre", group);

            group = new RecipeGroup(() => "Any Gold Bar", new int[]
            {
                ItemID.GoldBar,
                ItemID.PlatinumBar
            });
            RecipeGroup.RegisterGroup("AnyGoldBar", group);

            group = new RecipeGroup(() => "Any Evil Block", new int[]
            {
                ItemID.EbonstoneBlock,
                ItemID.CrimstoneBlock
            });
            RecipeGroup.RegisterGroup("AnyEvilBlock", group);

            group = new RecipeGroup(() => "Any Evil Bar", new int[]
            {
                ItemID.DemoniteBar,
                ItemID.CrimtaneBar
            });
            RecipeGroup.RegisterGroup("AnyEvilBar", group);

            group = new RecipeGroup(() => "Any Cobalt Bar", new int[]
            {
                ItemID.CobaltBar,
                ItemID.PalladiumBar
            });
            RecipeGroup.RegisterGroup("AnyCobaltBar", group);

            group = new RecipeGroup(() => "Any Mythril Bar", new int[]
            {
                ItemID.MythrilBar,
                ItemID.OrichalcumBar
            });
            RecipeGroup.RegisterGroup("AnyMythrilBar", group);

            group = new RecipeGroup(() => "Any Adamantite Bar", new int[]
            {
                ItemID.AdamantiteBar,
                ItemID.TitaniumBar
            });
            RecipeGroup.RegisterGroup("AnyAdamantiteBar", group);

            group = new RecipeGroup(() => "Any Evil Powder", new int[]
            {
                ItemID.VilePowder,
                ItemID.ViciousPowder
            });
            RecipeGroup.RegisterGroup("EvilPowder", group);

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

            group = new RecipeGroup(() => "Any Evil Flask", new int[]
            {
                ItemID.FlaskofCursedFlames,
                ItemID.FlaskofIchor
            });
            RecipeGroup.RegisterGroup("AnyEvilFlask", group);

            group = new RecipeGroup(() => "Any Evil Water", new int[]
            {
                ItemID.UnholyWater,
                ItemID.BloodWater
            });
            RecipeGroup.RegisterGroup("AnyEvilWater", group);

            group = new RecipeGroup(() => "Any Ice Block", new int[]
            {
                ItemID.IceBlock,
                ItemID.PurpleIceBlock,
                ItemID.RedIceBlock,
                ItemID.PinkIceBlock,
                ModContent.ItemType<AstralIce>()
            });
            RecipeGroup.RegisterGroup("AnyIceBlock", group);

            group = new RecipeGroup(() => "Any Snow Block", new int[]
            {
                ItemID.SnowBlock,
                ModContent.ItemType<AstralSnow>()
            });
            RecipeGroup.RegisterGroup("AnySnowBlock", group);

            group = new RecipeGroup(() => "Any Silt", new int[]
            {
                ItemID.SiltBlock,
                ItemID.SlushBlock,
                ModContent.ItemType<AstralSilt>()
            });
            RecipeGroup.RegisterGroup("SiltGroup", group);

            group = new RecipeGroup(() => "Any Hallowed Helmet", new int[]
            {
                ItemID.HallowedHelmet,
                ItemID.HallowedHeadgear,
                ItemID.HallowedMask/*,
                ItemID.HallowedHood,
                ItemID.AncientHallowedHelmet,
                ItemID.AncientHallowedHeadgear,
                ItemID.AncientHallowedMask,
                ItemID.AncientHallowedHood*/
            });
            RecipeGroup.RegisterGroup("AnyHallowedHelmet", group);

            /*group = new RecipeGroup(() => "Any Hallowed Platemail", new int[]
            {
                ItemID.HallowedPlateMail,
                ItemID.AncientHallowedPlateMail
            });
            RecipeGroup.RegisterGroup("AnyHallowedPlatemail", group);

            group = new RecipeGroup(() => "Any Hallowed Greaves", new int[]
            {
                ItemID.HallowedGreaves,
                ItemID.AncientHallowedGreaves
            });
            RecipeGroup.RegisterGroup("AnyHallowedGreaves", group);*/

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
                ItemID.StardustPickaxe,
                ModContent.ItemType<GallantPickaxe>()
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

            group = new RecipeGroup(() => "Any Large Gem", new int[]
            {
                ItemID.LargeAmber,
                ItemID.LargeAmethyst,
                ItemID.LargeDiamond,
                ItemID.LargeEmerald,
                ItemID.LargeRuby,
                ItemID.LargeSapphire,
                ItemID.LargeTopaz
            });
            RecipeGroup.RegisterGroup("AnyLargeGem", group);

            group = new RecipeGroup(() => "Any Food Item", new int[]
            {
                ItemID.CookedFish,
                ItemID.CookedMarshmallow,
                ItemID.PadThai,
                ItemID.Pho,
                ItemID.CookedShrimp,
                ItemID.Sashimi,
                ItemID.Bacon,
                ItemID.BowlofSoup,
                ItemID.GrubSoup,
                ItemID.GingerbreadCookie,
                ItemID.SugarCookie,
                ItemID.ChristmasPudding,
                ItemID.PumpkinPie,
                ModContent.ItemType<Baguette>(),
                ModContent.ItemType<DeliciousMeat>(),
                ModContent.ItemType<SunkenStew>()
            });
            RecipeGroup.RegisterGroup("AnyFood", group);

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
                ItemID.BejeweledValkyrieWing,
                /*
                ItemID.GhostarsWings,
                ItemID.GroxTheGreatWings,
                ItemID.FoodBarbarianWings,
                ItemID.SafemanWings,
                ItemID.CreativeWings,
                ItemID.RainbowWings,
                ItemID.LongRainbowTrailWings,
                */
                ModContent.ItemType<SkylineWings>(),
                ModContent.ItemType<StarlightWings>(),
                ModContent.ItemType<AureateWings>(),
                ModContent.ItemType<DiscordianWings>(),
                ModContent.ItemType<TarragonWings>(),
                ModContent.ItemType<XerocWings>(),
                ModContent.ItemType<HadarianWings>(),
                ModContent.ItemType<SilvaWings>()
            });
            RecipeGroup.RegisterGroup("WingsGroup", group);
        }
    }
}
