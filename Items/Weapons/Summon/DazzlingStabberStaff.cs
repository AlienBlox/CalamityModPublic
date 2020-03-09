using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class DazzlingStabberStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dazzling Stabber Staff");
            Tooltip.SetDefault("Summons a holy blade to fight for you");
        }

        public override void SetDefaults()
        {
            item.width = 54;
            item.height = 52;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.UseSound = SoundID.DD2_DarkMageHealImpact;
            item.summon = true;
            item.mana = 12;
            item.damage = 350;
            item.knockBack = 2f;
            item.autoReuse = true;
            item.useTime = item.useAnimation = 21;
            item.shoot = ModContent.ProjectileType<DazzlingStabber>();
            item.shootSpeed = 13f;

            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
            {
                Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, type, damage, knockBack, player.whoAmI);
            }
            float angleMax = MathHelper.ToRadians(45f);
            if (CalamityUtils.CountProjectiles(type) == 1)
                angleMax = 0f;
            float index = 1f;
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].type == type && Main.projectile[i].owner == player.whoAmI)
                {
                    Main.projectile[i].ai[1] = (index / CalamityUtils.CountProjectiles(type)) * angleMax - angleMax / 2f;
                    Main.projectile[i].netUpdate = true;
                    index++;
                }
            }
            return false;
        }
    }
}
