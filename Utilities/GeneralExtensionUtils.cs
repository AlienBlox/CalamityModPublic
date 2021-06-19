using CalamityMod.CalPlayer;
using CalamityMod.Items;
using CalamityMod.NPCs;
using CalamityMod.Projectiles;
using Terraria;

namespace CalamityMod
{
	public static partial class CalamityUtils
	{
		public static CalamityPlayer Calamity(this Player player) => player.GetModPlayer<CalamityPlayer>();
		public static CalamityGlobalNPC Calamity(this NPC npc) => npc.active ? npc.GetGlobalNPC<CalamityGlobalNPC>() : null;
		public static CalamityGlobalItem Calamity(this Item item) => item.GetGlobalItem<CalamityGlobalItem>();
		public static CalamityGlobalProjectile Calamity(this Projectile proj) => proj.GetGlobalProjectile<CalamityGlobalProjectile>();
		public static Item ActiveItem(this Player player) => Main.mouseItem.IsAir ? player.HeldItem : Main.mouseItem;
	}
}
