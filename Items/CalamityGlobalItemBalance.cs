﻿using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
	public partial class CalamityGlobalItem : GlobalItem
	{
		#region Database and Initialization
		internal static SortedDictionary<int, IBalanceRule[]> balance = null;

		internal static void LoadBalance()
		{
			// Various shorthands for items which receive very simple changes, such as setting one flag.
			IBalanceRule[] trueMelee = Do(TrueMelee);
			IBalanceRule[] pointBlank = Do(PointBlank);
			IBalanceRule[] autoReuse = Do(AutoReuse);
			IBalanceRule[] maxStack999 = Do(MaxStack(999));
			IBalanceRule[] nonConsumableBossSummon = Do(MaxStack(1), NotConsumable);

			// Please keep this strictly alphabetical. It's the only way to keep it sane. Thanks in advance.
			// - Ozzatron
			balance = new SortedDictionary<int, IBalanceRule[]>
			{
				{ ItemID.Abeemination, nonConsumableBossSummon },
				{ ItemID.AdamantiteChainsaw, Do(TrueMelee, AxePower(90), UseTimeExact(4), TileBoostExact(+0)) },
				{ ItemID.AdamantiteDrill, Do(TrueMelee, PickPower(180), UseTimeExact(4), TileBoostExact(+1)) },
				{ ItemID.AdamantiteGlaive, Do(AutoReuse, TrueMelee, UseRatio(0.8f), DamageExact(65), ShootSpeedRatio(1.25f)) },
				{ ItemID.AdamantitePickaxe, Do(PickPower(180), UseTimeExact(8), TileBoostExact(+1)) },
				{ ItemID.AdamantiteRepeater, Do(PointBlank, DamageRatio(1.1f)) },
				{ ItemID.AdamantiteSword, Do(UseTurn, ScaleRatio(1.5f), UseRatio(0.8f), DamageExact(77)) },
				{ ItemID.AdamantiteWaraxe, Do(AxePower(160), UseTimeExact(10), TileBoostExact(+1)) },
				{ ItemID.Amarok, autoReuse },
				{ ItemID.AmberStaff, Do(DamageRatio(1.33f), UseTimeExact(15), UseAnimationExact(40)) },
				{ ItemID.AmethystStaff, Do(ManaExact(2)) },
				{ ItemID.Anchor, Do(DamageExact(107)) },
				{ ItemID.AntlionClaw, Do(ScaleRatio(1.5f), DamageRatio(1.5f)) },
				{ ItemID.AquaScepter, Do(DamageExact(21), ShootSpeedExact(25f)) },
				{ ItemID.Arkhalis, trueMelee },
				// { ItemID.BabyBirdStaff, Do(AutoReuse, UseExact(35)) },
				{ ItemID.BallOHurt, Do(DamageRatio(2f)) },
				{ ItemID.BatScepter, Do(DamageRatio(1.1f)) },
				{ ItemID.BeamSword, autoReuse },
				{ ItemID.BeeGun, Do(DamageExact(11)) },
				{ ItemID.BeeKeeper, Do(UseTurn, ScaleRatio(1.5f), DamageRatio(2.5f)) },
				{ ItemID.BeesKnees, Do(PointBlank, DamageRatio(1.1f)) },
				{ ItemID.BladedGlove, Do(DamageExact(15), UseExact(7)) },
				{ ItemID.BladeofGrass, Do(AutoReuse, UseTurn, ScaleRatio(1.5f), DamageExact(65)) },
				{ ItemID.Bladetongue, Do(UseTurn, UseRatio(0.8f), DamageExact(120), ScaleRatio(1.75f)) },
				{ ItemID.BlizzardStaff, Do(DamageExact(41), ManaExact(7)) },
				{ ItemID.BloodButcherer, Do(AutoReuse, UseTurn, DamageRatio(1.66f)) },
				{ ItemID.BloodLustCluster, Do(AxePower(100), UseTimeExact(13), TileBoostExact(+0)) },
				{ ItemID.BloodyMachete, Do(AutoReuse, DamageExact(30)) },
				{ ItemID.BloodySpine, nonConsumableBossSummon },
				{ ItemID.Blowgun, pointBlank },
				{ ItemID.Blowpipe, pointBlank },
				{ ItemID.BlueMoon, Do(DamageRatio(2f)) },
				{ ItemID.BluePhaseblade, Do(AutoReuse, UseTurn, ScaleRatio(1.5f), DamageRatio(2f)) },
				{ ItemID.BluePhasesaber, Do(ScaleRatio(1.5f), DamageExact(72), UseExact(20)) },
				{ ItemID.BlueSolution, Do(Value(Item.buyPrice(silver: 5))) },
				{ ItemID.BoneArrow, Do(DamageRatio(1.1f)) },
				{ ItemID.BonePickaxe, Do(PickPower(55), UseTimeExact(6)) },
				{ ItemID.BoneSword, Do(AutoReuse, UseTurn, ScaleRatio(1.5f), DamageRatio(1.25f)) },
				{ ItemID.BookofSkulls, Do(ManaExact(12), ShootSpeedExact(9f)) },
				{ ItemID.BookStaff, Do(ManaExact(10), DamageRatio(1.25f)) },
				{ ItemID.Boomstick, pointBlank },
				{ ItemID.BorealWoodBow, Do(PointBlank, DamageRatio(1.1f)) },
				{ ItemID.BorealWoodHammer, Do(HammerPower(25), UseTimeExact(11), TileBoostExact(+0)) },
				{ ItemID.BorealWoodSword, Do(AutoReuse, UseTurn) },
				{ ItemID.BouncyBomb, maxStack999 },
				{ ItemID.BouncyDynamite, maxStack999 },
				{ ItemID.BreakerBlade, Do(AutoReuse, UseTurn, ScaleRatio(1.5f), DamageExact(97)) },
				{ ItemID.ButchersChainsaw, Do(TrueMelee, AxePower(150), UseTimeExact(3), TileBoostExact(+0)) },
				{ ItemID.CactusPickaxe, Do(PickPower(34), UseTimeExact(9)) },
				{ ItemID.CactusSword, Do(AutoReuse, UseTurn) },
				{ ItemID.CnadyCanePickaxe, Do(PickPower(55), UseTimeExact(9), TileBoostExact(+1)) }, // intentionally not in alphabetical order to correct for typo
				{ ItemID.CandyCaneSword, Do(AutoReuse, DamageRatio(1.25f)) },
				{ ItemID.CandyCornRifle, pointBlank },
				{ ItemID.Cascade, Do(AutoReuse, DamageExact(39)) },
				{ ItemID.CelestialSigil, nonConsumableBossSummon },
				{ ItemID.ChainGuillotines, Do(DamageRatio(1.2f)) },
				{ ItemID.ChainGun, pointBlank },
				{ ItemID.ChainKnife, Do(AutoReuse, DamageExact(14)) },
				{ ItemID.ChargedBlasterCannon, Do(DamageRatio(1.33f)) },
				{ ItemID.Chik, autoReuse },
				{ ItemID.ChlorophyteArrow, Do(DamageRatio(1.1f)) },
				{ ItemID.ChlorophyteChainsaw, Do(TrueMelee, AxePower(120), UseTimeExact(3), TileBoostExact(+0)) },
				{ ItemID.ChlorophyteDrill, Do(TrueMelee, PickPower(200), UseTimeExact(4), TileBoostExact(+2)) },
				{ ItemID.ChlorophyteGreataxe, Do(AxePower(165), UseTimeExact(7), TileBoostExact(+2)) },
				{ ItemID.ChlorophyteJackhammer, Do(TrueMelee, HammerPower(90), UseTimeExact(5), TileBoostExact(+0)) },
				{ ItemID.ChlorophytePartisan, Do(AutoReuse, UseRatio(0.8f), DamageExact(100)) },
				{ ItemID.ChlorophytePickaxe, Do(PickPower(200), UseTimeExact(7), TileBoostExact(+2)) },
				{ ItemID.ChlorophyteShotbow, Do(PointBlank, DamageRatio(1.1f)) },
				{ ItemID.ChlorophyteWarhammer, Do(HammerPower(90), UseTimeExact(8), TileBoostExact(+2)) },
				{ ItemID.ChristmasTreeSword, Do(AutoReuse, UseTurn, DamageExact(155)) },
				{ ItemID.ClingerStaff, Do(DamageRatio(1.33f)) },
				{ ItemID.ClockworkAssaultRifle, pointBlank },
				{ ItemID.CobaltChainsaw, Do(TrueMelee, AxePower(70), UseTimeExact(4), TileBoostExact(-1)) },
				{ ItemID.CobaltDrill, Do(TrueMelee, PickPower(130), UseTimeExact(5)) },
				{ ItemID.CobaltNaginata, Do(AutoReuse, TrueMelee, UseRatio(0.8f), DamageExact(90)) },
				{ ItemID.CobaltPickaxe, Do(PickPower(130), UseTimeExact(9)) },
				{ ItemID.CobaltRepeater, Do(PointBlank, DamageRatio(1.1f)) },
				{ ItemID.CobaltSword, Do(UseTurn, ScaleRatio(1.5f), UseRatio(0.8f), DamageExact(80)) },
				{ ItemID.CobaltWaraxe, Do(AxePower(125), UseTimeExact(12), TileBoostExact(+0)) },
				{ ItemID.Code1, Do(AutoReuse, DamageExact(25)) },
				{ ItemID.Code2, autoReuse },
				{ ItemID.CopperAxe, Do(AxePower(50), UseTimeExact(16), TileBoostExact(+0)) },
				{ ItemID.CopperBow, Do(PointBlank, DamageRatio(1.1f)) },
				{ ItemID.CopperBroadsword, Do(AutoReuse, UseTurn) },
				{ ItemID.CopperHammer, Do(HammerPower(35), UseTimeExact(12), TileBoostExact(+0)) },
				{ ItemID.CopperPickaxe, Do(PickPower(35), UseTimeExact(10), TileBoostExact(+0)) },
				{ ItemID.CopperShortsword, autoReuse },
				{ ItemID.CorruptYoyo, Do(AutoReuse, DamageExact(27)) },
				{ ItemID.CrimsonYoyo, Do(AutoReuse, DamageExact(30)) },
				{ ItemID.CrystalVileShard, Do(DamageRatio(1.33f)) },
				{ ItemID.CursedArrow, Do(DamageRatio(1.1f)) },
				{ ItemID.Cutlass, Do(UseRatio(0.8f), DamageRatio(2f)) },
				{ ItemID.DaedalusStormbow, Do(DamageRatio(0.9f)) },
				{ ItemID.DaoofPow, Do(DamageExact(160)) },
				{ ItemID.DarkBlueSolution, Do(Value(Item.buyPrice(silver: 5))) },
				{ ItemID.DarkLance, Do(AutoReuse, TrueMelee, DamageExact(68)) },
				{ ItemID.DartPistol, pointBlank },
				{ ItemID.DartRifle, pointBlank },
				{ ItemID.DayBreak, Do(DamageRatio(1.5f)) }, // TODO -- replace with 1.4 behavior of Daybreak spears exploding on death for 100% damage
				{ ItemID.DD2BallistraTowerT1Popper, Do(UseExact(30)) },
				{ ItemID.DD2BallistraTowerT2Popper, Do(UseExact(25)) },
				{ ItemID.DD2BallistraTowerT3Popper, Do(UseExact(20)) },
				{ ItemID.DD2BetsyBow, Do(DamageRatio(1.1f)) }, // Aerial Bane's ridiculous multiplier is removed, so this compensates for that
				{ ItemID.DD2ExplosiveTrapT1Popper, Do(UseExact(30)) },
				{ ItemID.DD2ExplosiveTrapT2Popper, Do(UseExact(25)) },
				{ ItemID.DD2ExplosiveTrapT3Popper, Do(UseExact(20)) },
				{ ItemID.DD2FlameburstTowerT1Popper, Do(UseExact(30)) },
				{ ItemID.DD2FlameburstTowerT2Popper, Do(UseExact(25)) },
				{ ItemID.DD2FlameburstTowerT3Popper, Do(UseExact(20)) },
				{ ItemID.DD2LightningAuraT1Popper, Do(UseExact(30)) },
				{ ItemID.DD2LightningAuraT2Popper, Do(UseExact(25)) },
				{ ItemID.DD2LightningAuraT3Popper, Do(UseExact(20)) },
				{ ItemID.DD2PhoenixBow, Do(PointBlank, DamageRatio(1.1f)) },
				{ ItemID.DD2SquireDemonSword, Do(DamageExact(110)) },
				{ ItemID.DeadlySphereStaff, Do(UseExact(20)) },
				{ ItemID.DeathbringerPickaxe, Do(PickPower(70), UseTimeExact(10)) },
				{ ItemID.DemonBow, Do(PointBlank, DamageRatio(1.1f)) },
				{ ItemID.DemonScythe, Do(AutoReuse, DamageExact(33), ManaExact(10)) },
				{ ItemID.DiamondStaff, Do(DamageExact(26)) },
				{ ItemID.Drax, Do(TrueMelee, PickPower(200), AxePower(110), UseTimeExact(4), TileBoostExact(+1)) },
				{ ItemID.DyeTradersScimitar, Do(AutoReuse, UseTurn, ScaleRatio(1.5f), DamageRatio(1.33f)) },
				{ ItemID.Dynamite, maxStack999 },
				{ ItemID.EbonwoodBow, Do(PointBlank, DamageRatio(1.1f)) },
				{ ItemID.EbonwoodHammer, Do(HammerPower(25), UseTimeExact(9), TileBoostExact(+0)) },
				{ ItemID.EbonwoodSword, Do(AutoReuse, UseTurn) },
				{ ItemID.ElectrosphereLauncher, Do(DamageRatio(1.1f)) },
				{ ItemID.EldMelter, Do(DamageRatio(1.5f)) }, // intentionally not in alphabetical order to correct for typo
				{ ItemID.EmeraldStaff, Do(DamageExact(28)) },
				// { ItemID.EmpressBlade, Do(UseExact(20)) },
				{ ItemID.EnchantedBoomerang, Do(DamageRatio(2f)) },
				{ ItemID.EnchantedSword, Do(ScaleRatio(1.5f), DamageExact(42), UseAnimationExact(20), ShootSpeedExact(15f)) },
				{ ItemID.EndlessQuiver, Do(DamageRatio(1.1f)) },
				// { ItemID.Eventide, pointBlank },
				{ ItemID.Excalibur, Do(UseTurn, ScaleRatio(1.5f), UseRatio(0.8f), DamageExact(125)) },
				{ ItemID.FalconBlade, Do(UseTurn, ScaleRatio(1.5f), DamageExact(40)) },
				{ ItemID.FieryGreatsword, Do(AutoReuse, UseTurn, ScaleRatio(1.5f), DamageExact(98), UseExact(45)) },
				{ ItemID.FireworksLauncher, Do(DamageRatio(2f)) },
				{ ItemID.Flamarang, Do(DamageRatio(2f)) },
				{ ItemID.Flamelash, Do(DamageExact(75), UseExact(38)) },
				{ ItemID.Flamethrower, Do(DamageRatio(1.5f)) },
				{ ItemID.FlamingArrow, Do(DamageRatio(1.1f)) },
				{ ItemID.FlareGun, pointBlank },
				{ ItemID.FleshGrinder, Do(HammerPower(70), UseTimeExact(13), TileBoostExact(+0)) },
				{ ItemID.FlintlockPistol, pointBlank },
				// { ItemID.FlinxStaff, Do(AutoReuse, UseExact(35)) },
				{ ItemID.FlowerofFire, Do(AutoReuse, DamageExact(60)) },
				{ ItemID.FlowerofFrost, Do(DamageRatio(1.2f)) },
				{ ItemID.FlowerPow, Do(DamageRatio(2f)) },
				{ ItemID.FormatC, autoReuse },
				{ ItemID.Frostbrand, Do(DamageRatio(1.66f)) },
				{ ItemID.FrostburnArrow, Do(DamageRatio(1.1f)) },
				{ ItemID.FrostStaff, Do(ManaExact(9)) },
				{ ItemID.FruitcakeChakram, Do(DamageRatio(2f)) },
				{ ItemID.Gatligator, pointBlank },
				{ ItemID.GladiatorBreastplate, Do(DefenseDelta(+2)) },
				{ ItemID.GladiatorHelmet, Do(DefenseDelta(+1)) },
				{ ItemID.GladiatorLeggings, Do(DefenseDelta(+2)) },
                { ItemID.GoblinBattleStandard, nonConsumableBossSummon },
				{ ItemID.GoldAxe, Do(AxePower(80), UseTimeExact(14), TileBoostExact(+0)) },
				{ ItemID.GoldBow, Do(PointBlank, DamageRatio(1.1f)) },
				{ ItemID.GoldBroadsword, Do(AutoReuse, UseTurn, ScaleRatio(1.5f), DamageRatio(2f)) },
				{ ItemID.GoldHammer, Do(HammerPower(60), UseTimeExact(9), TileBoostExact(+0)) },
				{ ItemID.GoldPickaxe, Do(PickPower(55), UseTimeExact(9)) },
				{ ItemID.GoldShortsword, Do(AutoReuse, DamageRatio(2f)) },
				{ ItemID.GolemFist, Do(DamageExact(150)) },
				{ ItemID.Gradient, autoReuse },
				{ ItemID.GreenPhaseblade, Do(AutoReuse, UseTurn, ScaleRatio(1.5f), DamageRatio(2f)) },
				{ ItemID.GreenPhasesaber, Do(ScaleRatio(1.5f), DamageExact(72), UseExact(20)) },
				{ ItemID.GreenSolution, Do(Value(Item.buyPrice(silver: 5))) },
				{ ItemID.GrenadeLauncher, Do(DamageRatio(2f)) },
				{ ItemID.Gungnir, Do(AutoReuse, TrueMelee, UseRatio(0.8f), DamageExact(92), ShootSpeedRatio(1.25f)) },
				{ ItemID.HallowedGreaves, Do(DefenseDelta(+2)) },
				{ ItemID.HallowedPlateMail, Do(DefenseDelta(+3)) },
				{ ItemID.HallowedRepeater, Do(PointBlank, DamageRatio(1.1f)) },
				{ ItemID.Hammush, Do(HammerPower(85), UseTimeExact(10), TileBoostExact(+1)) },
				{ ItemID.Handgun, pointBlank },
				{ ItemID.Harpoon, pointBlank },
				{ ItemID.HelFire, autoReuse },
				{ ItemID.HellfireArrow, Do(DamageRatio(1.1f)) },
				{ ItemID.HellwingBow, Do(PointBlank, DamageRatio(1.1f)) },
				{ ItemID.HolyArrow, Do(DamageRatio(1.1f)) },
				{ ItemID.HornetStaff, Do(AutoReuse, UseExact(30)) },
				{ ItemID.IceBlade, Do(DamageExact(26), UseTimeExact(33)) },
				{ ItemID.IceBoomerang, Do(DamageRatio(2f)) },
				{ ItemID.IceBow, Do(PointBlank, DamageRatio(1.5f)) },
				{ ItemID.IchorArrow, Do(DamageRatio(1.1f)) },
				{ ItemID.ImpStaff, Do(AutoReuse, UseExact(30)) },
				{ ItemID.InfernoFork, Do(DamageRatio(1.66f)) },
				{ ItemID.InfluxWaver, Do(DamageRatio(0.75f)) },
				{ ItemID.IronAxe, Do(AxePower(60), UseTimeExact(15), TileBoostExact(+0)) },
				{ ItemID.IronBow, Do(PointBlank, DamageRatio(1.1f)) },
				{ ItemID.IronBroadsword, Do(AutoReuse, UseTurn, DamageRatio(1.25f)) },
				{ ItemID.IronHammer, Do(HammerPower(45), UseTimeExact(11), TileBoostExact(+0)) },
				{ ItemID.IronPickaxe, Do(PickPower(40), UseTimeExact(8)) },
				{ ItemID.IronShortsword, Do(AutoReuse, DamageRatio(1.25f)) },
				{ ItemID.JestersArrow, Do(DamageRatio(1.1f)) },
				{ ItemID.JungleYoyo, autoReuse },
				{ ItemID.Katana, Do(UseExact(15), DamageRatio(1.5f)) },
				{ ItemID.Keybrand, Do(UseTurn, ScaleRatio(1.5f), DamageExact(184), UseExact(18)) },
				{ ItemID.KOCannon, Do(DamageRatio(3f)) },
				{ ItemID.Kraken, autoReuse },
				{ ItemID.LastPrism, Do(DamageRatio(0.75f)) },
				{ ItemID.LeadAxe, Do(AxePower(60), UseTimeExact(15), TileBoostExact(+0)) },
				{ ItemID.LeadBow, Do(PointBlank, DamageRatio(1.1f)) },
				{ ItemID.LeadBroadsword, Do(AutoReuse, UseTurn, DamageRatio(1.25f)) },
				{ ItemID.LeadHammer, Do(HammerPower(45), UseTimeExact(11), TileBoostExact(+0)) },
				{ ItemID.LeadPickaxe, Do(PickPower(40), UseTimeExact(8)) },
				{ ItemID.LeadShortsword, Do(AutoReuse, DamageRatio(1.25f)) },
				{ ItemID.LightsBane, Do(AutoReuse, UseTurn, ScaleRatio(1.5f), DamageRatio(2f)) },
				{ ItemID.LunarFlareBook, Do(DamageRatio(1.5f)) },
				{ ItemID.LunarHamaxeNebula, Do(HammerPower(100), AxePower(175), UseTimeExact(5), TileBoostExact(+4)) },
				{ ItemID.LunarHamaxeSolar, Do(HammerPower(100), AxePower(175), UseTimeExact(5), TileBoostExact(+4)) },
				{ ItemID.LunarHamaxeStardust, Do(HammerPower(100), AxePower(175), UseTimeExact(5), TileBoostExact(+4)) },
				{ ItemID.LunarHamaxeVortex, Do(HammerPower(100), AxePower(175), UseTimeExact(5), TileBoostExact(+4)) },
				{ ItemID.MagicDagger, autoReuse },
				{ ItemID.MagicMissile, Do(DamageExact(48), UseExact(15)) },
				{ ItemID.MagnetSphere, Do(DamageRatio(1.1f)) },
				{ ItemID.Marrow, Do(PointBlank, DamageRatio(1.5f)) },
				{ ItemID.MechanicalEye, nonConsumableBossSummon },
				{ ItemID.MechanicalSkull, nonConsumableBossSummon },
				{ ItemID.MechanicalWorm, nonConsumableBossSummon },
				{ ItemID.MedusaHead, Do(DamageRatio(1.66f)) },
				{ ItemID.Megashark, pointBlank },
				{ ItemID.Meowmere, Do(DamageRatio(1.33f)) },
				{ ItemID.MeteorHamaxe, Do(HammerPower(70), AxePower(100), UseTimeExact(16), TileBoostExact(+0)) },
				{ ItemID.Minishark, pointBlank },
				{ ItemID.MoltenFury, Do(PointBlank, DamageRatio(1.1f)) },
				{ ItemID.MoltenHamaxe, Do(HammerPower(75), AxePower(125), UseTimeExact(14), TileBoostExact(+0)) },
				{ ItemID.MoltenPickaxe, Do(PickPower(100), UseTimeExact(10)) },
				{ ItemID.MonkStaffT1, Do(TrueMelee, DamageExact(110)) },
				{ ItemID.MonkStaffT2, Do(AutoReuse, TrueMelee, DamageRatio(2f)) },
				{ ItemID.MonkStaffT3, Do(DamageExact(225)) },
				{ ItemID.MoonlordArrow, Do(DamageRatio(1.1f)) },
				{ ItemID.MoonlordTurretStaff, Do(UseExact(15), DamageRatio(1.5f)) },
				{ ItemID.Muramasa, Do(ScaleRatio(1.5f), DamageRatio(2f)) },
				{ ItemID.MushroomSpear, Do(AutoReuse, TrueMelee, UseRatio(0.8f), DamageExact(100)) },
				{ ItemID.Musket, pointBlank },
				{ ItemID.MythrilChainsaw, Do(TrueMelee, AxePower(80), UseTimeExact(4), TileBoostExact(-1)) },
				{ ItemID.MythrilDrill, Do(TrueMelee, PickPower(160), UseTimeExact(4)) },
				{ ItemID.MythrilHalberd, Do(AutoReuse, TrueMelee, UseRatio(0.8f), DamageExact(95), ShootSpeedRatio(1.25f)) },
				{ ItemID.MythrilPickaxe, Do(PickPower(160), UseTimeExact(8)) },
				{ ItemID.MythrilRepeater, Do(PointBlank, DamageRatio(1.1f)) },
				{ ItemID.MythrilSword, Do(UseTurn, ScaleRatio(1.5f), DamageExact(100), UseExact(25)) },
				{ ItemID.MythrilWaraxe, Do(AxePower(140), UseTimeExact(11), TileBoostExact(+0)) },
                { ItemID.NaughtyPresent, nonConsumableBossSummon },
				{ ItemID.NebulaChainsaw, trueMelee },
				{ ItemID.NebulaDrill, Do(TrueMelee, PickPower(225), UseTimeExact(3), TileBoostExact(+4)) },
				{ ItemID.NebulaPickaxe, Do(PickPower(225), UseTimeExact(6), TileBoostExact(+4)) },
				{ ItemID.NettleBurst, Do(DamageRatio(1.33f)) },
				{ ItemID.NightmarePickaxe, Do(PickPower(66), UseTimeExact(9)) },
				{ ItemID.NightsEdge, Do(AutoReuse, UseTurn, ScaleRatio(1.5f), DamageRatio(3f)) },
				{ ItemID.NorthPole, autoReuse },
				{ ItemID.ObsidianSwordfish, Do(AutoReuse, TrueMelee, DamageExact(45)) },
				{ ItemID.OnyxBlaster, pointBlank },
				{ ItemID.OpticStaff, Do(AutoReuse, UseExact(25), DamageRatio(0.75f)) }, // NOTE: Optic Staff minions have local iframes, so they should be much better overall
				{ ItemID.OrichalcumChainsaw, Do(TrueMelee, AxePower(80), UseTimeExact(4), TileBoostExact(-1)) },
				{ ItemID.OrichalcumDrill, Do(TrueMelee, PickPower(160), UseTimeExact(4)) },
				{ ItemID.OrichalcumHalberd, Do(AutoReuse, TrueMelee, UseRatio(0.8f), DamageExact(98), ShootSpeedRatio(1.25f)) },
				{ ItemID.OrichalcumPickaxe, Do(PickPower(160), UseTimeExact(8)) },
				{ ItemID.OrichalcumRepeater, Do(PointBlank, DamageRatio(1.1f)) },
				{ ItemID.OrichalcumSword, Do(UseTurn, ScaleRatio(1.5f), UseRatio(0.8f), DamageExact(82)) },
				{ ItemID.OrichalcumWaraxe, Do(AxePower(140), UseTimeExact(11), TileBoostExact(+0)) },
				{ ItemID.PainterPaintballGun, pointBlank },
				{ ItemID.PaladinsHammer, Do(AutoReuse, DamageExact(100)) },
				{ ItemID.PalladiumChainsaw, Do(TrueMelee, AxePower(70), UseTimeExact(4), TileBoostExact(-1)) },
				{ ItemID.PalladiumDrill, Do(TrueMelee, PickPower(130), UseTimeExact(5)) },
				{ ItemID.PalladiumPickaxe, Do(PickPower(130), UseTimeExact(9)) },
				{ ItemID.PalladiumPike, Do(AutoReuse, TrueMelee, UseRatio(0.8f), DamageRatio(3f)) },
				{ ItemID.PalladiumRepeater, Do(PointBlank, DamageRatio(1.1f)) },
				{ ItemID.PalladiumSword, Do(UseTurn, ScaleRatio(1.5f), UseRatio(0.8f), DamageExact(100)) },
				{ ItemID.PalladiumWaraxe, Do(AxePower(125), UseTimeExact(12), TileBoostExact(+0)) },
				{ ItemID.PalmWoodBow, Do(PointBlank, DamageRatio(1.1f)) },
				{ ItemID.PalmWoodHammer, Do(HammerPower(25), UseTimeExact(11), TileBoostExact(+0)) },
				{ ItemID.PalmWoodSword, Do(AutoReuse, UseTurn, DamageRatio(4f)) },
				{ ItemID.PearlwoodBow, Do(AutoReuse, PointBlank, DamageRatio(2.31f), UseDelta(+8), ShootSpeedDelta(+3.4f), KnockbackDelta(+1f)) },
				{ ItemID.PearlwoodHammer, Do(HammerPower(25), UseTimeExact(4), UseAnimationExact(20), DamageRatio(4f), TileBoostExact(+0)) },
				{ ItemID.PearlwoodSword, autoReuse },
				{ ItemID.Phantasm, pointBlank },
				{ ItemID.PhoenixBlaster, Do(AutoReuse, PointBlank, DamageRatio(0.75f)) }, // Phoenix Blaster is nerfed to compensate for autofire
				{ ItemID.PickaxeAxe, Do(PickPower(200), AxePower(110), UseTimeExact(7), TileBoostExact(+1)) },
				{ ItemID.Picksaw, Do(PickPower(210), AxePower(125), UseTimeExact(6), TileBoostExact(+1)) },
				{ ItemID.PirateMap, nonConsumableBossSummon },
				{ ItemID.PirateStaff, Do(AutoReuse, UseExact(25)) },
				{ ItemID.PlatinumAxe, Do(AxePower(80), UseTimeExact(14), TileBoostExact(+0)) },
				{ ItemID.PlatinumBow, Do(PointBlank, DamageRatio(1.1f)) },
				{ ItemID.PlatinumBroadsword, Do(AutoReuse, UseTurn, ScaleRatio(1.5f), DamageRatio(2f)) },
				{ ItemID.PlatinumHammer, Do(HammerPower(60), UseTimeExact(9), TileBoostExact(+0)) },
				{ ItemID.PlatinumPickaxe, Do(PickPower(55), UseTimeExact(9)) },
				{ ItemID.PlatinumShortsword, Do(AutoReuse, DamageRatio(2f)) },
				{ ItemID.PoisonStaff, Do(DamageRatio(1.2f)) },
				{ ItemID.ProximityMineLauncher, Do(DamageRatio(2f)) },
				{ ItemID.PsychoKnife, Do(UseTurn, DamageRatio(4f)) },
				{ ItemID.PulseBow, Do(PointBlank, DamageRatio(1.66f)) },
                { ItemID.PumpkinMoonMedallion, nonConsumableBossSummon },
				{ ItemID.PurpleClubberfish, Do(UseTurn, ScaleRatio(1.5f), DamageExact(45), KnockbackExact(10f)) },
				{ ItemID.PurplePhaseblade, Do(AutoReuse, UseTurn, ScaleRatio(1.5f), DamageRatio(2f)) },
				{ ItemID.PurplePhasesaber, Do(ScaleRatio(1.5f), DamageExact(72), UseExact(20)) },
				{ ItemID.PurpleSolution, Do(Value(Item.buyPrice(silver: 5))) },
				{ ItemID.Pwnhammer, Do(HammerPower(80), UseTimeExact(11), TileBoostExact(+1)) },
				{ ItemID.PygmyStaff, Do(UseExact(20)) },
				// { ItemID.QuadBarrelShotgun, pointBlank },
				{ ItemID.QueenSpiderStaff, Do(UseExact(25)) },
				{ ItemID.RainbowCrystalStaff, Do(UseExact(15)) },
				{ ItemID.RainbowRod, Do(DamageExact(130)) },
				{ ItemID.Rally, Do(AutoReuse, DamageExact(20)) },
				{ ItemID.RavenStaff, Do(UseExact(20)) },
				{ ItemID.Razorpine, Do(DamageRatio(0.75f)) },
				{ ItemID.ReaverShark, Do(PickPower(100), UseTimeExact(16)) },
				{ ItemID.RedPhaseblade, Do(AutoReuse, UseTurn, ScaleRatio(1.5f), DamageRatio(2f)) },
				{ ItemID.RedPhasesaber, Do(ScaleRatio(1.5f), DamageExact(72), UseExact(20)) },
				{ ItemID.RedRyder, pointBlank },
				{ ItemID.RedSolution, Do(Value(Item.buyPrice(silver: 5))) },
				{ ItemID.RedsYoyo, autoReuse },
				{ ItemID.Revolver, pointBlank },
				{ ItemID.RichMahoganyBow, Do(PointBlank, DamageRatio(1.1f)) },
				{ ItemID.RichMahoganyHammer, Do(HammerPower(25), UseTimeExact(10), TileBoostExact(+0)) },
				{ ItemID.RichMahoganySword, Do(AutoReuse, UseTurn) },
				{ ItemID.RocketIII, Do(DamageRatio(0.75f)) },
				{ ItemID.RocketIV, Do(DamageRatio(0.75f)) },
				{ ItemID.RocketLauncher, Do(DamageRatio(2f)) },
				{ ItemID.Rockfish, Do(HammerPower(50), UseTimeExact(10), TileBoostExact(+0)) },
				{ ItemID.RubyStaff, Do(DamageExact(25)) },
				{ ItemID.Sandgun, pointBlank },
				{ ItemID.SapphireStaff, Do(AutoReuse, DamageExact(25)) },
				// { ItemID.SanguineStaff, Do(AutoReuse, UseExact(25)) },
				{ ItemID.SawtoothShark, Do(TrueMelee, AxePower(45), UseTimeExact(4), TileBoostExact(-1)) },
				{ ItemID.SDMG, pointBlank },
				{ ItemID.Seedler, Do(DamageRatio(1.5f)) },
				{ ItemID.ShadewoodBow, Do(PointBlank, DamageRatio(1.1f)) },
				{ ItemID.ShadewoodHammer, Do(HammerPower(25), UseTimeExact(9), TileBoostExact(+0)) },
				{ ItemID.ShadewoodSword, Do(AutoReuse, UseTurn) },
				{ ItemID.ShadowbeamStaff, Do(DamageRatio(2f)) },
				{ ItemID.ShadowFlameBow, Do(PointBlank, DamageRatio(1.1f)) },
				{ ItemID.Shotgun, pointBlank },
				{ ItemID.ShroomiteDiggingClaw, Do(PickPower(200), AxePower(125), UseTimeExact(4), TileBoostExact(-1)) },
				{ ItemID.SilverAxe, Do(AxePower(70), UseTimeExact(14), TileBoostExact(+0)) },
				{ ItemID.SilverBow, Do(PointBlank, DamageRatio(1.1f)) },
				{ ItemID.SilverBroadsword, Do(AutoReuse, UseTurn, DamageRatio(1.5f)) },
				{ ItemID.SilverHammer, Do(HammerPower(55), UseTimeExact(10), TileBoostExact(+0)) },
				{ ItemID.SilverPickaxe, Do(PickPower(50), UseTimeExact(11)) },
				{ ItemID.SilverShortsword, Do(AutoReuse, DamageRatio(1.5f)) },
				{ ItemID.SlapHand, Do(UseTurn, ScaleRatio(1.5f), DamageExact(120)) },
				{ ItemID.SlimeCrown, nonConsumableBossSummon },
				{ ItemID.SlimeStaff, Do(AutoReuse, UseExact(30)) },
				// { ItemID.Smolstar, Do(AutoReuse, UseExact(25)) },
				{ ItemID.SniperRifle, pointBlank },
				{ ItemID.SnowballCannon, pointBlank },
				{ ItemID.SnowGlobe, Do(MaxStack(20)) },
				{ ItemID.SolarEruption, Do(DamageRatio(1.5f)) },
				{ ItemID.SolarFlareBreastplate, Do(DefenseDelta(+7)) },
				{ ItemID.SolarFlareChainsaw, trueMelee },
				{ ItemID.SolarFlareDrill, Do(TrueMelee, PickPower(225), UseTimeExact(3), TileBoostExact(+4)) },
				{ ItemID.SolarFlareHelmet, Do(DefenseDelta(+5)) },
				{ ItemID.SolarFlareLeggings, Do(DefenseDelta(+4)) },
				{ ItemID.SolarFlarePickaxe, Do(PickPower(225), UseTimeExact(6), TileBoostExact(+4)) },
				{ ItemID.SoulDrain, Do(DamageRatio(1.33f)) },
				{ ItemID.SpaceGun, Do(DamageExact(25)) },
				{ ItemID.Spear, Do(AutoReuse, TrueMelee, DamageRatio(2f)) },
				{ ItemID.SpectreHamaxe, Do(HammerPower(90), AxePower(170), UseTimeExact(8), TileBoostExact(+4)) },
				{ ItemID.SpectrePickaxe, Do(PickPower(200), UseTimeExact(8), TileBoostExact(+4)) },
				{ ItemID.SpectreStaff, Do(DamageRatio(3f)) },
				{ ItemID.SpiderStaff, Do(AutoReuse, UseExact(25)) },
				{ ItemID.StaffofEarth, Do(DamageRatio(1.66f)) },
				{ ItemID.StaffoftheFrostHydra, Do(UseExact(20)) },
				{ ItemID.StakeLauncher, Do(PointBlank, DamageRatio(1.25f)) },
				{ ItemID.StardustCellStaff, Do(UseExact(20)) },
				{ ItemID.StardustChainsaw, trueMelee },
				{ ItemID.StardustDragonStaff, Do(AutoReuse, DamageExact(20), UseExact(19)) },
				{ ItemID.StardustDrill, Do(TrueMelee, PickPower(225), UseTimeExact(3), TileBoostExact(+4)) },
				{ ItemID.StardustPickaxe, Do(PickPower(225), UseTimeExact(6), TileBoostExact(+4)) },
				{ ItemID.Starfury, autoReuse },
				{ ItemID.StarWrath, Do(DamageRatio(0.9f)) },
				{ ItemID.StickyBomb, maxStack999 },
				{ ItemID.StickyDynamite, maxStack999 },
				// { ItemID.StormSpear, autoReuse },
				// { ItemID.StormTigerStaff, UseExact(20) },
				{ ItemID.StylistKilLaKillScissorsIWish, Do(AutoReuse, UseTurn, ScaleRatio(1.5f), DamageExact(33)) },
				{ ItemID.Sunfury, Do(DamageRatio(2f)) },
				{ ItemID.SuspiciousLookingEye, nonConsumableBossSummon },
				{ ItemID.Swordfish, Do(AutoReuse, TrueMelee, DamageExact(38)) },
				{ ItemID.TacticalShotgun, Do(PointBlank, DamageRatio(1.2f)) },
				{ ItemID.TaxCollectorsStickOfDoom, Do(AutoReuse, UseTurn, ScaleRatio(1.5f), UseRatio(0.8f), DamageExact(70)) },
				{ ItemID.TempestStaff, Do(UseExact(20)) },
				{ ItemID.TendonBow, Do(PointBlank, DamageRatio(1.1f)) },
				{ ItemID.Terrarian, Do(AutoReuse, DamageExact(352)) },
				{ ItemID.TheAxe, Do(HammerPower(100), AxePower(175), UseTimeExact(7), TileBoostExact(+1)) },
				{ ItemID.TheBreaker, Do(HammerPower(70), UseTimeExact(13), TileBoostExact(+0)) },
				{ ItemID.TheEyeOfCthulhu, autoReuse },
				{ ItemID.TheHorsemansBlade, Do(UseTurn, ScaleRatio(1.5f), UseRatio(0.8f), DamageExact(95)) },
				{ ItemID.TheMeatball, Do(DamageRatio(2f)) },
				{ ItemID.TheRottedFork, Do(AutoReuse, TrueMelee, DamageExact(20)) },
				{ ItemID.TheUndertaker, pointBlank },
				{ ItemID.ThornChakram, Do(DamageRatio(2f)) },
				{ ItemID.TinAxe, Do(AxePower(50), UseTimeExact(16), TileBoostExact(+0)) },
				{ ItemID.TinBow, Do(PointBlank, DamageRatio(1.1f)) },
				{ ItemID.TinBroadsword, Do(AutoReuse, UseTurn) },
				{ ItemID.TinHammer, Do(HammerPower(35), UseTimeExact(12), TileBoostExact(+0)) },
				{ ItemID.TinPickaxe, Do(PickPower(35), UseTimeExact(10), TileBoostExact(+0)) },
				{ ItemID.TinShortsword, autoReuse },
				{ ItemID.TitaniumChainsaw, Do(TrueMelee, AxePower(90), UseTimeExact(4), TileBoostExact(+0)) },
				{ ItemID.TitaniumDrill, Do(TrueMelee, PickPower(180), UseTimeExact(4), TileBoostExact(+1)) },
				{ ItemID.TitaniumPickaxe, Do(PickPower(180), UseTimeExact(8), TileBoostExact(+1)) },
				{ ItemID.TitaniumRepeater, Do(PointBlank, DamageRatio(1.1f)) },
				{ ItemID.TitaniumSword, Do(UseTurn, ScaleRatio(1.5f), UseRatio(0.8f), DamageExact(77)) },
				{ ItemID.TitaniumTrident, Do(AutoReuse, TrueMelee, UseRatio(0.8f), DamageExact(72), ShootSpeedRatio(1.25f)) },
				{ ItemID.TitaniumWaraxe, Do(AxePower(160), UseTimeExact(10), TileBoostExact(+1)) },
				{ ItemID.TopazStaff, Do(ManaExact(2)) },
				{ ItemID.Trident, Do(AutoReuse, TrueMelee, DamageRatio(2f)) },
				{ ItemID.TrueExcalibur, Do(AutoReuse, UseTurn, DamageRatio(1.33f)) },
				{ ItemID.TrueNightsEdge, Do(AutoReuse, UseTurn, DamageRatio(1.66f)) },
				{ ItemID.Tsunami, Do(PointBlank, DamageRatio(1.1f)) },
				{ ItemID.TungstenAxe, Do(AxePower(70), UseTimeExact(14), TileBoostExact(+0)) },
				{ ItemID.TungstenBow, Do(PointBlank, DamageRatio(1.1f)) },
				{ ItemID.TungstenBroadsword, Do(AutoReuse, UseTurn, DamageRatio(1.5f)) },
				{ ItemID.TungstenHammer, Do(HammerPower(55), UseTimeExact(10), TileBoostExact(+0)) },
				{ ItemID.TungstenPickaxe, Do(PickPower(50), UseTimeExact(11)) },
				{ ItemID.TungstenShortsword, Do(AutoReuse, DamageRatio(1.5f)) },
				{ ItemID.UnholyArrow, Do(DamageRatio(1.1f)) },
				{ ItemID.UnholyTrident, Do(ManaExact(14)) },
				{ ItemID.Uzi, pointBlank },
				{ ItemID.ValkyrieYoyo, autoReuse },
				// { ItemID.VampireFrogStaff, Do(AutoReuse, UseExact(30)) }
				{ ItemID.VampireKnives, Do(DamageRatio(1.33f)) },
				{ ItemID.VenomArrow, Do(DamageRatio(1.1f)) },
				{ ItemID.VenomStaff, Do(DamageRatio(1.33f)) },
				{ ItemID.VenusMagnum, Do(AutoReuse, PointBlank) },
				{ ItemID.Vilethorn, Do(DamageExact(14)) },
				{ ItemID.VortexBeater, pointBlank },
				{ ItemID.VortexChainsaw, trueMelee },
				{ ItemID.VortexDrill, Do(TrueMelee, PickPower(225), UseTimeExact(3), TileBoostExact(+4)) },
				{ ItemID.VortexPickaxe, Do(PickPower(225), UseTimeExact(6), TileBoostExact(+4)) },
				{ ItemID.WandofSparking, Do(AutoReuse, DamageExact(15)) },
				{ ItemID.WarAxeoftheNight, Do(AxePower(100), UseTimeExact(13), TileBoostExact(+0)) },
				{ ItemID.WaspGun, Do(DamageRatio(1.5f)) },
				{ ItemID.WaterBolt, Do(DamageExact(28)) },
				{ ItemID.WhitePhaseblade, Do(AutoReuse, UseTurn, ScaleRatio(1.5f), DamageRatio(2f)) },
				{ ItemID.WhitePhasesaber, Do(ScaleRatio(1.5f), DamageExact(72), UseExact(20)) },
				{ ItemID.WoodenArrow, Do(DamageRatio(1.1f)) },
				{ ItemID.WoodenBoomerang, Do(DamageRatio(2f)) },
				{ ItemID.WoodenBow, Do(PointBlank, DamageRatio(1.1f)) },
				{ ItemID.WoodenHammer, Do(HammerPower(25), UseTimeExact(11), TileBoostExact(+0)) },
				{ ItemID.WoodenSword, Do(AutoReuse, UseTurn) },
				{ ItemID.WoodYoyo, autoReuse },
				{ ItemID.WormFood, nonConsumableBossSummon },
				{ ItemID.Xenopopper, Do(DamageRatio(0.75f)) },
				{ ItemID.XenoStaff, Do(UseExact(20)) },
				{ ItemID.Yelets, autoReuse },
				{ ItemID.YellowPhaseblade, Do(AutoReuse, UseTurn, ScaleRatio(1.5f), DamageRatio(2f)) },
				{ ItemID.YellowPhasesaber, Do(ScaleRatio(1.5f), DamageExact(72), UseExact(20)) },
				{ ItemID.ZombieArm, Do(AutoReuse, UseTurn, ScaleRatio(1.5f), DamageRatio(2f)) },
			};
		}

		internal static void UnloadBalance()
		{
			balance.Clear();
			balance = null;
		}
		#endregion

		#region SetDefaults (Item Balance Applied Here)
		internal void SetDefaults_ApplyBalance(Item item)
		{
			// Do nothing if the balance database is not defined.
			if (balance is null)
				return;

			// Grab the balancing to apply, if any. If nothing comes back, do nothing.
			bool needsBalancing = balance.TryGetValue(item.type, out IBalanceRule[] rules);
			if (!needsBalancing)
				return;

			// Apply all balance changes sequentially, assuming they are relevant.
			foreach (IBalanceRule rule in rules)
				if (rule.AppliesTo(item))
					rule.ApplyBalance(item);
		}
		#endregion

		#region Internal Structures

		// This function simply concatenates a bunch of Balance Rules into an array.
		// It looks a lot nicer than constantly typing "new IBalanceRule[]".
		internal static IBalanceRule[] Do(params IBalanceRule[] r) => r;

		#region Applicability Lambdas
		internal static bool DealsDamage(Item it) => it.damage > 0;
		internal static bool HasDefense(Item it) => it.defense > 0;
		internal static bool HasKnockback(Item it) => !it.accessory & !it.vanity; // how to check if something is wearable armor?
		internal static bool IsAxe(Item it) => it.axe > 0;
		internal static bool IsHammer(Item it) => it.hammer > 0;
		internal static bool IsMelee(Item it) => it.melee;
		internal static bool IsPickaxe(Item it) => it.pick > 0;
		internal static bool IsScalable(Item it) => it.damage > 0 && it.melee; // sanity check: only melee weapons get scaled
		internal static bool IsUsable(Item it) => it.useStyle != 0 && it.useTime > 0 && it.useAnimation > 0;
		internal static bool UsesMana(Item it) => IsUsable(it); // Only usable items cost mana, but items must be able to have their mana cost disabled or enabled at will.
		#endregion

		#region Balance Rules
		internal interface IBalanceRule
		{
			bool AppliesTo(Item it);
			void ApplyBalance(Item it);
		}

		internal class AutoReuseRule : IBalanceRule
		{
			internal readonly bool flag = true;

			public AutoReuseRule(bool ar) => flag = ar;
			public bool AppliesTo(Item it) => IsUsable(it);
			public void ApplyBalance(Item it) => it.autoReuse = flag;
		}
		internal static IBalanceRule AutoReuse => new AutoReuseRule(true);
		internal static IBalanceRule NoAutoReuse => new AutoReuseRule(false);

		// Uses the values shown by Terraria, which are multiplied by 5, not the internal values
		internal class AxePowerRule : IBalanceRule
		{
			internal readonly int newAxePower = 0;

			public AxePowerRule(int newDisplayedAxePower) => newAxePower = newDisplayedAxePower / 5;
			public bool AppliesTo(Item it) => IsAxe(it);
			public void ApplyBalance(Item it)
			{
				it.axe = newAxePower;
				if (it.axe < 0)
					it.axe = 0;
			}
		}
		internal static IBalanceRule AxePower(int a) => new AxePowerRule(a);

		internal class ConsumableRule : IBalanceRule
		{
			internal readonly bool flag = false;

			public ConsumableRule(bool c) => flag = c;
			public bool AppliesTo(Item it) => true;
			public void ApplyBalance(Item it) => it.consumable = flag;
		}
		internal static IBalanceRule Consumable => new ConsumableRule(true);
		internal static IBalanceRule NotConsumable => new ConsumableRule(false);

		#region Damage
		internal class DamageDeltaRule : IBalanceRule
		{
			internal readonly int delta = 0;

			public DamageDeltaRule(int d) => delta = d;
			public bool AppliesTo(Item it) => DealsDamage(it);
			public void ApplyBalance(Item it)
			{
				it.damage += delta;
				if (it.damage < 0)
					it.damage = 0;
			}
		}
		internal static IBalanceRule DamageDelta(int d) => new DamageDeltaRule(d);

		internal class DamageExactRule : IBalanceRule
		{
			internal readonly int newDamage = 0;

			public DamageExactRule(int dmg) => newDamage = dmg;
			public bool AppliesTo(Item it) => DealsDamage(it);
			public void ApplyBalance(Item it)
			{
				it.damage = newDamage;
				if (it.damage < 0)
					it.damage = 0;
			}
		}
		internal static IBalanceRule DamageExact(int d) => new DamageExactRule(d);

		internal class DamageRatioRule : IBalanceRule
		{
			internal readonly float ratio = 1f;

			public DamageRatioRule(float f) => ratio = f;
			public bool AppliesTo(Item it) => DealsDamage(it);
			public void ApplyBalance(Item it)
			{
				it.damage = (int)(it.damage * ratio);
				if (it.damage < 0)
					it.damage = 0;
			}
		}
		internal static IBalanceRule DamageRatio(float f) => new DamageRatioRule(f);
		#endregion

		internal class DefenseDeltaRule : IBalanceRule
		{
			internal readonly int delta = 0;

			public DefenseDeltaRule(int d) => delta = d;
			public bool AppliesTo(Item it) => HasDefense(it);
			public void ApplyBalance(Item it)
			{
				it.defense += delta;
				if (it.defense < 0)
					it.defense = 0;
			}
		}
		internal static IBalanceRule DefenseDelta(int d) => new DefenseDeltaRule(d);

		internal class DefenseExactRule : IBalanceRule
		{
			internal readonly int newDefense = 0;

			public DefenseExactRule(int def) => newDefense = def;
			public bool AppliesTo(Item it) => HasDefense(it);
			public void ApplyBalance(Item it)
			{
				it.defense = newDefense;
				if (it.defense < 0)
					it.defense = 0;
			}
		}
		internal static IBalanceRule DefenseExact(int d) => new DefenseExactRule(d);

		internal class HammerPowerRule : IBalanceRule
		{
			internal readonly int newHammerPower = 0;

			public HammerPowerRule(int h) => newHammerPower = h;
			public bool AppliesTo(Item it) => IsHammer(it);
			public void ApplyBalance(Item it)
			{
				it.hammer = newHammerPower;
				if (it.hammer < 0)
					it.hammer = 0;
			}
		}
		internal static IBalanceRule HammerPower(int h) => new HammerPowerRule(h);

		#region Knockback
		internal class KnockbackDeltaRule : IBalanceRule
		{
			internal readonly float delta = 0;

			public KnockbackDeltaRule(float d) => delta = d;
			public bool AppliesTo(Item it) => HasKnockback(it);
			public void ApplyBalance(Item it)
			{
				it.knockBack += delta;
				if (it.knockBack < 0f)
					it.knockBack = 0f;
			}
		}
		internal static IBalanceRule KnockbackDelta(float d) => new KnockbackDeltaRule(d);

		internal class KnockbackExactRule : IBalanceRule
		{
			internal readonly float newKnockback = 0;

			public KnockbackExactRule(float kb) => newKnockback = kb;
			public bool AppliesTo(Item it) => HasKnockback(it);
			public void ApplyBalance(Item it)
			{
				it.knockBack = newKnockback;
				if (it.knockBack < 0f)
					it.knockBack = 0f;
			}
		}
		internal static IBalanceRule KnockbackExact(float kb) => new KnockbackExactRule(kb);

		internal class KnockbackRatioRule : IBalanceRule
		{
			internal readonly float ratio = 1f;

			public KnockbackRatioRule(float f) => ratio = f;
			public bool AppliesTo(Item it) => HasKnockback(it);
			public void ApplyBalance(Item it)
			{
				it.knockBack *= ratio;
				if (it.knockBack < 0f)
					it.knockBack = 0f;
			}
		}
		internal static IBalanceRule KnockbackRatio(float r) => new KnockbackRatioRule(r);
		#endregion

		#region Mana Cost
		internal class ManaDeltaRule : IBalanceRule
		{
			internal readonly int delta = 0;

			public ManaDeltaRule(int d) => delta = d;
			public bool AppliesTo(Item it) => UsesMana(it);
			public void ApplyBalance(Item it)
			{
				it.mana += delta;
				if (it.mana < 0)
					it.mana = 0;
			}
		}
		internal static IBalanceRule ManaDelta(int d) => new ManaDeltaRule(d);

		internal class ManaExactRule : IBalanceRule
		{
			internal readonly int newMana = 0;

			public ManaExactRule(int m) => newMana = m;
			public bool AppliesTo(Item it) => UsesMana(it);
			public void ApplyBalance(Item it)
			{
				it.mana = newMana;
				if (it.mana < 0)
					it.mana = 0;
			}
		}
		internal static IBalanceRule ManaExact(int m) => new ManaExactRule(m);

		internal class ManaRatioRule : IBalanceRule
		{
			internal readonly float ratio = 1f;

			public ManaRatioRule(float f) => ratio = f;
			public bool AppliesTo(Item it) => UsesMana(it);
			public void ApplyBalance(Item it)
			{
				it.mana = (int)(it.mana * ratio);
				if (it.mana < 0)
					it.mana = 0;
			}
		}
		internal static IBalanceRule ManaRatio(float f) => new ManaRatioRule(f);
		#endregion

		internal class MaxStackRule : IBalanceRule // max stack plus - calamity style
		{
			internal readonly int newMaxStack = 999;

			public MaxStackRule(int stk) => newMaxStack = stk;
			public bool AppliesTo(Item it) => true;
			public void ApplyBalance(Item it)
			{
				it.maxStack = newMaxStack;
				if (it.maxStack < 1)
					it.maxStack = 1;
			}
		}
		internal static IBalanceRule MaxStack(int stk) => new MaxStackRule(stk);
		
		internal class PickPowerRule : IBalanceRule
		{
			internal readonly int newPickPower = 0;

			public PickPowerRule(int p) => newPickPower = p;
			public bool AppliesTo(Item it) => IsPickaxe(it);
			public void ApplyBalance(Item it)
			{
				it.pick = newPickPower;
				if (it.pick < 0)
					it.pick = 0;
			}
		}
		internal static IBalanceRule PickPower(int p) => new PickPowerRule(p);

		internal class PointBlankRule : IBalanceRule
		{
			public bool AppliesTo(Item it) => true;
			public void ApplyBalance(Item it) => it.Calamity().canFirePointBlankShots = true;
		}
		internal static IBalanceRule PointBlank => new PointBlankRule();
		
		#region Scale (True Melee)
		internal class ScaleDeltaRule : IBalanceRule
		{
			internal readonly float delta = 0;

			public ScaleDeltaRule(float d) => delta = d;
			public bool AppliesTo(Item it) => IsScalable(it);
			public void ApplyBalance(Item it)
			{
				it.scale += delta;
				if (it.scale < 0f)
					it.scale = 0f;
			}
		}
		internal static IBalanceRule ScaleDelta(float d) => new ScaleDeltaRule(d);

		internal class ScaleExactRule : IBalanceRule
		{
			internal readonly float newScale = 0;

			public ScaleExactRule(float s) => newScale = s;
			public bool AppliesTo(Item it) => IsScalable(it);
			public void ApplyBalance(Item it)
			{
				it.scale = newScale;
				if (it.scale < 0f)
					it.scale = 0f;
			}
		}
		internal static IBalanceRule ScaleExact(float s) => new ScaleExactRule(s);

		internal class ScaleRatioRule : IBalanceRule
		{
			internal readonly float ratio = 1f;

			public ScaleRatioRule(float f) => ratio = f;
			public bool AppliesTo(Item it) => IsScalable(it);
			public void ApplyBalance(Item it)
			{
				it.scale *= ratio;
				if (it.scale < 0f)
					it.scale = 0f;
			}
		}
		internal static IBalanceRule ScaleRatio(float f) => new ScaleRatioRule(f);
		#endregion

		#region Shoot Speed (Velocity)
		internal class ShootSpeedDeltaRule : IBalanceRule
		{
			internal readonly float delta = 0;

			public ShootSpeedDeltaRule(float d) => delta = d;
			public bool AppliesTo(Item it) => IsScalable(it);
			public void ApplyBalance(Item it)
			{
				it.shootSpeed += delta;
				if (it.shootSpeed < 0f)
					it.shootSpeed = 0f;
			}
		}
		internal static IBalanceRule ShootSpeedDelta(float d) => new ShootSpeedDeltaRule(d);

		internal class ShootSpeedExactRule : IBalanceRule
		{
			internal readonly float newShootSpeed = 0;

			public ShootSpeedExactRule(float ss) => newShootSpeed = ss;
			public bool AppliesTo(Item it) => IsScalable(it);
			public void ApplyBalance(Item it)
			{
				it.shootSpeed = newShootSpeed;
				if (it.shootSpeed < 0f)
					it.shootSpeed = 0f;
			}
		}
		internal static IBalanceRule ShootSpeedExact(float s) => new ShootSpeedExactRule(s);

		internal class ShootSpeedRatioRule : IBalanceRule
		{
			internal readonly float ratio = 1f;

			public ShootSpeedRatioRule(float f) => ratio = f;
			public bool AppliesTo(Item it) => IsScalable(it);
			public void ApplyBalance(Item it)
			{
				it.shootSpeed *= ratio;
				if (it.shootSpeed < 0f)
					it.shootSpeed = 0f;
			}
		}
		internal static IBalanceRule ShootSpeedRatio(float f) => new ShootSpeedRatioRule(f);
		#endregion

		internal class TileBoostDeltaRule : IBalanceRule
		{
			private readonly int delta = 0;

			public TileBoostDeltaRule(int d) => delta = d;
			public bool AppliesTo(Item it) => true;
			public void ApplyBalance(Item it) => it.tileBoost += delta;
		}
		internal static IBalanceRule TileBoostDelta(int d) => new TileBoostDeltaRule(d);

		internal class TileBoostExactRule : IBalanceRule
		{
			private readonly int newTileBoost = 0;

			public TileBoostExactRule(int tb) => newTileBoost = tb;
			public bool AppliesTo(Item it) => true;
			public void ApplyBalance(Item it) => it.tileBoost = newTileBoost;
		}
		internal static IBalanceRule TileBoostExact(int tb) => new TileBoostExactRule(tb);
		
		internal class TrueMeleeRule : IBalanceRule
		{
			public bool AppliesTo(Item it) => IsMelee(it);
			public void ApplyBalance(Item it) => it.Calamity().trueMelee = true;
		}
		internal static IBalanceRule TrueMelee => new TrueMeleeRule();

		#region Use Time and Use Animation
		internal class UseDeltaRule : IBalanceRule
		{
			internal readonly int delta = 0;

			public UseDeltaRule(int d) => delta = d;
			public bool AppliesTo(Item it) => IsUsable(it);
			public void ApplyBalance(Item it)
			{
				it.useAnimation += delta;
				it.useTime += delta;
				if (it.useAnimation < 1)
					it.useAnimation = 1;
				if (it.useTime < 1)
					it.useTime = 1;
			}
		}
		internal static IBalanceRule UseDelta(int d) => new UseDeltaRule(d);

		internal class UseExactRule : IBalanceRule
		{
			internal readonly int newUseTime = 0;

			public UseExactRule(int ut) => newUseTime = ut;
			public bool AppliesTo(Item it) => IsUsable(it);
			public void ApplyBalance(Item it)
			{
				it.useAnimation = newUseTime;
				it.useTime = newUseTime;
				if (it.useAnimation < 1)
					it.useAnimation = 1;
				if (it.useTime < 1)
					it.useTime = 1;
			}
		}
		internal static IBalanceRule UseExact(int ut) => new UseExactRule(ut);

		internal class UseRatioRule : IBalanceRule
		{
			internal readonly float ratio = 1f;

			public UseRatioRule(float f) => ratio = f;
			public bool AppliesTo(Item it) => IsUsable(it);
			public void ApplyBalance(Item it)
			{
				it.useAnimation = (int)(it.useAnimation * ratio);
				it.useTime = (int)(it.useTime * ratio);
				if (it.useAnimation < 1)
					it.useAnimation = 1;
				if (it.useTime < 1)
					it.useTime = 1;
			}
		}
		internal static IBalanceRule UseRatio(float f) => new UseRatioRule(f);
		
		internal class UseAnimationDeltaRule : IBalanceRule
		{
			internal readonly int delta = 0;

			public UseAnimationDeltaRule(int d) => delta = d;
			public bool AppliesTo(Item it) => IsUsable(it);
			public void ApplyBalance(Item it)
			{
				it.useAnimation += delta;
				if (it.useAnimation < 1)
					it.useAnimation = 1;
			}
		}
		internal static IBalanceRule UseAnimationDelta(int d) => new UseAnimationDeltaRule(d);

		internal class UseAnimationExactRule : IBalanceRule
		{
			internal readonly int newUseAnimation = 0;

			public UseAnimationExactRule(int ua) => newUseAnimation = ua;
			public bool AppliesTo(Item it) => IsUsable(it);
			public void ApplyBalance(Item it)
			{
				it.useAnimation = newUseAnimation;
				if (it.useAnimation < 1)
					it.useAnimation = 1;
			}
		}
		internal static IBalanceRule UseAnimationExact(int ua) => new UseAnimationExactRule(ua);

		internal class UseAnimationRatioRule : IBalanceRule
		{
			internal readonly float ratio = 1f;

			public UseAnimationRatioRule(float f) => ratio = f;
			public bool AppliesTo(Item it) => IsUsable(it);
			public void ApplyBalance(Item it)
			{
				it.useAnimation = (int)(it.useAnimation * ratio);
				if (it.useAnimation < 1)
					it.useAnimation = 1;
			}
		}
		internal static IBalanceRule UseAnimationRatio(float f) => new UseAnimationRatioRule(f);

		internal class UseTimeDeltaRule : IBalanceRule
		{
			internal readonly int delta = 0;

			public UseTimeDeltaRule(int d) => delta = d;
			public bool AppliesTo(Item it) => IsUsable(it);
			public void ApplyBalance(Item it)
			{
				it.useTime += delta;
				if (it.useTime < 1)
					it.useTime = 1;
			}
		}
		internal static IBalanceRule UseTimeDelta(int d) => new UseTimeDeltaRule(d);

		internal class UseTimeExactRule : IBalanceRule
		{
			internal readonly int newUseTime = 0;

			public UseTimeExactRule(int ut) => newUseTime = ut;
			public bool AppliesTo(Item it) => IsUsable(it);
			public void ApplyBalance(Item it)
			{
				it.useTime = newUseTime;
				if (it.useTime < 1)
					it.useTime = 1;
			}
		}
		internal static IBalanceRule UseTimeExact(int ut) => new UseTimeExactRule(ut);

		internal class UseTimeRatioRule : IBalanceRule
		{
			internal readonly float ratio = 1f;

			public UseTimeRatioRule(float f) => ratio = f;
			public bool AppliesTo(Item it) => IsUsable(it);
			public void ApplyBalance(Item it)
			{
				it.useTime = (int)(it.useTime * ratio);
				if (it.useTime < 1)
					it.useTime = 1;
			}
		}
		internal static IBalanceRule UseTimeRatio(float f) => new UseTimeRatioRule(f);
		#endregion

		internal class UseTurnRule : IBalanceRule
		{
			internal readonly bool flag = true;

			public UseTurnRule(bool ut) => flag = ut;
			public bool AppliesTo(Item it) => IsUsable(it);
			public void ApplyBalance(Item it) => it.useTurn = flag;
		}
		internal static IBalanceRule UseTurn => new UseTurnRule(true);
		internal static IBalanceRule NoUseTurn => new UseTurnRule(false);

		internal class ValueRule : IBalanceRule
		{
			internal readonly int newValue = 0;

			public ValueRule(int v) => newValue = v;
			public bool AppliesTo(Item it) => true;
			public void ApplyBalance(Item it)
			{
				it.value = newValue;
				if (it.value < 0)
					it.value = 0;
			}
		}
		internal static IBalanceRule Value(int v) => new ValueRule(v);
		internal static IBalanceRule Worthless => new ValueRule(0);
		#endregion
		#endregion
	}
}
