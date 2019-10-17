using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class BlackAnurian : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Black Anurian");
            Tooltip.SetDefault("Spews bubbles and homing plankton");
        }

        public override void SetDefaults()
        {
            item.damage = 36;
            item.magic = true;
            item.mana = 10;
            item.width = 58;
            item.height = 38;
            item.useTime = 14;
            item.useAnimation = 14;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 2.75f;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = 3;
            item.UseSound = SoundID.Item111;
            item.autoReuse = true;
            item.shootSpeed = 8f;
            item.shoot = ModContent.ProjectileType<Projectiles.BlackAnurian>();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int num6 = 2;
            for (int index = 0; index < num6; ++index)
            {
                float SpeedX = speedX + (float)Main.rand.Next(-25, 26) * 0.05f;
                float SpeedY = speedY + (float)Main.rand.Next(-25, 26) * 0.05f;
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<BlackAnurianPlankton>(), (int)((double)damage * 0.5), knockBack, player.whoAmI, 0.0f, 0.0f);
            }
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, (int)((double)damage * 0.5), knockBack, player.whoAmI, 0.0f, 0.0f);
            return false;
        }
    }
}
