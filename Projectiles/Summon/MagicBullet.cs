using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
	public class MagicBullet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bullet");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.light = 0.5f;
            projectile.alpha = 255;
            projectile.extraUpdates = 7;
            projectile.scale = 1.18f;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.ignoreWater = true;
            projectile.aiStyle = 1;
            aiType = 242;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
			projectile.tileCollide = false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.BetsysCurse, 180);
            target.AddBuff(BuffID.Ichor, 180);
            target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 180);
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 180);
            target.AddBuff(ModContent.BuffType<WarCleave>(), 180);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.BetsysCurse, 180);
            target.AddBuff(BuffID.Ichor, 180);
            target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 180);
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 180);
            target.AddBuff(ModContent.BuffType<WarCleave>(), 180);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(189, 51, 164, projectile.alpha);
        }
    }
}
