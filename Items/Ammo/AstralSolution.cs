﻿using CalamityMod.Projectiles.Typeless;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    public class AstralSolution : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
           			ItemID.Sets.SortingPriorityTerraforming[Type] = 95; // Red Solution
        }

        public override void SetDefaults()
        {
            Item.ammo = AmmoID.Solution;
            Item.shoot = ModContent.ProjectileType<AstralSpray>() - ProjectileID.PureSpray;
            Item.width = 10;
            Item.height = 12;
            Item.value = Item.buyPrice(0, 0, 5, 0);
            Item.rare = ItemRarityID.Orange;
            Item.maxStack = 9999;
            Item.ResearchUnlockCount = 99;
            Item.consumable = true;
            return;
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.Solutions;
		}

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return !(player.itemAnimation < player.ActiveItem().useAnimation - 3);
        }
    }
}
