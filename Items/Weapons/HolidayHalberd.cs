using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class HolidayHalberd : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holiday Halberd");
            Tooltip.SetDefault("idk I'm miserable with names\n" +
                "- The General\n" +
                "Fires red and green bouncing balls that emit clouds as they travel");
        }

        public override void SetDefaults()
        {
            item.width = 70;
            item.damage = 94;
            item.melee = true;
            item.useAnimation = 17;
            item.useStyle = 1;
            item.useTime = 17;
            item.useTurn = true;
            item.knockBack = 7.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 72;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
            item.shoot = ModContent.ProjectileType<RedBall>();
            item.shootSpeed = 12f;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            int dustType = 0;
            switch (Main.rand.Next(4))
            {
                case 1:
                    dustType = 107;
                    break;
                case 2:
                    dustType = 90;
                    break;
            }
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, dustType);
            }
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (Main.rand.NextBool(3))
            {
                type = ModContent.ProjectileType<GreenBall>();
            }
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, Main.myPlayer);
            return false;
        }
    }
}
