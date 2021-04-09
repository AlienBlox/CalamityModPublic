using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class NullShot2 : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Null");
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 120;
            projectile.ranged = true;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 3f)
            {
                for (int num134 = 0; num134 < 10; num134++)
                {
                    float x = projectile.position.X - projectile.velocity.X / 10f * (float)num134;
                    float y = projectile.position.Y - projectile.velocity.Y / 10f * (float)num134;
                    int num135 = Dust.NewDust(new Vector2(x, y), 1, 1, 66, 0f, 0f, 0, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1f);
                    Main.dust[num135].alpha = projectile.alpha;
                    Main.dust[num135].position.X = x;
                    Main.dust[num135].position.Y = y;
                    Main.dust[num135].velocity *= 0f;
                    Main.dust[num135].noGravity = true;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.type == NPCID.TargetDummy)
            {
                return;
            }
            int nullBuff = Main.rand.Next(10);
            if (!target.boss)
            {
                if (nullBuff == 0)
                {
                    target.scale *= 5f;
                }
                else if (nullBuff == 1)
                {
                    target.scale *= 0.1f;
                }
                else if (nullBuff == 2)
                {
                    target.damage += 20;
                }
                else if (nullBuff == 3)
                {
                    target.damage -= 20;
                }
                else if (nullBuff == 4)
                {
                    target.knockBackResist = 0f;
                }
                else if (nullBuff == 5)
                {
                    target.knockBackResist = 1f;
                }
                else if (nullBuff == 6)
                {
                    target.defense += 10;
                }
                else if (nullBuff == 7)
                {
                    target.defense -= 10;
                }
                else if (nullBuff == 8)
                {
                    target.velocity.Y = Main.rand.NextBool(2) ? 30f : -30f;
                }
                else
                {
                    target.velocity.X = Main.rand.NextBool(2) ? 30f : -30f;
                }
            }
        }
    }
}
