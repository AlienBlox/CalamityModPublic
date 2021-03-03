using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.World;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class GladiatorsLocket : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gladiator's Locket");
            Tooltip.SetDefault("Summons two spirit swords to protect you");
        }

        public override void SetDefaults()
        {
            item.width = 42;
            item.height = 36;
            item.value = CalamityGlobalItem.Rarity3BuyPrice;
            item.rare = ItemRarityID.Orange;
            item.defense = 5;
            item.accessory = true;
        }

        public override bool CanEquipAccessory(Player player, int slot)
        {
            CalamityPlayer modPlayer = player.Calamity(); //there might be an upgrade sometime later?
            if (modPlayer.gladiatorSword)
            {
                return false;
            }
            return true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.gladiatorSword = true;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.FindBuffIndex(ModContent.BuffType<GladiatorSwords>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<GladiatorSwords>(), 3600, true);
                }
				int damage = Main.hardMode ? 50 : 20;
				int swordDmg = (int)(damage * player.AverageDamage());
                if (player.ownedProjectileCounts[ModContent.ProjectileType<GladiatorSword>()] < 1)
                {
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<GladiatorSword>(), swordDmg, 6f, Main.myPlayer, 0f, 0f);
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<GladiatorSword2>(), swordDmg, 6f, Main.myPlayer, 0f, 0f);
                }
            }
        }
    }
}
