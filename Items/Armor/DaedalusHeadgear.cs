using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class DaedalusHeadgear : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Daedalus Mask");
            Tooltip.SetDefault("5% increased minion damage");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 25, 0, 0);
            item.rare = 5;
            item.defense = 3; //33
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<DaedalusBreastplate>() && legs.type == ModContent.ItemType<DaedalusLeggings>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "20% increased minion damage and +2 max minions\n" +
                "A daedalus crystal floats above you to protect you";
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.daedalusCrystal = true;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.FindBuffIndex(ModContent.BuffType<DaedalusSummonSetBuff>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<DaedalusSummonSetBuff>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<DaedalusCrystal>()] < 1)
                {
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, ModContent.ProjectileType<DaedalusCrystal>(), (int)(95f * player.MinionDamage()), 0f, Main.myPlayer, 50f, 0f);
                }
            }
            player.minionDamage += 0.2f;
			player.maxMinions += 2;
		}

		public override void UpdateEquip(Player player)
        {
            player.minionDamage += 0.05f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<VerstaltiteBar>(), 8);
			recipe.AddIngredient(ItemID.CrystalShard, 6);
			recipe.AddIngredient(ModContent.ItemType<EssenceofEleum>());
			recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
