using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class DirectStrike : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nondescript Damaging Entity");
        }

        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.extraUpdates = 0;
            projectile.ignoreWater = true;
            projectile.alpha = 255;
            projectile.timeLeft = 2;
        }

        // If the AI parameter isn't a valid NPC slot, it can hit anything. Otherwise it can only hit one NPC.
		// TODO -- this might actually not work in multiplayer if the DirectStrike gets synced...
        public override bool? CanHitNPC(NPC target)
        {
			if (projectile.ai[0] < 0f || projectile.ai[0] > 199f || projectile.ai[0] == target.whoAmI)
                return null;
            return (bool?)false;
        }
    }
}
