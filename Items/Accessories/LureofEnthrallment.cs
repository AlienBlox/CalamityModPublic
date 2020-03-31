using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class LureofEnthrallment : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pearl of Enthrallment");
            Tooltip.SetDefault("Summons a siren to fight for you\n" +
                "The siren stays above you, shooting water spears, ice mist, and treble clefs at nearby enemies");
        }

        public override void SetDefaults()
        {
            item.width = 56;
            item.height = 56;
            item.value = Item.buyPrice(0, 30, 0, 0);
            item.rare = 7;
            item.accessory = true;
        }

        public override bool CanEquipAccessory(Player player, int slot)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.elementalHeart)
            {
                return false;
            }
            return true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.sirenWaifu = true;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.FindBuffIndex(ModContent.BuffType<WaterWaifu>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<WaterWaifu>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<WaterElementalMinion>()] < 1)
                {
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, ModContent.ProjectileType<WaterElementalMinion>(), (int)(65 * player.MinionDamage()), 2f, Main.myPlayer, 0f, 0f);
                }
            }
        }
    }
}
