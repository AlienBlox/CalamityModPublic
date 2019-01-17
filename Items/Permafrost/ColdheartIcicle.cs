using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Permafrost
{
	public class ColdheartIcicle : ModItem
	{
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Coldheart Icicle");
			Tooltip.SetDefault("Drains a percentage of enemy health on hit\nCannot inflict critical hits");
		}
		public override void SetDefaults()
		{
			item.damage = 1;
			item.melee = true;
			item.width = 26;
			item.height = 26;
            item.useTime = 27;
            item.useAnimation = 27;
            item.autoReuse = true;
            item.useStyle = 3;
            item.UseSound = SoundID.Item1;
            item.useTurn = true;
			item.knockBack = 3f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
		}
        public override void ModifyHitPvp(Player player, Player target, ref int damage, ref bool crit)
        {
            damage = target.statLifeMax2 * 2 / 100;
            target.statDefense = 0;
            target.endurance = 0f;
            crit = false;
        }
        public override void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
        {
            damage = 1;
            crit = false;
            if (target.type != NPCID.TargetDummy)
                target.life -= target.lifeMax * 2 / 100;
            target.checkDead();
        }
    }
}
