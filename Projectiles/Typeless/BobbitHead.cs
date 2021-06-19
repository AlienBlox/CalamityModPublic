using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Projectiles.Typeless
{
	public class BobbitHead : ModProjectile
	{
		public const float PullSpeed = 24f;
		public const float ReelbackSpeed = 28f;
		public const float LaunchSpeed = 25f;
		public const float GrappleRangInTiles = 40f;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bobbit Head");
		}

		public override void SetDefaults()
		{
			projectile.CloneDefaults(ProjectileID.GemHookAmethyst);
		}

		// Use this hook for hooks that can have multiple hooks mid-flight: Dual Hook, Web Slinger, Fish Hook, Static Hook, Lunar Hook
		public override bool? CanUseGrapple(Player player)
		{
			int hooksOut = 0;
			for (int l = 0; l < Main.maxProjectiles; l++)
			{
				if (Main.projectile[l].active && Main.projectile[l].owner == Main.myPlayer && Main.projectile[l].type == projectile.type)
				{
					hooksOut++;
				}
			}
			if (hooksOut > 1) // This hook can have 2 hooks out.
			{
				return false;
			}
			return true;
		}

		// Amethyst Hook is 300, Static Hook is 600, 16f = 1 tile
		public override float GrappleRange() => GrappleRangInTiles * 16f;

		public override void NumGrappleHooks(Player player, ref int numHooks)
		{
			numHooks = 1;
		}

		// default is 11, Lunar is 24
		public override void GrappleRetreatSpeed(Player player, ref float speed)
		{
			speed = ReelbackSpeed;
		}

		public override void GrapplePullSpeed(Player player, ref float speed)
		{
			speed = PullSpeed;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawHook(projectile, GetTexture("CalamityMod/ExtraTextures/Chains/BobbitHookChain"));
			return true;
		}

        public override void AI()
        {
            projectile.spriteDirection = -projectile.direction;

			if (projectile.ai[0] == 2f)
				projectile.extraUpdates = 1;
			else
				projectile.extraUpdates = 0;
        }
    }
}
