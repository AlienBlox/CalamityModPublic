﻿using System.Collections.Generic;
using CalamityMod.Balancing;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    // TODO -- The item tweaks database and functions should be stored in a ModSystem.
    // ApplyTweaks(ref Item item) would be the one exposed function, which CalamityGlobalItem would call in SetDefaults.
    public partial class CalamityGlobalItem : GlobalItem
    {
        #region Database and Initialization
        internal static SortedDictionary<int, IItemTweak[]> currentTweaks = null;

        internal static void LoadTweaks()
        {
            // Various shorthands for items which receive very simple changes, such as setting one flag.
            IItemTweak[] trueMelee = Do(TrueMelee);
            IItemTweak[] trueMeleeNoSpeed = Do(TrueMeleeNoSpeed); 
            IItemTweak[] pointBlank = Do(PointBlank);
            IItemTweak[] autoReuse = Do(AutoReuse);
            IItemTweak[] maxStack999 = Do(MaxStack(999));
            IItemTweak[] nonConsumableBossSummon = Do(MaxStack(1), NotConsumable, UseTimeExact(10));

            // Please keep this strictly alphabetical. It's the only way to keep it sane. Thanks in advance.
            // - Ozzatron
            currentTweaks = new SortedDictionary<int, IItemTweak[]>
            {
                { ItemID.Abeemination, nonConsumableBossSummon },
                { ItemID.AbigailsFlower, autoReuse },
                { ItemID.AdamantiteChainsaw, Do(TrueMeleeNoSpeed, AxePower(90), UseTimeExact(4), TileBoostExact(+0)) },
                { ItemID.AdamantiteDrill, Do(TrueMeleeNoSpeed, PickPower(180), UseTimeExact(4), TileBoostExact(+1)) },
                { ItemID.AdamantiteGlaive, Do(AutoReuse, TrueMelee, UseRatio(0.8f), DamageExact(65), ShootSpeedRatio(1.25f)) },
                { ItemID.AdamantitePickaxe, Do(PickPower(180), UseTimeExact(8), TileBoostExact(+1)) },
                { ItemID.AdamantiteRepeater, Do(PointBlank, UseExact(18), DamageExact(61)) },
                { ItemID.AdamantiteSword, Do(UseTurn, ScaleRatio(1.5f), UseRatio(0.8f), DamageExact(77)) },
                { ItemID.AdamantiteWaraxe, Do(AxePower(160), UseTimeExact(10), TileBoostExact(+1)) },
                { ItemID.Amarok, autoReuse },
                { ItemID.AmberStaff, Do(DamageExact(19), UseTimeExact(15), UseAnimationExact(40)) },
                { ItemID.AmethystStaff, Do(ManaExact(2)) },
                { ItemID.Anchor, Do(DamageExact(107), UseExact(30)) },
                { ItemID.AncientHallowedGreaves, Do(DefenseDelta(+2)) },
                { ItemID.AncientHallowedPlateMail, Do(DefenseDelta(+3)) },
                { ItemID.AnkhShield, Do(DefenseDelta(+8)) },
                { ItemID.AntlionClaw, Do(ScaleRatio(1.5f), DamageRatio(1.5f), UseExact(14)) },
                { ItemID.AquaScepter, Do(DamageExact(21), ShootSpeedExact(25f)) },
                { ItemID.Arkhalis, trueMeleeNoSpeed },
                { ItemID.BabyBirdStaff, Do(AutoReuse, UseExact(35)) },
                { ItemID.Bananarang, Do(DamageExact(98), UseExact(14)) },
                { ItemID.BatBat, autoReuse },
                { ItemID.BatScepter, Do(DamageExact(56)) },
                { ItemID.BeamSword, Do(AutoReuse, UseMeleeSpeed, DamageExact(360), UseAnimationExact(60), ShootSpeedExact(23f)) },
                { ItemID.BeeGun, Do(DamageExact(11)) },
                { ItemID.BeeKeeper, Do(UseTurn, ScaleRatio(1.5f), DamageExact(60)) },
                { ItemID.BeesKnees, Do(PointBlank, DamageExact(18), UseExact(38)) },
                { ItemID.BladedGlove, Do(DamageExact(15), UseExact(7)) },
                { ItemID.BladeofGrass, Do(AutoReuse, UseTurn, ScaleRatio(1.8f), DamageExact(45), UseExact(33)) },
                { ItemID.Bladetongue, Do(UseTurn, UseRatio(0.8f), DamageExact(120), ScaleRatio(1.75f)) },
                { ItemID.BlizzardStaff, Do(DamageExact(41), ManaExact(7)) },
                { ItemID.BloodButcherer, Do(AutoReuse, UseTurn, DamageRatio(1.66f), ScaleRatio(1.4f)) },
                { ItemID.BloodLustCluster, Do(AxePower(100), UseTimeExact(13), TileBoostExact(+0)) },
                { ItemID.BloodMoonStarter, nonConsumableBossSummon },
                { ItemID.BloodyMachete, Do(AutoReuse, DamageExact(30)) },
                { ItemID.BloodySpine, nonConsumableBossSummon },
                { ItemID.Blowgun, Do(PointBlank, DamageExact(40)) },
                { ItemID.Blowpipe, Do(PointBlank, DamageExact(16), UseExact(35)) },
                { ItemID.BluePhaseblade, Do(AutoReuse, UseTurn, ScaleRatio(1.5f), DamageExact(51)) },
                { ItemID.BluePhasesaber, Do(ScaleRatio(1.5f), DamageExact(72), UseExact(20)) },
                { ItemID.BlueSolution, Do(Value(Item.buyPrice(silver: 5))) },
                { ItemID.BoneArrow, Do(DamageExact(7)) },
                { ItemID.BonePickaxe, Do(PickPower(55), UseTimeExact(6)) },
                { ItemID.BoneSword, Do(AutoReuse, UseTurn, ScaleRatio(1.5f), DamageRatio(1.25f)) },
                { ItemID.BoneWhip, autoReuse },
                { ItemID.BookofSkulls, Do(ManaExact(12), ShootSpeedExact(5.5f)) },
                { ItemID.BookStaff, Do(ManaExact(14)) }, // Tome of Infinite Wisdom
                { ItemID.Boomstick, Do(PointBlank, DamageExact(11)) },
                { ItemID.BorealWoodBow, Do(PointBlank, DamageRatio(1.1f)) },
                { ItemID.BorealWoodHammer, Do(HammerPower(25), UseTimeExact(11), TileBoostExact(+0)) },
                { ItemID.BorealWoodSword, Do(AutoReuse, UseTurn) },
                { ItemID.BouncyBomb, maxStack999 },
                { ItemID.BouncyDynamite, maxStack999 },
                { ItemID.BreakerBlade, Do(AutoReuse, UseTurn, ScaleRatio(1.5f), DamageExact(97)) },
                { ItemID.ButchersChainsaw, Do(TrueMeleeNoSpeed, AxePower(150), UseTimeExact(3), TileBoostExact(+0)) },
                { ItemID.CactusPickaxe, Do(PickPower(34), UseTimeExact(9)) },
                { ItemID.CactusSword, Do(AutoReuse, UseTurn) },
                { ItemID.CnadyCanePickaxe, Do(PickPower(55), UseTimeExact(9), TileBoostExact(+1)) }, // Candy Cane Pickaxe, intentionally not in alphabetical order to correct for typo
                { ItemID.CandyCaneSword, Do(AutoReuse, UseTurn, DamageExact(24)) },
                { ItemID.CandyCornRifle, pointBlank },
                { ItemID.Cascade, Do(AutoReuse, DamageExact(39)) },
                { ItemID.CelestialSigil, nonConsumableBossSummon },
                { ItemID.ChainGuillotines, Do(DamageExact(100)) },
                { ItemID.ChainGun, pointBlank },
                { ItemID.ChainKnife, Do(AutoReuse, DamageExact(14)) },
                // Charged Blaster Cannon is now an earlier Last Prism-like, so it will probably need careful balance attention.
                // { ItemID.ChargedBlasterCannon, Do(DamageRatio(1.33f)) },
                { ItemID.Chik, autoReuse },
                { ItemID.ChlorophyteArrow, Do(DamageRatio(1.1f)) },
                { ItemID.ChlorophyteChainsaw, Do(TrueMeleeNoSpeed, AxePower(120), UseTimeExact(3), TileBoostExact(+0)) },
                { ItemID.ChlorophyteClaymore, Do(UseMeleeSpeed) },
                { ItemID.ChlorophyteDrill, Do(TrueMeleeNoSpeed, PickPower(200), UseTimeExact(4), TileBoostExact(+2)) },
                { ItemID.ChlorophyteGreataxe, Do(AxePower(165), UseTimeExact(7), TileBoostExact(+2)) },
                { ItemID.ChlorophyteJackhammer, Do(TrueMeleeNoSpeed, HammerPower(90), UseTimeExact(5), TileBoostExact(+0)) },
                { ItemID.ChlorophytePartisan, Do(AutoReuse, UseMeleeSpeed, UseRatio(0.8f), DamageExact(100)) },
                { ItemID.ChlorophytePickaxe, Do(PickPower(200), UseTimeExact(7), TileBoostExact(+2)) },
                { ItemID.ChlorophyteSaber, Do(UseMeleeSpeed, DamageExact(80), UseExact(10)) },
                { ItemID.ChlorophyteShotbow, Do(PointBlank, DamageExact(80), UseExact(50)) },
                { ItemID.ChlorophyteWarhammer, Do(HammerPower(90), UseTimeExact(8), TileBoostExact(+2)) },
                { ItemID.ChristmasTreeSword, Do(AutoReuse, UseTurn, UseMeleeSpeed, DamageExact(114)) },
                { ItemID.ClingerStaff, Do(DamageExact(118)) },
                { ItemID.ClockworkAssaultRifle, Do(PointBlank, DamageExact(21)) },
                { ItemID.CobaltBreastplate, Do(DefenseDelta(+3)) },
                { ItemID.CobaltChainsaw, Do(TrueMeleeNoSpeed, AxePower(70), UseTimeExact(4), TileBoostExact(-1)) },
                { ItemID.CobaltDrill, Do(TrueMeleeNoSpeed, PickPower(130), UseTimeExact(5)) },
                { ItemID.CobaltHat, Do(DefenseDelta(+2)) },
                { ItemID.CobaltHelmet, Do(DefenseDelta(+4)) },
                { ItemID.CobaltLeggings, Do(DefenseDelta(+2)) },
                { ItemID.CobaltMask, Do(DefenseDelta(+3)) },
                { ItemID.CobaltNaginata, Do(AutoReuse, TrueMelee, UseRatio(0.8f), DamageExact(90)) },
                { ItemID.CobaltPickaxe, Do(PickPower(130), UseTimeExact(9)) },
                { ItemID.CobaltRepeater, Do(PointBlank, DamageExact(50), UseExact(20)) },
                { ItemID.CobaltShield, Do(DefenseDelta(+3)) },
                { ItemID.CobaltSword, Do(UseTurn, ScaleRatio(1.5f), UseRatio(0.8f), DamageExact(80)) },
                { ItemID.CobaltWaraxe, Do(AxePower(125), UseTimeExact(12), TileBoostExact(+0)) },
                { ItemID.Code1, Do(AutoReuse, DamageExact(25)) },
                { ItemID.Code2, autoReuse },
                { ItemID.CoolWhip, autoReuse },
                { ItemID.CopperAxe, Do(AxePower(50), UseTimeExact(16), TileBoostExact(+0)) },
                { ItemID.CopperBow, Do(PointBlank, DamageRatio(1.1f)) },
                { ItemID.CopperBroadsword, Do(AutoReuse, UseTurn) },
                { ItemID.CopperHammer, Do(HammerPower(35), UseTimeExact(12), TileBoostExact(+0)) },
                { ItemID.CopperPickaxe, Do(PickPower(35), UseTimeExact(10), TileBoostExact(+0)) },
                { ItemID.CopperShortsword, Do(AutoReuse, TrueMelee) },
                { ItemID.CorruptYoyo, Do(AutoReuse, DamageExact(27)) },
                { ItemID.CrimsonYoyo, Do(AutoReuse, DamageExact(30)) },
                { ItemID.CrystalDart, Do(DamageExact(20)) },
                { ItemID.CrystalSerpent, Do(DamageExact(45)) },
                { ItemID.CrystalStorm, Do(DamageExact(40))},
                { ItemID.CursedArrow, Do(DamageRatio(1.1f)) },
                { ItemID.CursedBullet, Do(DamageExact(16)) },
                { ItemID.CursedDart, Do(DamageExact(25)) },
                { ItemID.Cutlass, Do(UseRatio(0.8f), DamageRatio(2f)) },
                // Vanilla 1.4 nerfed Daedalus Stormbow themselves. If further nerfs are needed, apply them here.
                // { ItemID.DaedalusStormbow, Do(DamageRatio(0.9f)) },
                { ItemID.DaoofPow, Do(DamageExact(160)) },
                { ItemID.DarkBlueSolution, Do(Value(Item.buyPrice(silver: 5))) },
                { ItemID.DarkLance, Do(AutoReuse, TrueMelee, DamageExact(68)) },
                { ItemID.DartPistol, pointBlank },
                { ItemID.DartRifle, Do(PointBlank, DamageExact(58)) },
                // if Daybreak still needs a buff after the 1.4 explosion change, apply it here
                // { ItemID.DayBreak, Do(DamageRatio(1.0f)) }
                { ItemID.DD2BallistraTowerT1Popper, Do(AutoReuse, UseExact(30)) }, // Ballista Tier 1
                { ItemID.DD2BallistraTowerT2Popper, Do(AutoReuse, UseExact(25)) }, // Ballista Tier 2
                { ItemID.DD2BallistraTowerT3Popper, Do(AutoReuse, UseExact(20)) }, // Ballista Tier 3
                { ItemID.DD2BetsyBow, Do(DamageRatio(1.1f)) }, // Aerial Bane's ridiculous multiplier is removed, so this compensates for that
                { ItemID.DD2ExplosiveTrapT1Popper, Do(AutoReuse, UseExact(30)) }, // Explosive Trap Tier 1
                { ItemID.DD2ExplosiveTrapT2Popper, Do(AutoReuse, UseExact(25)) }, // Explosive Trap Tier 2
                { ItemID.DD2ExplosiveTrapT3Popper, Do(AutoReuse, UseExact(20)) }, // Explosive Trap Tier 3
                { ItemID.DD2FlameburstTowerT1Popper, Do(AutoReuse, UseExact(30)) }, // Flameburst Tier 1
                { ItemID.DD2FlameburstTowerT2Popper, Do(AutoReuse, UseExact(25)) }, // Flameburst Tier 2
                { ItemID.DD2FlameburstTowerT3Popper, Do(AutoReuse, UseExact(20)) }, // Flameburst Tier 3
                { ItemID.DD2LightningAuraT1Popper, Do(AutoReuse, UseExact(30)) }, // Lightning Aura Tier 1
                { ItemID.DD2LightningAuraT2Popper, Do(AutoReuse, UseExact(25)) }, // Lightning Aura Tier 2
                { ItemID.DD2LightningAuraT3Popper, Do(AutoReuse, UseExact(20)) }, // Lightning Aura Tier 3
                { ItemID.DD2PhoenixBow, Do(PointBlank, UseExact(18)) }, // Phantom Phoenix
                { ItemID.DD2SquireBetsySword, Do(UseMeleeSpeed) }, // Flying Dragon
                { ItemID.DD2SquireDemonSword, Do(DamageExact(110), UseExact(25)) }, // Brand of the Inferno
                { ItemID.DeadlySphereStaff, Do(AutoReuse, UseExact(20)) },
                { ItemID.DeathbringerPickaxe, Do(PickPower(70), UseTimeExact(10)) },
                { ItemID.DeathSickle, Do(UseMeleeSpeed, DamageExact(82), ShootSpeedExact(15f)) },
                { ItemID.DeerThing, nonConsumableBossSummon },
                { ItemID.DemonBow, Do(PointBlank, DamageExact(12), AutoReuse) },
                { ItemID.DemonScythe, Do(AutoReuse, DamageExact(33), ManaExact(10)) },
                { ItemID.DiamondStaff, Do(DamageExact(26)) },
                { ItemID.Drax, Do(TrueMeleeNoSpeed, PickPower(200), AxePower(110), UseTimeExact(4), TileBoostExact(+1)) },
                { ItemID.DyeTradersScimitar, Do(AutoReuse, UseTurn, ScaleRatio(1.5f), DamageRatio(1.33f)) },
                { ItemID.Dynamite, maxStack999 },
                { ItemID.EbonwoodBow, Do(PointBlank, DamageRatio(1.1f)) },
                { ItemID.EbonwoodHammer, Do(HammerPower(25), UseTimeExact(9), TileBoostExact(+0)) },
                { ItemID.EbonwoodSword, Do(AutoReuse, UseTurn) },
                { ItemID.ElectrosphereLauncher, Do(DamageRatio(1.1f)) },
                { ItemID.EldMelter, Do(DamageExact(113), ShootSpeedDelta(+3f)) }, // Elf Melter, intentionally not in alphabetical order to correct for typo
                { ItemID.EmeraldStaff, Do(DamageExact(28)) },
                { ItemID.EmpressBlade, Do(AutoReuse, DamageExact(60), UseExact(20)) }, // Terraprisma
                { ItemID.EnchantedBoomerang, Do(DamageRatio(2f), UseExact(28)) },
                { ItemID.EnchantedSword, Do(UseMeleeSpeed, ScaleRatio(1.5f), DamageExact(42), UseAnimationExact(20), ShootSpeedExact(15f)) },
                { ItemID.EndlessQuiver, Do(DamageRatio(1.1f)) },
                { ItemID.EoCShield, Do(DefenseDelta(+1)) },
                { ItemID.FairyQueenRangedItem, pointBlank },
                { ItemID.Excalibur, Do(UseTurn, ScaleRatio(1.5f), UseRatio(0.8f), DamageExact(125), UseAnimationExact(45)) },
                { ItemID.FalconBlade, Do(UseTurn, ScaleRatio(1.5f), DamageExact(35), UseExact(15)) },
                { ItemID.FieryGreatsword, Do(AutoReuse, UseTurn, ScaleRatio(1.5f), DamageExact(90), UseExact(45)) },
                { ItemID.FireWhip, autoReuse },
                // Unsure what to do with Celebration. Should it be treated as a serious weapon or not? Currently not changing it from vanilla.
                // { ItemID.FireworksLauncher, Do(DamageRatio(2f)) }, // Celebration
                { ItemID.Flamarang, Do(DamageExact(45)) },
                { ItemID.Flamelash, Do(DamageRatio(1.25f)) },
                { ItemID.Flamethrower, Do(DamageExact(47), ShootSpeedDelta(+3f)) },
                { ItemID.FlamingArrow, Do(DamageRatio(1.1f)) },
                { ItemID.FlareGun, pointBlank },
                { ItemID.FleshGrinder, Do(HammerPower(70), UseTimeExact(13), TileBoostExact(+0)) },
                { ItemID.FlintlockPistol, pointBlank },
                { ItemID.FlinxStaff, Do(AutoReuse, UseExact(35)) },
                { ItemID.FlowerofFire, Do(AutoReuse, ManaExact(7), UseExact(14)) },
                { ItemID.FlowerofFrost, Do(AutoReuse, ManaExact(7), UseExact(30), DamageExact(70), ShootSpeedExact(14)) },
                { ItemID.FlyingKnife, Do(DamageExact(70)) },
                { ItemID.FormatC, autoReuse },
                { ItemID.Frostbrand, Do(UseMeleeSpeed, DamageExact(97), UseExact(23)) },
                { ItemID.FrostburnArrow, Do(DamageExact(8)) },
                { ItemID.FrostStaff, Do(DamageExact(160), UseExact(37), ShootSpeedExact(20f)) }, // has 1 extra update
                { ItemID.FrozenShield, Do(DefenseDelta(+7)) },
                { ItemID.FrozenTurtleShell, Do(DefenseExact(6)) },
                { ItemID.FruitcakeChakram, Do(DamageRatio(2f)) },
                { ItemID.Gatligator, Do(PointBlank, UseExact(6)) },
                { ItemID.Gladius, Do(AutoReuse, TrueMelee) },
                { ItemID.GoblinBattleStandard, nonConsumableBossSummon },
                { ItemID.GoldAxe, Do(AxePower(80), UseTimeExact(14), TileBoostExact(+0)) },
                { ItemID.GoldBow, Do(PointBlank, DamageExact(12)) },
                { ItemID.GoldBroadsword, Do(AutoReuse, UseTurn, ScaleRatio(1.5f), DamageRatio(2f)) },
                { ItemID.GoldenShower, Do(DamageExact(44)) },
                { ItemID.GoldHammer, Do(HammerPower(60), UseTimeExact(9), TileBoostExact(+0)) },
                { ItemID.GoldPickaxe, Do(PickPower(55), UseTimeExact(9)) },
                { ItemID.GoldShortsword, Do(AutoReuse, TrueMelee, DamageRatio(2f)) },
                { ItemID.GolemFist, Do(DamageExact(150)) },
                { ItemID.Gradient, autoReuse },
                { ItemID.GreenPhaseblade, Do(AutoReuse, UseTurn, ScaleRatio(1.5f), DamageExact(51)) },
                { ItemID.GreenPhasesaber, Do(ScaleRatio(1.5f), DamageExact(72), UseExact(20)) },
                { ItemID.GreenSolution, Do(Value(Item.buyPrice(silver: 5))) },
                { ItemID.GrenadeLauncher, Do(DamageRatio(1.5f)) },
                { ItemID.GuideVoodooDoll, maxStack999 },
                { ItemID.Gungnir, Do(AutoReuse, TrueMelee, UseRatio(0.8f), DamageExact(92), ShootSpeedRatio(1.25f)) },
                { ItemID.HallowedGreaves, Do(DefenseDelta(+2)) },
                { ItemID.HallowedPlateMail, Do(DefenseDelta(+3)) },
                { ItemID.HallowedRepeater, Do(PointBlank, UseExact(14), DamageExact(57)) },
                { ItemID.HallowJoustingLance, trueMelee },
                { ItemID.Hammush, Do(HammerPower(85), UseTimeExact(10), TileBoostExact(+1)) },
                { ItemID.Handgun, Do(PointBlank, UseExact(27), DamageExact(36)) },
                { ItemID.Harpoon, pointBlank },
                { ItemID.HelFire, autoReuse },
                { ItemID.HellfireArrow, Do(DamageRatio(1.1f)) },
                { ItemID.HellwingBow, Do(PointBlank, DamageExact(16)) },
                { ItemID.HeroShield, Do(DefenseDelta(+10)) },
                { ItemID.HighVelocityBullet, Do(DamageExact(15)) },
                { ItemID.HolyArrow, Do(DamageRatio(1.1f)) },
                { ItemID.HornetStaff, Do(AutoReuse, UseExact(30)) },
                { ItemID.IceBlade, Do(UseMeleeSpeed, DamageExact(26), UseTimeExact(33)) },
                { ItemID.IceBoomerang, Do(DamageExact(28), UseExact(25), ShootSpeedExact(10)) },
                { ItemID.IceBow, Do(PointBlank, AutoReuse, DamageExact(120), UseExact(35)) },
                { ItemID.IceRod, Do(UseExact(6), DamageExact(30), ShootSpeedExact(20)) },
                { ItemID.IceSickle, Do(AutoReuse, UseMeleeSpeed, DamageExact(95), ShootSpeedExact(20f)) },
                { ItemID.IchorArrow, Do(DamageExact(15)) },
                { ItemID.ImpStaff, Do(AutoReuse, UseExact(30)) },
                { ItemID.InfernoFork, Do(DamageRatio(1.66f)) },
                { ItemID.InfluxWaver, Do(UseMeleeSpeed, DamageRatio(0.75f)) },
                { ItemID.IronAxe, Do(AxePower(60), UseTimeExact(15), TileBoostExact(+0)) },
                { ItemID.IronBow, Do(PointBlank, DamageRatio(1.1f)) },
                { ItemID.IronBroadsword, Do(AutoReuse, UseTurn, DamageRatio(1.25f), ScaleRatio(1.2f)) },
                { ItemID.IronHammer, Do(HammerPower(45), UseTimeExact(11), TileBoostExact(+0)) },
                { ItemID.IronPickaxe, Do(PickPower(40), UseTimeExact(8)) },
                { ItemID.IronShortsword, Do(AutoReuse, TrueMelee, DamageRatio(1.25f)) },
                { ItemID.JestersArrow, Do(DamageExact(6)) },
                { ItemID.JoustingLance, trueMelee },
                { ItemID.JungleYoyo, autoReuse },
                { ItemID.Katana, Do(UseExact(15), DamageRatio(1.5f), CritDelta(+30)) },
                { ItemID.Keybrand, Do(UseTurn, ScaleRatio(1.5f), DamageExact(110)) },
                { ItemID.KOCannon, Do(DamageRatio(2.65f)) },
                { ItemID.Kraken, autoReuse },
                { ItemID.LaserDrill, Do(PickPower(220), AxePower(120), UseTimeExact(4)) },
                { ItemID.LaserRifle, Do(DamageExact(46), UseExact(10), ManaExact(4)) },
                { ItemID.LastPrism, Do(DamageRatio(0.75f)) },
                { ItemID.LavaSkull, Do(DefenseExact(4)) },
                { ItemID.LeadAxe, Do(AxePower(60), UseTimeExact(15), TileBoostExact(+0)) },
                { ItemID.LeadBow, Do(PointBlank, DamageRatio(1.1f)) },
                { ItemID.LeadBroadsword, Do(AutoReuse, UseTurn, DamageRatio(1.25f), ScaleRatio(1.2f)) },
                { ItemID.LeadHammer, Do(HammerPower(45), UseTimeExact(11), TileBoostExact(+0)) },
                { ItemID.LeadPickaxe, Do(PickPower(40), UseTimeExact(8)) },
                { ItemID.LeadShortsword, Do(AutoReuse, TrueMelee, DamageRatio(1.25f)) },
                { ItemID.BlandWhip, autoReuse },
                { ItemID.LifeCrystal, autoReuse },
                { ItemID.LifeFruit, autoReuse },
                { ItemID.LightDisc, Do(DamageExact(128)) },
                { ItemID.LightsBane, Do(AutoReuse, UseTurn, DamageExact(34)) },
                { ItemID.LucyTheAxe, Do(AxePower(150), UseExact(13), TileBoostExact(+1)) },
                { ItemID.LunarFlareBook, Do(DamageRatio(1.5f)) },
                { ItemID.LunarHamaxeNebula, Do(HammerPower(100), AxePower(175), UseTimeExact(5), TileBoostExact(+4)) },
                { ItemID.LunarHamaxeSolar, Do(HammerPower(100), AxePower(175), UseTimeExact(5), TileBoostExact(+4)) },
                { ItemID.LunarHamaxeStardust, Do(HammerPower(100), AxePower(175), UseTimeExact(5), TileBoostExact(+4)) },
                { ItemID.LunarHamaxeVortex, Do(HammerPower(100), AxePower(175), UseTimeExact(5), TileBoostExact(+4)) },
                { ItemID.MaceWhip, autoReuse },
                { ItemID.MagicalHarp, Do(DamageExact(50), ShootSpeedExact(12f)) },
                { ItemID.MagicDagger, Do(AutoReuse, DamageExact(77), UseExact(16), ShootSpeedExact(30)) },
                { ItemID.MagicMissile, Do(DamageRatio(1.2f), ManaExact(5)) },
                { ItemID.MagnetSphere, Do(DamageRatio(1.1f)) },
                { ItemID.ManaCrystal, autoReuse },
                { ItemID.Marrow, Do(PointBlank, AutoReuse, DamageExact(69)) },
                { ItemID.MechanicalEye, nonConsumableBossSummon },
                { ItemID.MechanicalSkull, nonConsumableBossSummon },
                { ItemID.MechanicalWorm, nonConsumableBossSummon },
                { ItemID.MedusaHead, Do(ManaExact(6), DamageRatio(1.2f)) },
                { ItemID.Megashark, pointBlank },
                { ItemID.Meowmere, Do(UseMeleeSpeed/*, DamageRatio(1.33f) */) },
                { ItemID.MeteorHamaxe, Do(HammerPower(70), AxePower(100), UseTimeExact(16), TileBoostExact(+0)) },
                { ItemID.MeteorStaff, Do(DamageExact(58), ManaExact(7), ShootSpeedExact(13f)) },
                { ItemID.Minishark, Do(PointBlank, DamageExact(4)) },
                { ItemID.MoltenFury, Do(PointBlank, DamageRatio(1.1f), UseExact(29), AutoReuse) },
                { ItemID.MoltenHamaxe, Do(HammerPower(75), AxePower(125), UseTimeExact(14), TileBoostExact(+0)) },
                { ItemID.MoltenPickaxe, Do(PickPower(100), UseTimeExact(10)) },
                { ItemID.MoltenSkullRose, Do(DefenseExact(8)) },
                { ItemID.MonkStaffT1, Do(TrueMeleeNoSpeed, DamageExact(83)) }, // Sleepy Octopod
                { ItemID.MonkStaffT2, Do(AutoReuse, TrueMelee, DamageRatio(2f)) }, // Ghastly Glaive
                { ItemID.MonkStaffT3, Do(DamageExact(225)) }, // Sky Dragon's Fury
                { ItemID.MoonlordArrow, Do(DamageRatio(1.1f)) },
                { ItemID.MoonlordTurretStaff, Do(UseExact(15), DamageRatio(1.5f)) },
                { ItemID.Muramasa, Do(ScaleRatio(1.5f), DamageRatio(1.5f), UseExact(14), CritDelta(+30)) },
                { ItemID.MushroomSpear, Do(AutoReuse, TrueMelee, UseRatio(0.8f), DamageExact(100)) },
                { ItemID.Musket, Do(PointBlank, DamageExact(25)) },
                { ItemID.MythrilChainsaw, Do(TrueMeleeNoSpeed, AxePower(80), UseTimeExact(4), TileBoostExact(-1)) },
                { ItemID.MythrilDrill, Do(TrueMeleeNoSpeed, PickPower(160), UseTimeExact(4)) },
                { ItemID.MythrilHalberd, Do(AutoReuse, TrueMelee, UseRatio(0.8f), DamageExact(95), ShootSpeedRatio(1.25f)) },
                { ItemID.MythrilPickaxe, Do(PickPower(160), UseTimeExact(8)) },
                { ItemID.MythrilRepeater, Do(PointBlank, DamageExact(58), UseExact(19)) },
                { ItemID.MythrilSword, Do(UseTurn, ScaleRatio(1.5f), DamageExact(100), UseExact(25)) },
                { ItemID.MythrilWaraxe, Do(AxePower(140), UseTimeExact(11), TileBoostExact(+0)) },
                { ItemID.NaughtyPresent, nonConsumableBossSummon },
                { ItemID.NebulaChainsaw, trueMeleeNoSpeed },
                { ItemID.NebulaDrill, Do(TrueMeleeNoSpeed, PickPower(225), UseTimeExact(3), TileBoostExact(+4)) },
                { ItemID.NebulaPickaxe, Do(PickPower(225), UseTimeExact(6), TileBoostExact(+4)) },
                { ItemID.NettleBurst, Do(ManaExact(10), DamageExact(43)) },
                { ItemID.NightmarePickaxe, Do(PickPower(66), UseTimeExact(9)) },
                { ItemID.NightsEdge, Do(AutoReuse, UseTurn, ScaleRatio(1.5f), DamageExact(98)) },
                { ItemID.NorthPole, Do(AutoReuse, UseMeleeSpeed, AttackSpeedExact(0.33f)) },
                { ItemID.ObsidianShield, Do(DefenseDelta(+5)) },
                { ItemID.ObsidianSkull, Do(DefenseDelta(+1)) },
                { ItemID.ObsidianSkullRose, Do(DefenseExact(4)) },
                { ItemID.ObsidianSwordfish, Do(AutoReuse, TrueMelee, DamageExact(45)) },
                { ItemID.OnyxBlaster, Do(PointBlank, UseTurn) },
                { ItemID.OpticStaff, Do(AutoReuse, UseExact(25), DamageRatio(0.75f)) }, // NOTE: Optic Staff minions have local iframes, so they should be much better overall
                { ItemID.OrangePhaseblade, Do(AutoReuse, UseTurn, ScaleRatio(1.5f), DamageExact(51)) },
                { ItemID.OrangePhasesaber, Do(ScaleRatio(1.5f), DamageExact(72), UseExact(20)) },
                { ItemID.OrichalcumBreastplate, Do(DefenseDelta(+3)) },
                { ItemID.OrichalcumChainsaw, Do(TrueMeleeNoSpeed, AxePower(80), UseTimeExact(4), TileBoostExact(-1)) },
                { ItemID.OrichalcumDrill, Do(TrueMeleeNoSpeed, PickPower(160), UseTimeExact(4)) },
                { ItemID.OrichalcumHalberd, Do(AutoReuse, TrueMelee, UseRatio(0.8f), DamageExact(98), ShootSpeedRatio(1.25f)) },
                { ItemID.OrichalcumHeadgear, Do(DefenseDelta(+2)) },
                { ItemID.OrichalcumHelmet, Do(DefenseDelta(+3)) },
                { ItemID.OrichalcumLeggings, Do(DefenseDelta(+4)) },
                { ItemID.OrichalcumMask, Do(DefenseDelta(+3)) },
                { ItemID.OrichalcumPickaxe, Do(PickPower(160), UseTimeExact(8)) },
                { ItemID.OrichalcumRepeater, Do(PointBlank, DamageExact(86), UseExact(27)) },
                { ItemID.OrichalcumSword, Do(UseTurn, ScaleRatio(1.5f), UseRatio(0.8f), DamageExact(82)) },
                { ItemID.OrichalcumWaraxe, Do(AxePower(140), UseTimeExact(11), TileBoostExact(+0)) },
                { ItemID.PainterPaintballGun, Do(PointBlank, DamageExact(8)) },
                { ItemID.PaladinsHammer, Do(AutoReuse, DamageExact(100)) },
                { ItemID.PaladinsShield, Do(DefenseDelta(+3)) },
                { ItemID.PalladiumBreastplate, Do(DefenseDelta(+3)) },
                { ItemID.PalladiumChainsaw, Do(TrueMeleeNoSpeed, AxePower(70), UseTimeExact(4), TileBoostExact(-1)) },
                { ItemID.PalladiumDrill, Do(TrueMeleeNoSpeed, PickPower(130), UseTimeExact(5)) },
                { ItemID.PalladiumHeadgear, Do(DefenseDelta(+2)) },
                { ItemID.PalladiumHelmet, Do(DefenseDelta(+3)) },
                { ItemID.PalladiumMask, Do(DefenseDelta(+1)) },
                { ItemID.PalladiumLeggings, Do(DefenseDelta(+3)) },
                { ItemID.PalladiumPickaxe, Do(PickPower(130), UseTimeExact(9)) },
                { ItemID.PalladiumPike, Do(AutoReuse, TrueMelee, UseRatio(0.8f), DamageRatio(3f)) },
                { ItemID.PalladiumRepeater, Do(PointBlank, UseExact(25), DamageExact(75)) },
                { ItemID.PalladiumSword, Do(UseTurn, ScaleRatio(1.5f), UseRatio(0.8f), DamageExact(100)) },
                { ItemID.PalladiumWaraxe, Do(AxePower(125), UseTimeExact(12), TileBoostExact(+0)) },
                { ItemID.PalmWoodBow, Do(PointBlank, DamageRatio(1.1f)) },
                { ItemID.PalmWoodHammer, Do(HammerPower(25), UseTimeExact(11), TileBoostExact(+0)) },
                { ItemID.PalmWoodSword, Do(AutoReuse, UseTurn) },
                { ItemID.PaperAirplaneA, autoReuse },
                { ItemID.PaperAirplaneB, autoReuse },
                { ItemID.PearlwoodBow, Do(AutoReuse, PointBlank, DamageRatio(2.31f), UseDelta(+8), ShootSpeedDelta(+3.4f), KnockbackDelta(+1f)) },
                { ItemID.PearlwoodHammer, Do(HammerPower(25), UseTimeExact(4), UseAnimationExact(20), DamageRatio(4f), TileBoostExact(+0)) },
                { ItemID.PearlwoodSword, Do(AutoReuse, UseTurn, DamageRatio(4f)) },
                { ItemID.PewMaticHorn, Do(DamageExact(23)) },
                { ItemID.Phantasm, pointBlank },
                { ItemID.PhoenixBlaster, Do(AutoReuse, PointBlank, DamageExact(36), UseExact(27)) },
                { ItemID.PickaxeAxe, Do(PickPower(200), AxePower(110), UseTimeExact(7), TileBoostExact(+1)) },
                { ItemID.Picksaw, Do(PickPower(210), AxePower(125), UseTimeExact(6), TileBoostExact(+1)) },
                { ItemID.PiercingStarlight, Do(TrueMelee) },
                { ItemID.PirateMap, nonConsumableBossSummon },
                { ItemID.PirateStaff, Do(AutoReuse, UseExact(25)) },
                { ItemID.PlatinumAxe, Do(AxePower(80), UseTimeExact(14), TileBoostExact(+0)) },
                { ItemID.PlatinumBow, Do(PointBlank, DamageExact(13)) },
                { ItemID.PlatinumBroadsword, Do(AutoReuse, UseTurn, ScaleRatio(1.5f), DamageRatio(2f)) },
                { ItemID.PlatinumHammer, Do(HammerPower(60), UseTimeExact(9), TileBoostExact(+0)) },
                { ItemID.PlatinumPickaxe, Do(PickPower(55), UseTimeExact(9)) },
                { ItemID.PlatinumShortsword, Do(AutoReuse, TrueMelee, DamageRatio(2f)) },
                { ItemID.PoisonStaff, Do(DamageExact(57)) },
                { ItemID.ProximityMineLauncher, Do(DamageRatio(4f), UseRatio(0.8f)) },
                { ItemID.PsychoKnife, Do(UseTurn, UseExact(11), AttackSpeedExact(0.5f), DamageRatio(3f)) },
                { ItemID.PulseBow, Do(PointBlank, DamageRatio(1.2f)) },
                { ItemID.PumpkinMoonMedallion, nonConsumableBossSummon },
                { ItemID.PurpleClubberfish, Do(UseTurn, ScaleRatio(1.5f), DamageExact(45), KnockbackExact(10f)) },
                { ItemID.PurplePhaseblade, Do(AutoReuse, UseTurn, ScaleRatio(1.5f), DamageExact(51)) },
                { ItemID.PurplePhasesaber, Do(ScaleRatio(1.5f), DamageExact(72), UseExact(20)) },
                { ItemID.PurpleSolution, Do(Value(Item.buyPrice(silver: 5))) },
                { ItemID.Pwnhammer, Do(HammerPower(80), UseTimeExact(11), TileBoostExact(+1)) },
                { ItemID.PygmyStaff, Do(AutoReuse, UseExact(20)) },
                { ItemID.QuadBarrelShotgun, pointBlank },
                { ItemID.QueenSlimeCrystal, nonConsumableBossSummon },
                { ItemID.QueenSpiderStaff, Do(UseExact(25)) },
                { ItemID.RainbowCrystalStaff, Do(UseExact(15)) },
                { ItemID.RainbowRod, Do(DamageExact(50), UseExact(35)) },
                { ItemID.RainbowWhip, autoReuse },
                { ItemID.Rally, Do(AutoReuse, DamageExact(20)) },
                { ItemID.RavenStaff, Do(AutoReuse, UseExact(20)) },
                { ItemID.Razorpine, Do(DamageRatio(0.75f)) },
                { ItemID.ReaverShark, Do(PickPower(100), UseTimeExact(16)) },
                { ItemID.RedPhaseblade, Do(AutoReuse, UseTurn, ScaleRatio(1.5f), DamageExact(51)) },
                { ItemID.RedPhasesaber, Do(ScaleRatio(1.5f), DamageExact(72), UseExact(20)) },
                { ItemID.RedRyder, Do(PointBlank, DamageExact(24)) },
                { ItemID.RedSolution, Do(Value(Item.buyPrice(silver: 5))) },
                { ItemID.RedsYoyo, autoReuse },
                { ItemID.Revolver, Do(PointBlank, AutoReuse) },
                { ItemID.RichMahoganyBow, Do(PointBlank, DamageRatio(1.1f)) },
                { ItemID.RichMahoganyHammer, Do(HammerPower(25), UseTimeExact(10), TileBoostExact(+0)) },
                { ItemID.RichMahoganySword, Do(AutoReuse, UseTurn) },
                { ItemID.RocketIII, Do(DamageRatio(0.75f)) },
                { ItemID.RocketIV, Do(DamageRatio(0.75f)) },
                { ItemID.RocketLauncher, Do(DamageRatio(1.1f)) },
                { ItemID.Rockfish, Do(HammerPower(50), UseTimeExact(10), TileBoostExact(+0)) },
                { ItemID.RubyStaff, Do(DamageExact(25)) },
                { ItemID.Ruler, Do(TrueMelee) },
                { ItemID.Sandgun, pointBlank },
                { ItemID.SapphireStaff, Do(AutoReuse, DamageExact(25)) },
                { ItemID.SanguineStaff, Do(AutoReuse, UseExact(25)) },
                { ItemID.SawtoothShark, Do(TrueMeleeNoSpeed, AxePower(45), UseTimeExact(4), TileBoostExact(-1)) },
                { ItemID.ScytheWhip, autoReuse },
                { ItemID.SDMG, pointBlank },
                { ItemID.Seedler, Do(UseMeleeSpeed, DamageRatio(1.5f)) },
                { ItemID.Shackle, Do(DefenseDelta(+2)) },
                { ItemID.ShadewoodBow, Do(PointBlank, DamageRatio(1.1f)) },
                { ItemID.ShadewoodHammer, Do(HammerPower(25), UseTimeExact(9), TileBoostExact(+0)) },
                { ItemID.ShadewoodSword, Do(AutoReuse, UseTurn) },
                { ItemID.ShadowbeamStaff, Do(DamageRatio(1.75f)) },
                { ItemID.ShadowFlameBow, Do(PointBlank, DamageExact(55)) },
                { ItemID.ShadowFlameHexDoll, Do(DamageExact(69), ShootSpeedExact(30)) },
                { ItemID.ShadowFlameKnife, Do(DamageExact(70)) },
                { ItemID.ShadowJoustingLance, trueMelee },
                { ItemID.Shotgun, Do(PointBlank, DamageExact(36), AutoReuse) },
                { ItemID.ShroomiteDiggingClaw, Do(PickPower(200), AxePower(125), UseTimeExact(4), TileBoostExact(-1)) },
                { ItemID.SilverAxe, Do(AxePower(70), UseTimeExact(14), TileBoostExact(+0)) },
                { ItemID.SilverBow, Do(PointBlank, DamageRatio(1.1f)) },
                { ItemID.SilverBroadsword, Do(AutoReuse, UseTurn, DamageRatio(1.5f), ScaleRatio(1.3f)) },
                { ItemID.SilverBullet, Do(DamageExact(8)) },
                { ItemID.SilverHammer, Do(HammerPower(55), UseTimeExact(10), TileBoostExact(+0)) },
                { ItemID.SilverPickaxe, Do(PickPower(50), UseTimeExact(11)) },
                { ItemID.SilverShortsword, Do(AutoReuse, TrueMelee, DamageRatio(1.5f)) },
                { ItemID.SkyFracture, Do(DamageExact(54), ShootSpeedExact(30f)) },
                { ItemID.SlapHand, Do(UseTurn, ScaleRatio(1.5f), DamageExact(120)) },
                { ItemID.SlimeCrown, nonConsumableBossSummon },
                { ItemID.SlimeStaff, Do(AutoReuse, UseExact(30)) },
                { ItemID.Smolstar, Do(DamageExact(9), AutoReuse, UseExact(25)) },
                { ItemID.SniperRifle, pointBlank },
                { ItemID.SnowballCannon, pointBlank },
                { ItemID.SnowGlobe, nonConsumableBossSummon },
                { ItemID.SolarEruption, Do(DamageRatio(1.5f)) },
                { ItemID.SolarFlareChainsaw, trueMeleeNoSpeed },
                { ItemID.SolarFlareDrill, Do(TrueMeleeNoSpeed, PickPower(225), UseTimeExact(3), TileBoostExact(+4)) },
                { ItemID.SolarFlarePickaxe, Do(PickPower(225), UseTimeExact(6), TileBoostExact(+4)) },
                { ItemID.SolarTablet, nonConsumableBossSummon },
                // Life Drain could probably get a bigger buff
                { ItemID.SoulDrain, Do(DamageRatio(1.1f)) }, // Life Drain
                { ItemID.SpaceGun, Do(DamageExact(25)) },
                { ItemID.Spear, Do(AutoReuse, TrueMelee, DamageRatio(2f)) },
                { ItemID.SpectreHamaxe, Do(HammerPower(90), AxePower(170), UseTimeExact(8), TileBoostExact(+4)) },
                { ItemID.SpectrePickaxe, Do(PickPower(200), UseTimeExact(8), TileBoostExact(+4)) },
                { ItemID.SpectreStaff, Do(DamageRatio(3f)) },
                { ItemID.SpiderStaff, Do(AutoReuse, UseExact(25)) },
                { ItemID.SpiritFlame, Do(UseExact(20), ManaExact(11), ShootSpeedExact(2f)) },
                { ItemID.StaffofEarth, Do(DamageRatio(1.2f)) },
                { ItemID.StaffoftheFrostHydra, Do(UseExact(20)) },
                { ItemID.StakeLauncher, Do(PointBlank, DamageRatio(2f), UseRatio(1.5f)) },
                { ItemID.StarCannon, Do(DamageExact(25)) },
                { ItemID.StardustCellStaff, Do(AutoReuse, UseExact(20)) },
                { ItemID.StardustChainsaw, trueMeleeNoSpeed },
                { ItemID.StardustDragonStaff, Do(AutoReuse, DamageExact(20), UseExact(19)) },
                { ItemID.StardustDrill, Do(TrueMeleeNoSpeed, PickPower(225), UseTimeExact(3), TileBoostExact(+4)) },
                { ItemID.StardustPickaxe, Do(PickPower(225), UseTimeExact(6), TileBoostExact(+4)) },
                { ItemID.Starfury, autoReuse },
                { ItemID.PortableStool, Do(Value(Item.sellPrice(copper: 20))) }, // Step Stool
                { ItemID.StarWrath, Do(DamageRatio(0.9f), AttackSpeedExact(0.33f)) },
                { ItemID.StickyBomb, maxStack999 },
                { ItemID.StickyDynamite, maxStack999 },
                { ItemID.StormTigerStaff, Do(AutoReuse, DamageExact(49), UseExact(20)) }, // Desert Tiger Staff
                { ItemID.StylistKilLaKillScissorsIWish, Do(AutoReuse, UseTurn, ScaleRatio(1.5f), DamageExact(33)) },
                { ItemID.SuspiciousLookingEye, nonConsumableBossSummon },
                { ItemID.Swordfish, Do(AutoReuse, TrueMelee, DamageExact(38)) },
                { ItemID.SwordWhip, autoReuse },
                { ItemID.TacticalShotgun, Do(PointBlank, DamageRatio(1.2f)) },
                { ItemID.TaxCollectorsStickOfDoom, Do(AutoReuse, UseTurn, ScaleRatio(1.5f), UseRatio(0.8f), DamageExact(70)) },
                { ItemID.TempestStaff, Do(AutoReuse, UseExact(20)) },
                { ItemID.TendonBow, Do(PointBlank, DamageExact(17), AutoReuse) },
                { ItemID.TentacleSpike, autoReuse },
                { ItemID.TerraBlade, Do(UseMeleeSpeed) },
                { ItemID.Terragrim, trueMeleeNoSpeed },
				// Vanilla damage 190. After fixing iframes so yoyo and shots can hit simultaneously,
				// Terrarian is extremely overpowered and requires a heavy nerf.
                { ItemID.Terrarian, Do(AutoReuse, DamageExact(106)) },
                { ItemID.TheAxe, Do(HammerPower(100), AxePower(175), UseTimeExact(7), TileBoostExact(+1)) },
                { ItemID.TheBreaker, Do(HammerPower(70), UseTimeExact(13), TileBoostExact(+0)) },
                { ItemID.TheEyeOfCthulhu, autoReuse },
                { ItemID.TheHorsemansBlade, Do(UseTurn, ScaleRatio(1.5f), UseRatio(0.8f), DamageExact(95)) },
                { ItemID.TheRottedFork, Do(AutoReuse, TrueMelee, DamageExact(20)) },
                { ItemID.TheUndertaker, Do(AutoReuse, UseExact(21)) },
                { ItemID.ThornChakram, Do(DamageExact(32)) },
                { ItemID.ThornWhip, autoReuse },
                { ItemID.ThunderSpear, Do(AutoReuse, UseMeleeSpeed) }, // Storm Spear
                { ItemID.TinAxe, Do(AxePower(50), UseTimeExact(16), TileBoostExact(+0)) },
                { ItemID.TinBow, Do(PointBlank, DamageRatio(1.1f)) },
                { ItemID.TinBroadsword, Do(AutoReuse, UseTurn) },
                { ItemID.TinHammer, Do(HammerPower(35), UseTimeExact(12), TileBoostExact(+0)) },
                { ItemID.TinPickaxe, Do(PickPower(35), UseTimeExact(10), TileBoostExact(+0)) },
                { ItemID.TinShortsword, Do(AutoReuse, TrueMelee) },
                { ItemID.TitaniumChainsaw, Do(TrueMeleeNoSpeed, AxePower(90), UseTimeExact(4), TileBoostExact(+0)) },
                { ItemID.TitaniumDrill, Do(TrueMeleeNoSpeed, PickPower(180), UseTimeExact(4), TileBoostExact(+1)) },
                { ItemID.TitaniumPickaxe, Do(PickPower(180), UseTimeExact(8), TileBoostExact(+1)) },
                { ItemID.TitaniumRepeater, Do(PointBlank, UseExact(29), DamageExact(122)) },
                { ItemID.TitaniumSword, Do(UseTurn, ScaleRatio(1.5f), UseRatio(0.8f), DamageExact(77)) },
                { ItemID.TitaniumTrident, Do(AutoReuse, TrueMelee, UseRatio(0.8f), DamageExact(72), ShootSpeedRatio(1.25f)) },
                { ItemID.TitaniumWaraxe, Do(AxePower(160), UseTimeExact(10), TileBoostExact(+1)) },
                { ItemID.TopazStaff, Do(ManaExact(2)) },
                { ItemID.Toxikarp, Do(UseTimeExact(7), UseAnimationExact(14)) },
                { ItemID.TragicUmbrella, autoReuse },
                { ItemID.Trident, Do(AutoReuse, TrueMelee, DamageExact(22)) },
                { ItemID.TrueExcalibur, Do(AutoReuse, UseTurn, UseMeleeSpeed, DamageExact(82)) },
                { ItemID.TrueNightsEdge, Do(AutoReuse, UseTurn, UseMeleeSpeed, DamageExact(160), ShootSpeedExact(16), ScaleRatio(1.5f)) },
                { ItemID.Tsunami, Do(PointBlank, DamageRatio(1.25f)) },
                { ItemID.TungstenAxe, Do(AxePower(70), UseTimeExact(14), TileBoostExact(+0)) },
                { ItemID.TungstenBow, Do(PointBlank, DamageRatio(1.1f)) },
                { ItemID.TungstenBroadsword, Do(AutoReuse, UseTurn, DamageRatio(1.5f), ScaleRatio(1.3f)) },
                { ItemID.TungstenHammer, Do(HammerPower(55), UseTimeExact(10), TileBoostExact(+0)) },
                { ItemID.TungstenPickaxe, Do(PickPower(50), UseTimeExact(11)) },
                { ItemID.TungstenShortsword, Do(AutoReuse, TrueMelee, DamageRatio(1.5f)) },
                { ItemID.Umbrella, autoReuse },
                { ItemID.UnholyArrow, Do(DamageExact(11)) },
                { ItemID.UnholyTrident, Do(ManaExact(14), DamageRatio(1.25f)) },
                { ItemID.Uzi, pointBlank },
                { ItemID.ValkyrieYoyo, autoReuse },
                { ItemID.Valor, Do(DamageExact(32)) },
                { ItemID.VampireFrogStaff, Do(AutoReuse, UseExact(30)) },
                { ItemID.VampireKnives, Do(DamageRatio(1.33f)) },
                { ItemID.VenomArrow, Do(DamageRatio(1.1f)) },
                { ItemID.VenomStaff, Do(DamageRatio(1.5f)) },
                { ItemID.VenusMagnum, Do(AutoReuse, PointBlank) },
                { ItemID.Vilethorn, Do(DamageExact(14)) },
                { ItemID.VortexBeater, pointBlank },
                { ItemID.VortexChainsaw, trueMeleeNoSpeed },
                { ItemID.VortexDrill, Do(TrueMeleeNoSpeed, PickPower(225), UseTimeExact(3), TileBoostExact(+4)) },
                { ItemID.VortexPickaxe, Do(PickPower(225), UseTimeExact(6), TileBoostExact(+4)) },
                { ItemID.WandofSparking, Do(AutoReuse) },
                { ItemID.WarAxeoftheNight, Do(AxePower(100), UseTimeExact(13), TileBoostExact(+0)) },
                { ItemID.WaspGun, Do(UseExact(11)) },
                { ItemID.WaterBolt, Do(DamageExact(23)) },
                { ItemID.WeatherPain, autoReuse },
                { ItemID.WhitePhaseblade, Do(AutoReuse, UseTurn, ScaleRatio(1.5f), DamageExact(51)) },
                { ItemID.WhitePhasesaber, Do(ScaleRatio(1.5f), DamageExact(72), UseExact(20)) },
                { ItemID.WoodenArrow, Do(DamageRatio(1.1f)) },
                { ItemID.WoodenBoomerang, Do(DamageRatio(2f), Value(Item.sellPrice(copper: 20))) },
                { ItemID.WoodenBow, Do(PointBlank, DamageRatio(1.1f)) },
                { ItemID.WoodenHammer, Do(HammerPower(25), UseTimeExact(11), TileBoostExact(+0)) },
                { ItemID.WoodenSword, Do(AutoReuse, UseTurn) },
                { ItemID.WoodYoyo, autoReuse },
                { ItemID.WormFood, nonConsumableBossSummon },
                { ItemID.Xenopopper, Do(DamageRatio(0.75f)) },
                { ItemID.XenoStaff, Do(AutoReuse, UseExact(20)) },
                { ItemID.Yelets, autoReuse },
                { ItemID.YellowPhaseblade, Do(AutoReuse, UseTurn, ScaleRatio(1.5f), DamageExact(51)) },
                { ItemID.YellowPhasesaber, Do(ScaleRatio(1.5f), DamageExact(72), UseExact(20)) },
                { ItemID.ZapinatorGray, autoReuse },
                { ItemID.ZapinatorOrange, autoReuse },
                { ItemID.ZombieArm, Do(AutoReuse, UseTurn, ScaleRatio(1.5f), DamageExact(12), KnockbackExact(12f)) },
            };
        }

        internal static void UnloadTweaks()
        {
            currentTweaks?.Clear();
            currentTweaks = null;
        }
        #endregion

        #region SetDefaults (Item Tweaks Applied Here)
        internal static void SetDefaults_ApplyTweaks(Item item)
        {
            // Do nothing if the tweaks database is not defined.
            if (currentTweaks is null)
                return;

            // Grab the tweaking or balancing to apply, if any. If nothing comes back, do nothing.
            bool needsTweaking = currentTweaks.TryGetValue(item.type, out IItemTweak[] tweaks);
            if (!needsTweaking)
                return;

            // Apply all alterations sequentially, assuming they are relevant.
            foreach (IItemTweak tweak in tweaks)
                if (tweak.AppliesTo(item))
                    tweak.ApplyTweak(item);
        }
        #endregion

        #region Internal Structures

        // This function simply concatenates a bunch of Item Tweaks into an array.
        // It looks a lot nicer than constantly typing "new IItemTweak[]".
        internal static IItemTweak[] Do(params IItemTweak[] r) => r;

        #region Applicability Lambdas
        internal static bool DealsDamage(Item it) => it.damage > 0;
        internal static bool HasDefense(Item it) => it.defense > 0;
        internal static bool HasKnockback(Item it) => !it.accessory & !it.vanity; // how to check if something is wearable armor?
        internal static bool IsAxe(Item it) => it.axe > 0;
        internal static bool IsHammer(Item it) => it.hammer > 0;
        internal static bool IsMelee(Item it) => it.CountsAsClass<MeleeDamageClass>() || it.CountsAsClass<MeleeNoSpeedDamageClass>(); // true melee is included by extension
        internal static bool IsPickaxe(Item it) => it.pick > 0;
        internal static bool IsScalable(Item it) => it.damage > 0 && IsMelee(it); // sanity check: only melee weapons get scaled
        internal static bool IsUsable(Item it) => it.useStyle != ItemUseStyleID.None && it.useTime > 0 && it.useAnimation > 0;
        internal static bool UsesMana(Item it) => IsUsable(it); // Only usable items cost mana, but items must be able to have their mana cost disabled or enabled at will.
        internal static bool UtilizesVelocity(Item it) => IsUsable(it) || it.ammo > AmmoID.None; // The item must either be usable or be an ammunition for its velocity stat to do anything.
        #endregion

        #region Item Tweak Definitions
        internal interface IItemTweak
        {
            bool AppliesTo(Item it);
            void ApplyTweak(Item it);
        }

        #region Attack Speed Ratio
        private static float CapAttackSpeed(float f) => MathHelper.Clamp(f, BalancingConstants.MinimumAllowedAttackSpeed, BalancingConstants.MaximumAllowedAttackSpeed);

        internal class AttackSpeedExactRule : IItemTweak
        {
            internal readonly float ratio = 1f;

            public AttackSpeedExactRule(float f) => ratio = f;
            public bool AppliesTo(Item it) => IsUsable(it);
            public void ApplyTweak(Item it) => ItemID.Sets.BonusAttackSpeedMultiplier[it.type] = CapAttackSpeed(ratio);
        }
        internal static IItemTweak AttackSpeedExact(float f) => new AttackSpeedExactRule(f);

        internal class AttackSpeedRatioRule : IItemTweak
        {
            internal readonly float ratio = 1f;

            public AttackSpeedRatioRule(float f) => ratio = f;
            public bool AppliesTo(Item it) => IsUsable(it);
            public void ApplyTweak(Item it)
            {
                float currentAttackSpeedRatio = ItemID.Sets.BonusAttackSpeedMultiplier[it.type];
                ItemID.Sets.BonusAttackSpeedMultiplier[it.type] = CapAttackSpeed(ratio * currentAttackSpeedRatio);
            }
        }
        internal static IItemTweak AttackSpeedRatio(float f) => new AttackSpeedRatioRule(f);
        #endregion

        #region Auto Reuse
        internal class AutoReuseRule : IItemTweak
        {
            internal readonly bool flag = true;

            public AutoReuseRule(bool ar) => flag = ar;
            public bool AppliesTo(Item it) => IsUsable(it);
            public void ApplyTweak(Item it) => it.autoReuse = flag;
        }
        internal static IItemTweak AutoReuse => new AutoReuseRule(true);
        internal static IItemTweak NoAutoReuse => new AutoReuseRule(false);
        #endregion

        #region Axe Power
        // Uses the values shown by Terraria, which are multiplied by 5, not the internal values
        internal class AxePowerRule : IItemTweak
        {
            internal readonly int newAxePower = 0;

            public AxePowerRule(int newDisplayedAxePower) => newAxePower = newDisplayedAxePower / 5;
            public bool AppliesTo(Item it) => IsAxe(it);
            public void ApplyTweak(Item it)
            {
                it.axe = newAxePower;
                if (it.axe < 0)
                    it.axe = 0;
            }
        }
        internal static IItemTweak AxePower(int a) => new AxePowerRule(a);
        #endregion

        #region Consumable
        internal class ConsumableRule : IItemTweak
        {
            internal readonly bool flag = false;

            public ConsumableRule(bool c) => flag = c;
            public bool AppliesTo(Item it) => true;
            public void ApplyTweak(Item it) => it.consumable = flag;
        }
        internal static IItemTweak Consumable => new ConsumableRule(true);
        internal static IItemTweak NotConsumable => new ConsumableRule(false);
        #endregion

        #region Crit Chance
        internal class CritChanceDeltaRule : IItemTweak
        {
            internal readonly int delta = 0;

            public CritChanceDeltaRule(int d) => delta = d;
            public bool AppliesTo(Item it) => DealsDamage(it);
            public void ApplyTweak(Item it)
            {
                it.crit += delta;
                if (it.crit < 0)
                    it.crit = 0;
            }
        }
        internal static IItemTweak CritDelta(int d) => new CritChanceDeltaRule(d);

        internal class CritChanceExactRule : IItemTweak
        {
            internal readonly int newCrit = 0;

            public CritChanceExactRule(int crit) => newCrit = crit;
            public bool AppliesTo(Item it) => DealsDamage(it);
            public void ApplyTweak(Item it)
            {
                it.crit = newCrit;
                if (it.crit < 0)
                    it.crit = 0;
            }
        }
        internal static IItemTweak CritExact(int crit) => new CritChanceExactRule(crit);
        #endregion

        #region Damage
        internal class DamageDeltaRule : IItemTweak
        {
            internal readonly int delta = 0;

            public DamageDeltaRule(int d) => delta = d;
            public bool AppliesTo(Item it) => DealsDamage(it);
            public void ApplyTweak(Item it)
            {
                it.damage += delta;
                if (it.damage < 0)
                    it.damage = 0;
            }
        }
        internal static IItemTweak DamageDelta(int d) => new DamageDeltaRule(d);

        internal class DamageExactRule : IItemTweak
        {
            internal readonly int newDamage = 0;

            public DamageExactRule(int dmg) => newDamage = dmg;
            public bool AppliesTo(Item it) => DealsDamage(it);
            public void ApplyTweak(Item it)
            {
                it.damage = newDamage;
                if (it.damage < 0)
                    it.damage = 0;
            }
        }
        internal static IItemTweak DamageExact(int d) => new DamageExactRule(d);

        internal class DamageRatioRule : IItemTweak
        {
            internal readonly float ratio = 1f;

            public DamageRatioRule(float f) => ratio = f;
            public bool AppliesTo(Item it) => DealsDamage(it);
            public void ApplyTweak(Item it)
            {
                it.damage = (int)(it.damage * ratio);
                if (it.damage < 0)
                    it.damage = 0;
            }
        }
        internal static IItemTweak DamageRatio(float f) => new DamageRatioRule(f);
        #endregion

        #region Defense
        internal class DefenseDeltaRule : IItemTweak
        {
            internal readonly int delta = 0;

            public DefenseDeltaRule(int d) => delta = d;
            public bool AppliesTo(Item it) => HasDefense(it);
            public void ApplyTweak(Item it)
            {
                it.defense += delta;
                if (it.defense < 0)
                    it.defense = 0;
            }
        }
        internal static IItemTweak DefenseDelta(int d) => new DefenseDeltaRule(d);

        internal class DefenseExactRule : IItemTweak
        {
            internal readonly int newDefense = 0;

            public DefenseExactRule(int def) => newDefense = def;
            public bool AppliesTo(Item it) => HasDefense(it) || it.accessory;
            public void ApplyTweak(Item it)
            {
                it.defense = newDefense;
                if (it.defense < 0)
                    it.defense = 0;
            }
        }
        internal static IItemTweak DefenseExact(int d) => new DefenseExactRule(d);
        #endregion

        #region Hammer Power
        internal class HammerPowerRule : IItemTweak
        {
            internal readonly int newHammerPower = 0;

            public HammerPowerRule(int h) => newHammerPower = h;
            public bool AppliesTo(Item it) => IsHammer(it);
            public void ApplyTweak(Item it)
            {
                it.hammer = newHammerPower;
                if (it.hammer < 0)
                    it.hammer = 0;
            }
        }
        internal static IItemTweak HammerPower(int h) => new HammerPowerRule(h);
        #endregion

        #region Knockback
        internal class KnockbackDeltaRule : IItemTweak
        {
            internal readonly float delta = 0;

            public KnockbackDeltaRule(float d) => delta = d;
            public bool AppliesTo(Item it) => HasKnockback(it);
            public void ApplyTweak(Item it)
            {
                it.knockBack += delta;
                if (it.knockBack < 0f)
                    it.knockBack = 0f;
            }
        }
        internal static IItemTweak KnockbackDelta(float d) => new KnockbackDeltaRule(d);

        internal class KnockbackExactRule : IItemTweak
        {
            internal readonly float newKnockback = 0;

            public KnockbackExactRule(float kb) => newKnockback = kb;
            public bool AppliesTo(Item it) => HasKnockback(it);
            public void ApplyTweak(Item it)
            {
                it.knockBack = newKnockback;
                if (it.knockBack < 0f)
                    it.knockBack = 0f;
            }
        }
        internal static IItemTweak KnockbackExact(float kb) => new KnockbackExactRule(kb);

        internal class KnockbackRatioRule : IItemTweak
        {
            internal readonly float ratio = 1f;

            public KnockbackRatioRule(float f) => ratio = f;
            public bool AppliesTo(Item it) => HasKnockback(it);
            public void ApplyTweak(Item it)
            {
                it.knockBack *= ratio;
                if (it.knockBack < 0f)
                    it.knockBack = 0f;
            }
        }
        internal static IItemTweak KnockbackRatio(float r) => new KnockbackRatioRule(r);
        #endregion

        #region Mana Cost
        internal class ManaDeltaRule : IItemTweak
        {
            internal readonly int delta = 0;

            public ManaDeltaRule(int d) => delta = d;
            public bool AppliesTo(Item it) => UsesMana(it);
            public void ApplyTweak(Item it)
            {
                it.mana += delta;
                if (it.mana < 0)
                    it.mana = 0;
            }
        }
        internal static IItemTweak ManaDelta(int d) => new ManaDeltaRule(d);

        internal class ManaExactRule : IItemTweak
        {
            internal readonly int newMana = 0;

            public ManaExactRule(int m) => newMana = m;
            public bool AppliesTo(Item it) => UsesMana(it);
            public void ApplyTweak(Item it)
            {
                it.mana = newMana;
                if (it.mana < 0)
                    it.mana = 0;
            }
        }
        internal static IItemTweak ManaExact(int m) => new ManaExactRule(m);

        internal class ManaRatioRule : IItemTweak
        {
            internal readonly float ratio = 1f;

            public ManaRatioRule(float f) => ratio = f;
            public bool AppliesTo(Item it) => UsesMana(it);
            public void ApplyTweak(Item it)
            {
                it.mana = (int)(it.mana * ratio);
                if (it.mana < 0)
                    it.mana = 0;
            }
        }
        internal static IItemTweak ManaRatio(float f) => new ManaRatioRule(f);
        #endregion

        #region Max Stack
        internal class MaxStackRule : IItemTweak // max stack plus - calamity style
        {
            internal readonly int newMaxStack = 999;

            public MaxStackRule(int stk) => newMaxStack = stk;
            public bool AppliesTo(Item it) => true;
            public void ApplyTweak(Item it)
            {
                it.maxStack = newMaxStack;
                if (it.maxStack < 1)
                    it.maxStack = 1;
            }
        }
        internal static IItemTweak MaxStack(int stk) => new MaxStackRule(stk);
        #endregion

        #region Melee Settings (True Melee & Melee Speed)
        internal class MeleeSettingsRule : IItemTweak
        {
            // If true: Uses melee speed, which WILL apply to projectile fire rate.
            // If false: Does not use melee speed in any way.
            internal readonly bool speed = true;

            // If true: Counts as true melee, and benefits from True Melee specific bonuses.
            // If false: Does not count as true melee.
            internal readonly bool trueMelee = false;

            public MeleeSettingsRule(bool s, bool t = false)
            {
                speed = s;
                trueMelee = t;
            }
            public bool AppliesTo(Item it) => IsMelee(it);
            public void ApplyTweak(Item it)
            {
                // If set to use melee speed, the item's projectile fire rate now scales with melee speed.
                if (speed)
                    it.attackSpeedOnlyAffectsWeaponAnimation = false;

                // Set damage type appropriately.
                if (speed)
                    it.DamageType = trueMelee ? TrueMeleeDamageClass.Instance : DamageClass.Melee;
                else
                    it.DamageType = trueMelee ? TrueMeleeNoSpeedDamageClass.Instance : DamageClass.MeleeNoSpeed;
            }
        }
        internal static IItemTweak UseMeleeSpeed => new MeleeSettingsRule(true, false);
        internal static IItemTweak DontUseMeleeSpeed => new MeleeSettingsRule(false, false);
        internal static IItemTweak TrueMelee => new MeleeSettingsRule(true, true);
        internal static IItemTweak TrueMeleeNoSpeed => new MeleeSettingsRule(false, true);
        #endregion

        #region Pick Power
        internal class PickPowerRule : IItemTweak
        {
            internal readonly int newPickPower = 0;

            public PickPowerRule(int p) => newPickPower = p;
            public bool AppliesTo(Item it) => IsPickaxe(it);
            public void ApplyTweak(Item it)
            {
                it.pick = newPickPower;
                if (it.pick < 0)
                    it.pick = 0;
            }
        }
        internal static IItemTweak PickPower(int p) => new PickPowerRule(p);
        #endregion

        #region Point Blank
        internal class PointBlankRule : IItemTweak
        {
            public bool AppliesTo(Item it) => true;
            public void ApplyTweak(Item it) => it.Calamity().canFirePointBlankShots = true;
        }
        internal static IItemTweak PointBlank => new PointBlankRule();
        #endregion

        #region Scale (True Melee)
        internal class ScaleDeltaRule : IItemTweak
        {
            internal readonly float delta = 0;

            public ScaleDeltaRule(float d) => delta = d;
            public bool AppliesTo(Item it) => IsScalable(it);
            public void ApplyTweak(Item it)
            {
                it.scale += delta;
                if (it.scale < 0f)
                    it.scale = 0f;
            }
        }
        internal static IItemTweak ScaleDelta(float d) => new ScaleDeltaRule(d);

        internal class ScaleExactRule : IItemTweak
        {
            internal readonly float newScale = 0;

            public ScaleExactRule(float s) => newScale = s;
            public bool AppliesTo(Item it) => IsScalable(it);
            public void ApplyTweak(Item it)
            {
                it.scale = newScale;
                if (it.scale < 0f)
                    it.scale = 0f;
            }
        }
        internal static IItemTweak ScaleExact(float s) => new ScaleExactRule(s);

        internal class ScaleRatioRule : IItemTweak
        {
            internal readonly float ratio = 1f;

            public ScaleRatioRule(float f) => ratio = f;
            public bool AppliesTo(Item it) => IsScalable(it);
            public void ApplyTweak(Item it)
            {
                it.scale *= ratio;
                if (it.scale < 0f)
                    it.scale = 0f;
            }
        }
        internal static IItemTweak ScaleRatio(float f) => new ScaleRatioRule(f);
        #endregion

        #region Shoot Speed (Velocity)
        internal class ShootSpeedDeltaRule : IItemTweak
        {
            internal readonly float delta = 0;

            public ShootSpeedDeltaRule(float d) => delta = d;
            public bool AppliesTo(Item it) => UtilizesVelocity(it);
            public void ApplyTweak(Item it)
            {
                it.shootSpeed += delta;
                if (it.shootSpeed < 0f)
                    it.shootSpeed = 0f;
            }
        }
        internal static IItemTweak ShootSpeedDelta(float d) => new ShootSpeedDeltaRule(d);

        internal class ShootSpeedExactRule : IItemTweak
        {
            internal readonly float newShootSpeed = 0;

            public ShootSpeedExactRule(float ss) => newShootSpeed = ss;
            public bool AppliesTo(Item it) => UtilizesVelocity(it);
            public void ApplyTweak(Item it)
            {
                it.shootSpeed = newShootSpeed;
                if (it.shootSpeed < 0f)
                    it.shootSpeed = 0f;
            }
        }
        internal static IItemTweak ShootSpeedExact(float s) => new ShootSpeedExactRule(s);

        internal class ShootSpeedRatioRule : IItemTweak
        {
            internal readonly float ratio = 1f;

            public ShootSpeedRatioRule(float f) => ratio = f;
            public bool AppliesTo(Item it) => UtilizesVelocity(it);
            public void ApplyTweak(Item it)
            {
                it.shootSpeed *= ratio;
                if (it.shootSpeed < 0f)
                    it.shootSpeed = 0f;
            }
        }
        internal static IItemTweak ShootSpeedRatio(float f) => new ShootSpeedRatioRule(f);
        #endregion

        #region Tile Boost (Extra Tool Range)
        internal class TileBoostDeltaRule : IItemTweak
        {
            private readonly int delta = 0;

            public TileBoostDeltaRule(int d) => delta = d;
            public bool AppliesTo(Item it) => true;
            public void ApplyTweak(Item it) => it.tileBoost += delta;
        }
        internal static IItemTweak TileBoostDelta(int d) => new TileBoostDeltaRule(d);

        internal class TileBoostExactRule : IItemTweak
        {
            private readonly int newTileBoost = 0;

            public TileBoostExactRule(int tb) => newTileBoost = tb;
            public bool AppliesTo(Item it) => true;
            public void ApplyTweak(Item it) => it.tileBoost = newTileBoost;
        }
        internal static IItemTweak TileBoostExact(int tb) => new TileBoostExactRule(tb);
        #endregion

        #region Use Time and Use Animation
        internal class UseDeltaRule : IItemTweak
        {
            internal readonly int delta = 0;

            public UseDeltaRule(int d) => delta = d;
            public bool AppliesTo(Item it) => IsUsable(it);
            public void ApplyTweak(Item it)
            {
                it.useAnimation += delta;
                it.useTime += delta;
                if (it.useAnimation < 1)
                    it.useAnimation = 1;
                if (it.useTime < 1)
                    it.useTime = 1;
            }
        }
        internal static IItemTweak UseDelta(int d) => new UseDeltaRule(d);

        internal class UseExactRule : IItemTweak
        {
            internal readonly int newUseTime = 0;

            public UseExactRule(int ut) => newUseTime = ut;
            public bool AppliesTo(Item it) => IsUsable(it);
            public void ApplyTweak(Item it)
            {
                it.useAnimation = newUseTime;
                it.useTime = newUseTime;
                if (it.useAnimation < 1)
                    it.useAnimation = 1;
                if (it.useTime < 1)
                    it.useTime = 1;
            }
        }
        internal static IItemTweak UseExact(int ut) => new UseExactRule(ut);

        internal class UseRatioRule : IItemTweak
        {
            internal readonly float ratio = 1f;

            public UseRatioRule(float f) => ratio = f;
            public bool AppliesTo(Item it) => IsUsable(it);
            public void ApplyTweak(Item it)
            {
                it.useAnimation = (int)(it.useAnimation * ratio);
                it.useTime = (int)(it.useTime * ratio);
                if (it.useAnimation < 1)
                    it.useAnimation = 1;
                if (it.useTime < 1)
                    it.useTime = 1;
            }
        }
        internal static IItemTweak UseRatio(float f) => new UseRatioRule(f);

        internal class UseAnimationDeltaRule : IItemTweak
        {
            internal readonly int delta = 0;

            public UseAnimationDeltaRule(int d) => delta = d;
            public bool AppliesTo(Item it) => IsUsable(it);
            public void ApplyTweak(Item it)
            {
                it.useAnimation += delta;
                if (it.useAnimation < 1)
                    it.useAnimation = 1;
            }
        }
        internal static IItemTweak UseAnimationDelta(int d) => new UseAnimationDeltaRule(d);

        internal class UseAnimationExactRule : IItemTweak
        {
            internal readonly int newUseAnimation = 0;

            public UseAnimationExactRule(int ua) => newUseAnimation = ua;
            public bool AppliesTo(Item it) => IsUsable(it);
            public void ApplyTweak(Item it)
            {
                it.useAnimation = newUseAnimation;
                if (it.useAnimation < 1)
                    it.useAnimation = 1;
            }
        }
        internal static IItemTweak UseAnimationExact(int ua) => new UseAnimationExactRule(ua);

        internal class UseAnimationRatioRule : IItemTweak
        {
            internal readonly float ratio = 1f;

            public UseAnimationRatioRule(float f) => ratio = f;
            public bool AppliesTo(Item it) => IsUsable(it);
            public void ApplyTweak(Item it)
            {
                it.useAnimation = (int)(it.useAnimation * ratio);
                if (it.useAnimation < 1)
                    it.useAnimation = 1;
            }
        }
        internal static IItemTweak UseAnimationRatio(float f) => new UseAnimationRatioRule(f);

        internal class UseTimeDeltaRule : IItemTweak
        {
            internal readonly int delta = 0;

            public UseTimeDeltaRule(int d) => delta = d;
            public bool AppliesTo(Item it) => IsUsable(it);
            public void ApplyTweak(Item it)
            {
                it.useTime += delta;
                if (it.useTime < 1)
                    it.useTime = 1;
            }
        }
        internal static IItemTweak UseTimeDelta(int d) => new UseTimeDeltaRule(d);

        internal class UseTimeExactRule : IItemTweak
        {
            internal readonly int newUseTime = 0;

            public UseTimeExactRule(int ut) => newUseTime = ut;
            public bool AppliesTo(Item it) => IsUsable(it);
            public void ApplyTweak(Item it)
            {
                it.useTime = newUseTime;
                if (it.useTime < 1)
                    it.useTime = 1;
            }
        }
        internal static IItemTweak UseTimeExact(int ut) => new UseTimeExactRule(ut);

        internal class UseTimeRatioRule : IItemTweak
        {
            internal readonly float ratio = 1f;

            public UseTimeRatioRule(float f) => ratio = f;
            public bool AppliesTo(Item it) => IsUsable(it);
            public void ApplyTweak(Item it)
            {
                it.useTime = (int)(it.useTime * ratio);
                if (it.useTime < 1)
                    it.useTime = 1;
            }
        }
        internal static IItemTweak UseTimeRatio(float f) => new UseTimeRatioRule(f);
        #endregion

        #region Use Turn
        internal class UseTurnRule : IItemTweak
        {
            internal readonly bool flag = true;

            public UseTurnRule(bool ut) => flag = ut;
            public bool AppliesTo(Item it) => IsUsable(it);
            public void ApplyTweak(Item it) => it.useTurn = flag;
        }
        internal static IItemTweak UseTurn => new UseTurnRule(true);
        internal static IItemTweak NoUseTurn => new UseTurnRule(false);
        #endregion

        #region Value (Sell Price)
        internal class ValueRule : IItemTweak
        {
            internal readonly int newValue = 0;

            public ValueRule(int v) => newValue = v;
            public bool AppliesTo(Item it) => true;
            public void ApplyTweak(Item it)
            {
                it.value = newValue;
                if (it.value < 0)
                    it.value = 0;
            }
        }
        internal static IItemTweak Value(int v) => new ValueRule(v);
        internal static IItemTweak Worthless => new ValueRule(0);
        #endregion
        #endregion
        #endregion
    }
}
