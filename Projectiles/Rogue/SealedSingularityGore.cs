using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class SealedSingularityGore : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sealed Singularity Fragment");
        }

        public override void SetDefaults()
        {
            projectile.friendly = true;
            projectile.width = 25;
            projectile.height = 25;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            //Rotation and gravity
            projectile.rotation += 0.6f * projectile.direction;
            projectile.velocity.Y = projectile.velocity.Y + 0.27f;
            if (projectile.velocity.Y > 16f)
            {
                projectile.velocity.Y = 16f;
            }

        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(0, (int)projectile.position.X, (int)projectile.position.Y);
            //Dust effect
            int splash = 0;
            while (splash < 4)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 31, -projectile.velocity.X * 0.15f, -projectile.velocity.Y * 0.10f, 150, default, 0.9f);
                splash += 1;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture;
            switch (projectile.ai[0])
            {
                
                case 2f: texture = ModContent.GetTexture("CalamityMod/Projectiles/Rogue/SealedSingularityGore2");
                         break;
                case 3f: texture = ModContent.GetTexture("CalamityMod/Projectiles/Rogue/SealedSingularityGore3");
                         break;
                default: texture = Main.projectileTexture[projectile.type];
                         break;
            }
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, texture.Width, texture.Height)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2(texture.Width / 2f, texture.Height / 2f), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
