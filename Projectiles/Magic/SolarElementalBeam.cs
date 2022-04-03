using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class SolarElementalBeam : BaseLaserbeamProjectile
    {
        public override float MaxScale => 1f;
        public override float MaxLaserLength => 1000f;
        public override float Lifetime => 30f;
        public override Color LightCastColor => Color.White;
        public override Texture2D LaserBeginTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/UltimaRayStart");
        public override Texture2D LaserMiddleTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/UltimaRayMid");
        public override Texture2D LaserEndTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/UltimaRayEnd");

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beam");
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 10;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 13;
            Projectile.tileCollide = false;
            Projectile.timeLeft = (int)Lifetime;
        }

        public override void ExtraBehavior()
        {
            // Generate a star-like burst of solar fire dust.
            if (!Main.dedServ && Time == 5f)
            {
                int starPoints = 8;
                for (int i = 0; i < starPoints; i++)
                {
                    float angle = MathHelper.TwoPi * i / starPoints;
                    for (int j = 0; j < 12; j++)
                    {
                        float starSpeed = MathHelper.Lerp(2f, 10f, j / 12f);
                        Color dustColor = Color.Lerp(Color.White, Color.Yellow, j / 12f);
                        float dustScale = MathHelper.Lerp(1.6f, 0.85f, j / 12f);

                        Dust fire = Dust.NewDustPerfect(Projectile.Center, 6);
                        fire.velocity = angle.ToRotationVector2() * starSpeed;
                        fire.color = dustColor;
                        fire.scale = dustScale;
                        fire.noGravity = true;
                    }
                }
            }
        }

        public override void DetermineScale() => Projectile.scale = Projectile.timeLeft / Lifetime * MaxScale;


        public override bool PreDraw(ref Color lightColor)
        {
            DrawBeamWithColor(spriteBatch, Color.Lerp(Color.OrangeRed, Color.Transparent, 0.25f), Projectile.scale);
            DrawBeamWithColor(spriteBatch, Color.Lerp(Color.Yellow * 1.1f, Color.Transparent, 0.25f), Projectile.scale * 0.4f);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            int type = ModContent.ProjectileType<FuckYou>();
            int boomDamage = (int)(damage * 1.1);
            int boom = Projectile.NewProjectile(target.Center, Vector2.Zero, type, boomDamage, knockback, Projectile.owner, 0f, Main.rand.NextFloat(0.85f, 2f));
            Main.projectile[boom].Calamity().forceMagic = true;
        }
    }
}
