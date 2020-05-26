using CalamityMod.Items.Weapons.Rogue;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Prefixes
{
	public class RoguePrefix : ModPrefix
	{
		//Thank you Thomas for helping me set this up =D
		internal static List<byte> RogueModifiers;
		internal float damageMult = 1f;
		internal float useTimeMult = 1f;
		internal int critBonus = 0;
		internal float shootSpeedMult = 1f;
		internal float stealthDmgMult = 1f;

		public override PrefixCategory Category => PrefixCategory.Custom;

		public RoguePrefix()
		{

		}

		public RoguePrefix(float damageMult = 1f, float useTimeMult = 1f, int critBonus = 0, float shootSpeedMult = 1f, float stealthDmgMult = 1f)
		{
			this.damageMult = damageMult;
			this.useTimeMult = useTimeMult;
			this.critBonus = critBonus;
			this.shootSpeedMult = shootSpeedMult;
			this.stealthDmgMult = stealthDmgMult;
		}

		public override bool Autoload(ref string name)
		{
			if (base.Autoload(ref name))
			{
				//Damage, Use Time, Crit, Velocity, SS Dmg
				RogueModifiers = new List<byte>();
				AddRoguePrefix(mod, RoguePrefixType.Pointy, 1.1f, 1f, 0, 1f, 1f);
				AddRoguePrefix(mod, RoguePrefixType.Sharp, 1.15f, 1f, 0, 1f, 1f);
				AddRoguePrefix(mod, RoguePrefixType.Feathered, 1f, 0.85f, 0, 1.1f, 1f);
				AddRoguePrefix(mod, RoguePrefixType.Sleek, 1f, 0.9f, 0, 1.15f, 1f);
				AddRoguePrefix(mod, RoguePrefixType.Hefty, 1.1f, 1f, 0, 1f, 1.15f);
				AddRoguePrefix(mod, RoguePrefixType.Mighty, 1.15f, 1f, 0, 1f, 1.05f);
				AddRoguePrefix(mod, RoguePrefixType.Glorious, 1.1f, 0.95f, 0, 1f, 1f);
				AddRoguePrefix(mod, RoguePrefixType.Serrated, 1.1f, 0.9f, 0, 1.05f, 1f);
				AddRoguePrefix(mod, RoguePrefixType.Vicious, 1.1f, 0.95f, 0, 1.15f, 1f);
				AddRoguePrefix(mod, RoguePrefixType.Lethal, 1.1f, 0.95f, 2, 1.05f, 1.05f);
				AddRoguePrefix(mod, RoguePrefixType.Flawless, 1.15f, 0.9f, 5, 1.1f, 1.15f);
				AddRoguePrefix(mod, RoguePrefixType.Radical, 1.05f, 0.95f, 0, 1.05f, 0.9f);
				AddRoguePrefix(mod, RoguePrefixType.Blunt, 0.85f, 1f, 0, 1f, 1f);
				AddRoguePrefix(mod, RoguePrefixType.Flimsy, 0.9f, 1f, 0, 1f, 0.9f);
				AddRoguePrefix(mod, RoguePrefixType.Unbalanced, 1f, 1.15f, 0, 0.95f, 1f);
				AddRoguePrefix(mod, RoguePrefixType.Atrocious, 0.85f, 1f, 0, 0.9f, 0.9f);
			}
			return false;
		}

		public override void Apply(Item item)
		{
			ModItem moddedItem = item.modItem;
			if(moddedItem != null && moddedItem is RogueWeapon rogueWep)
			{
				rogueWep.StealthStrikeDamage = stealthDmgMult;
			}
		}

		public override void ModifyValue(ref float valueMult)
		{
			float extraValue = 1f + (1.1f * (stealthDmgMult - 1f));
			valueMult *= extraValue;
		}

		public override bool CanRoll(Item item) => item.Calamity().rogue && !item.consumable;

		public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
		{
			damageMult = this.damageMult;
			useTimeMult = this.useTimeMult;
			critBonus = this.critBonus;
			shootSpeedMult = this.shootSpeedMult;
		}

		private static void AddRoguePrefix(Mod mod, RoguePrefixType prefixType, float damageMult = 1f, float useTimeMult = 1f, int critBonus = 0, float shootSpeedMult = 1f, float stealthDmgMult = 1f)
		{
			string name = prefixType.ToString();
			mod.AddPrefix(name, new RoguePrefix(damageMult, useTimeMult, critBonus, shootSpeedMult, stealthDmgMult));
			RogueModifiers.Add(mod.GetPrefix(name).Type);
		}

		public override void ValidateItem(Item item, ref bool invalid)
		{
			if (item.damage == Math.Round(item.damage * damageMult))
				invalid = true;
			if (item.useAnimation == Math.Round(item.useAnimation * useTimeMult))
				invalid = true;
		}
	}

	public enum RoguePrefixType : byte
	{
		Pointy,
		Sharp,
		Feathered,
		Sleek,
		Hefty,
		Mighty,
		Glorious,
		Serrated,
		Vicious,
		Lethal,
		Flawless,
		Radical,
		Blunt,
		Flimsy,
		Unbalanced,
		Atrocious
	}
}